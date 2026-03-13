using Microsoft.EntityFrameworkCore;
using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Domain.Entities;
using WebsiteAnalyzer.Domain.Enums;
using WebsiteAnalyzer.Infrastructure.Data;

namespace WebsiteAnalyzer.Infrastructure.Repositories
{
    public class WebsiteScanRepository : IWebsiteScanRepository
    {
        private readonly ApplicationDbContext _context;

        public WebsiteScanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateScanAsync(
            int userId,
            string websiteUrl,
            string scanType)
        {
            var scan = new WebsiteScan
            {
                User_Id = userId,
                Website_Url = websiteUrl,
                Scan_Type = scanType,
                Scan_Status = ScanStatus.Pending,
                Created_On = DateTime.UtcNow,
                Is_Active = true,
                Is_Deleted = false
            };

            _context.Website_Scans.Add(scan);
            await _context.SaveChangesAsync();

            return scan.Id;
        }

        public async Task<List<ScanListDto>> GetUserScansAsync(int userId)
        {
            return await _context.Website_Scans
                .Where(x => x.User_Id == userId && !x.Is_Deleted)
                .OrderByDescending(x => x.Created_On)
                .Select(x => new ScanListDto
                {
                    Scan_Id = x.Id,
                    Website_Url = x.Website_Url,

                    Scan_Status = x.Scan_Status == ScanStatus.Pending ? "Pending" :
                                  x.Scan_Status == ScanStatus.InProgress ? "In Process" :
                                  x.Scan_Status == ScanStatus.Completed ? "Completed" :
                                  "Failed",

                    Performance_Score = x.Performance_Score,
                    Security_Score = x.Security_Score,
                    Code_Quality_Score = x.Code_Quality_Score,

                    BugsFound = _context.Automated_Issues
                        .Count(i => i.Scan_Id == x.Id && i.Issue_Category == "Bug"),

                    SecurityIssues = _context.Automated_Issues
                        .Count(i => i.Scan_Id == x.Id && i.Issue_Category == "Security"),

                    Created_On = x.Created_On
                })
                .ToListAsync();
        }

        public async Task<ScanResultDto> GetScanResultsAsync(int scanId)
        {
            var scan = await _context.Website_Scans
                .Where(x => x.Id == scanId)
                .Select(x => new
                {
                    x.Id,
                    x.Website_Url,
                    x.Performance_Score,
                    x.Security_Score,
                    x.Code_Quality_Score
                })
                .FirstOrDefaultAsync();

            if (scan == null)
                return null;

            var bugs = await _context.Automated_Issues
                .Where(x => x.Scan_Id == scanId && x.Issue_Category == "Bug")
                .Select(x => new IssueDto
                {
                    Title = x.Issue_Title,
                    Description = x.Issue_Description,
                    Severity = x.Severity.ToString()
                })
                .ToListAsync();

            var codeQuality = await _context.Automated_Issues
                .Where(x => x.Scan_Id == scanId && x.Issue_Category == "CodeQuality")
                .Select(x => new IssueDto
                {
                    Title = x.Issue_Title,
                    Description = x.Issue_Description,
                    Severity = x.Severity.ToString()
                })
                .ToListAsync();

            var performance = await _context.Performance_Metrics
                .Where(x => x.Scan_Id == scanId)
                .Select(x => new PerformanceMetricDto
                {
                    Page_Url = x.Page_Url,
                    Load_Time_MS = x.Load_Time_MS,
                    Page_Size_KB = x.Page_Size_KB
                })
                .ToListAsync();

            var security = await _context.Security_Headers
                .Where(x => x.Scan_Id == scanId)
                .Select(x => new SecurityHeaderDto
                {
                    Header_Name = x.Header_Name,
                    Status = x.Status
                })
                .ToListAsync();

            return new ScanResultDto
            {
                Scan_Id = scan.Id,
                Website_Url = scan.Website_Url,
                Performance_Score = scan.Performance_Score ?? 0,
                Security_Score = scan.Security_Score ?? 0,
                Code_Quality_Score = scan.Code_Quality_Score ?? 0,
                Bugs = bugs,
                CodeQuality = codeQuality,
                Performance = performance,
                Security = security
            };
        }
    }
}
