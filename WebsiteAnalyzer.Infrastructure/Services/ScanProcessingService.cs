using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
                string url = scan.Website_Url;

                // =========================
                // 1️⃣ REAL HTTP REQUEST
                // =========================
                var stopwatch = Stopwatch.StartNew();
                var response = await _httpClient.GetAsync(url);
                stopwatch.Stop();

                var html = await response.Content.ReadAsStringAsync();

                int loadTimeMs = (int)stopwatch.ElapsedMilliseconds;
                int pageSizeKb = Encoding.UTF8.GetByteCount(html) / 1024;

                // =========================
                // 2️⃣ HTML PARSING
                // =========================
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

                // =========================
                // 3️⃣ SECURITY HEADERS
                // =========================
                var headers = response.Headers;

                bool hasCsp = headers.Contains("Content-Security-Policy");
                bool hasHsts = headers.Contains("Strict-Transport-Security");
                bool hasXFrame = headers.Contains("X-Frame-Options");
                bool hasXss = headers.Contains("X-XSS-Protection");
                bool isHttps = url.StartsWith("https://");

                // =========================
                // 4️⃣ SAVE PERFORMANCE METRICS
                // =========================
                _context.Performance_Metrics.Add(new PerformanceMetric
                {
                    Scan_Id = scanId,
                    Page_Url = url,
                    Load_Time_MS = loadTimeMs,
                    Page_Size_KB = pageSizeKb,
                    Total_Requests = totalRequests,
                    Created_On = DateTime.UtcNow,
                    Is_Active = true,
                    Is_Deleted = false
                });

                // =========================
                // 5️⃣ SAVE SECURITY HEADERS
                // =========================
                AddSecurityHeader(scanId, "Content-Security-Policy", hasCsp);
                AddSecurityHeader(scanId, "Strict-Transport-Security", hasHsts);
                AddSecurityHeader(scanId, "X-Frame-Options", hasXFrame);
                AddSecurityHeader(scanId, "X-XSS-Protection", hasXss);

                // =========================
                // 6️⃣ SAVE AUTOMATED ISSUES
                // =========================
                if (imagesWithoutAlt > 0)
                    AddIssue(scanId, "CodeQuality",
                        "Images missing ALT attribute",
                        $"{imagesWithoutAlt} images found without ALT attribute",
                        Severity.Medium);

                if (inlineScripts > 0)
                    AddIssue(scanId, "CodeQuality",
                        "Inline JavaScript detected",
                        "Inline JavaScript reduces maintainability",
                        Severity.Low);

                if (!hasCsp)
                    AddIssue(scanId, "Security",
                        "Missing Content-Security-Policy",
                        "CSP header is missing",
                        Severity.High);

                // =========================
                // 7️⃣ SCORE CALCULATION
                // =========================
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

                // =========================
                // 8️⃣ UPDATE SCAN
                // =========================
                scan.Performance_Score = Math.Max(performanceScore, 0);
                scan.Security_Score = Math.Max(securityScore, 0);
                scan.Code_Quality_Score = Math.Max(codeQualityScore, 0);
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
        // HELPER METHODS
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
