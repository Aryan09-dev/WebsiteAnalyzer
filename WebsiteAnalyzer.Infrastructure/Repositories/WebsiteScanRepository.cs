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
                    Scan_Status = x.Scan_Status,
                    Performance_Score = x.Performance_Score,
                    Security_Score = x.Security_Score,
                    Code_Quality_Score = x.Code_Quality_Score,
                    Created_On = x.Created_On
                })
                .ToListAsync();
        }
    }
}
