using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Infrastructure.Data;

namespace WebsiteAnalyzer.Application.Services
{
    public class ScanAnalyzeService
    {
        private readonly IWebsiteScanRepository _scanRepo;
        private readonly IScanProcessingService _processingService;
        private readonly ApplicationDbContext _context;

        public ScanAnalyzeService(
            IWebsiteScanRepository scanRepo,
            IScanProcessingService processingService,
            ApplicationDbContext context)
        {
            _scanRepo = scanRepo;
            _processingService = processingService;
            _context = context;
        }

        public async Task<AnalyzeScanResponseDto> AnalyzeAsync(
            int userId,
            CreateScanRequestDto request)
        {
            // 1️⃣ Create scan
            var scanId = await _scanRepo.CreateScanAsync(
                userId,
                request.Website_Url,
                request.Scan_Type
            );

            // 2️⃣ Process scan (REAL DATA)
            await _processingService.ProcessScanAsync(scanId);

            // 3️⃣ Reload updated scan
            var updatedScan = await _context.Website_Scans.FindAsync(scanId);

            return new AnalyzeScanResponseDto
            {
                ScanId = updatedScan.Id,
                Website_Url = updatedScan.Website_Url,
                ScanStatus = updatedScan.Scan_Status,
                Performance_Score = updatedScan.Performance_Score ?? 0,
                Security_Score = updatedScan.Security_Score ?? 0,
                Code_Quality_Score = updatedScan.Code_Quality_Score ?? 0
            };
        }
    }
}
