using HtmlAgilityPack;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Domain.Entities;
using WebsiteAnalyzer.Domain.Enums;
using WebsiteAnalyzer.Infrastructure.Data;

namespace WebsiteAnalyzer.Application.Services
{
    public class ScanProcessingService : IScanProcessingService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public ScanProcessingService(ApplicationDbContext context)
        {
            _context = context;

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            _httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("WebsiteAnalyzer", "1.0"));
        }

        public async Task ProcessScanAsync(int scanId)
        {
            var scan = await _context.Website_Scans.FindAsync(scanId);
            if (scan == null) return;

            scan.Scan_Status = ScanStatus.InProgress;
            scan.Modified_On = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            try
            {
                var pages = await GetPagesToScan(scan.Website_Url);

                int totalPerformance = 0;
                int totalSecurity = 0;
                int totalCodeQuality = 0;

                int pageCount = 0;

                foreach (var pageUrl in pages)
                {
                    pageCount++;

                    var result = await AnalyzePage(scanId, pageUrl);

                    totalPerformance += result.Performance;
                    totalSecurity += result.Security;
                    totalCodeQuality += result.CodeQuality;
                }

                scan.Performance_Score = totalPerformance / pageCount;
                scan.Security_Score = totalSecurity / pageCount;
                scan.Code_Quality_Score = totalCodeQuality / pageCount;

                scan.Scan_Status = ScanStatus.Completed;
                scan.Modified_On = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            catch
            {
                scan.Scan_Status = ScanStatus.Failed;
                scan.Modified_On = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        // =========================
        // PAGE DISCOVERY
        // =========================

        private async Task<List<string>> GetPagesToScan(string rootUrl)
        {
            var pages = new List<string> { rootUrl };

            var html = await _httpClient.GetStringAsync(rootUrl);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var links = doc.DocumentNode.SelectNodes("//a[@href]");

            if (links != null)
            {
                foreach (var link in links)
                {
                    var href = link.GetAttributeValue("href", "");

                    if (string.IsNullOrWhiteSpace(href))
                        continue;

                    if (href.StartsWith("/"))
                        pages.Add(new Uri(new Uri(rootUrl), href).ToString());

                    else if (href.StartsWith(rootUrl))
                        pages.Add(href);

                    if (pages.Count >= 5)
                        break;
                }
            }

            return pages.Distinct().ToList();
        }

        // =========================
        // PAGE ANALYSIS
        // =========================

        private async Task<(int Performance, int Security, int CodeQuality)> AnalyzePage(int scanId, string pageUrl)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _httpClient.GetAsync(pageUrl);
            stopwatch.Stop();

            var html = await response.Content.ReadAsStringAsync();

            int loadTimeMs = (int)stopwatch.ElapsedMilliseconds;
            int pageSizeKb = Encoding.UTF8.GetByteCount(html) / 1024;

            _context.Scan_Pages.Add(new ScanPage
            {
                Scan_Id = scanId,
                Page_Url = pageUrl,
                Http_Status_Code = (int)response.StatusCode,
                Page_Load_Time_MS = loadTimeMs,
                Created_On = DateTime.UtcNow,
                Is_Active = true,
                Is_Deleted = false
            });

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            int scripts = doc.DocumentNode.SelectNodes("//script[@src]")?.Count ?? 0;
            int styles = doc.DocumentNode.SelectNodes("//link[@rel='stylesheet']")?.Count ?? 0;
            int images = doc.DocumentNode.SelectNodes("//img")?.Count ?? 0;

            int totalRequests = scripts + styles + images + 1;

            int imagesWithoutAlt =
                doc.DocumentNode.SelectNodes("//img[not(@alt)]")?.Count ?? 0;

            int inlineScripts =
                doc.DocumentNode.SelectNodes("//script[not(@src)]")?.Count ?? 0;

            int inlineStyles =
                doc.DocumentNode.SelectNodes("//style")?.Count ?? 0;

            int unminifiedJs =
                doc.DocumentNode.SelectNodes("//script[@src]")
                    ?.Count(n => !n.GetAttributeValue("src", "").Contains(".min.")) ?? 0;

            var headers = response.Headers;

            bool hasCsp = headers.Contains("Content-Security-Policy");
            bool hasHsts = headers.Contains("Strict-Transport-Security");
            bool hasXFrame = headers.Contains("X-Frame-Options");
            bool hasXss = headers.Contains("X-XSS-Protection");
            bool isHttps = pageUrl.StartsWith("https://");

            _context.Performance_Metrics.Add(new PerformanceMetric
            {
                Scan_Id = scanId,
                Page_Url = pageUrl,
                Load_Time_MS = loadTimeMs,
                Page_Size_KB = pageSizeKb,
                Total_Requests = totalRequests,
                Created_On = DateTime.UtcNow,
                Is_Active = true,
                Is_Deleted = false
            });

            AddSecurityHeader(scanId, "Content-Security-Policy", hasCsp);
            AddSecurityHeader(scanId, "Strict-Transport-Security", hasHsts);
            AddSecurityHeader(scanId, "X-Frame-Options", hasXFrame);
            AddSecurityHeader(scanId, "X-XSS-Protection", hasXss);

            if (imagesWithoutAlt > 0)
                AddIssue(scanId, "CodeQuality",
                    "Images missing ALT attribute",
                    $"{imagesWithoutAlt} images without ALT on {pageUrl}",
                    Severity.Medium);

            if (inlineScripts > 0)
                AddIssue(scanId, "CodeQuality",
                    "Inline JavaScript detected",
                    $"Inline JavaScript found on {pageUrl}",
                    Severity.Low);

            if (!hasCsp)
                AddIssue(scanId, "Security",
                    "Missing Content-Security-Policy",
                    $"CSP header missing on {pageUrl}",
                    Severity.High);

            await DetectBrokenLinks(scanId, pageUrl, doc);
            await DetectBrokenImages(scanId, pageUrl, doc);

            int performanceScore = 100;
            if (loadTimeMs > 3000) performanceScore -= 20;
            if (pageSizeKb > 1024) performanceScore -= 20;
            if (totalRequests > 30) performanceScore -= 20;
            if (unminifiedJs > 0) performanceScore -= 10;

            int securityScore = 100;
            if (!isHttps) securityScore -= 30;
            if (!hasCsp) securityScore -= 20;
            if (!hasHsts) securityScore -= 20;

            int codeQualityScore = 100;
            if (imagesWithoutAlt > 0) codeQualityScore -= 10;
            if (inlineScripts > 0) codeQualityScore -= 10;
            if (inlineStyles > 0) codeQualityScore -= 10;

            return (
                Math.Max(performanceScore, 0),
                Math.Max(securityScore, 0),
                Math.Max(codeQualityScore, 0)
            );
        }

        // =========================
        // BUG DETECTION
        // =========================

        private async Task DetectBrokenLinks(int scanId, string pageUrl, HtmlDocument doc)
        {
            var links = doc.DocumentNode.SelectNodes("//a[@href]");

            if (links == null) return;

            foreach (var link in links.Take(5))
            {
                var href = link.GetAttributeValue("href", "");

                if (string.IsNullOrWhiteSpace(href))
                    continue;

                string linkUrl = href.StartsWith("/")
                    ? new Uri(new Uri(pageUrl), href).ToString()
                    : href;

                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Head, linkUrl);
                    var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                        AddIssue(scanId, "Bug",
                            "Broken Link",
                            $"Broken link detected: {linkUrl}",
                            Severity.High);
                }
                catch
                {
                    AddIssue(scanId, "Bug",
                        "Invalid Link",
                        $"Invalid link detected: {linkUrl}",
                        Severity.High);
                }
            }
        }

        private async Task DetectBrokenImages(int scanId, string pageUrl, HtmlDocument doc)
        {
            var images = doc.DocumentNode.SelectNodes("//img[@src]");

            if (images == null) return;

            foreach (var img in images.Take(5))
            {
                var src = img.GetAttributeValue("src", "");

                if (string.IsNullOrWhiteSpace(src))
                    continue;

                string imgUrl = src.StartsWith("/")
                    ? new Uri(new Uri(pageUrl), src).ToString()
                    : src;

                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Head, imgUrl);
                    var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                        AddIssue(scanId, "Bug",
                            "Broken Image",
                            $"Broken image detected: {imgUrl}",
                            Severity.Medium);
                }
                catch
                {
                    AddIssue(scanId, "Bug",
                        "Broken Image",
                        $"Invalid image source: {imgUrl}",
                        Severity.Medium);
                }
            }
        }

        // =========================
        // HELPERS
        // =========================

        private void AddSecurityHeader(int scanId, string name, bool exists)
        {
            _context.Security_Headers.Add(new SecurityHeader
            {
                Scan_Id = scanId,
                Header_Name = name,
                Status = exists ? "Present" : "Missing",
                Risk_Level = exists ? "Low" : "High",
                Created_On = DateTime.UtcNow,
                Is_Active = true,
                Is_Deleted = false
            });
        }

        private void AddIssue(int scanId, string category, string title,
            string description, Severity severity)
        {
            _context.Automated_Issues.Add(new AutomatedIssue
            {
                Scan_Id = scanId,
                Issue_Category = category,
                Issue_Title = title,
                Issue_Description = description,
                Severity = severity,
                Created_On = DateTime.UtcNow,
                Is_Active = true,
                Is_Deleted = false
            });
        }
    }
}