using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Application.Services;
using WebsiteAnalyzer.Domain.Enums;
using WebsiteAnalyzer.Infrastructure.Services;

namespace WebsiteAnalyzer.API.Controllers
{
    [ApiController]
    [Route("api/scan")]
    public class WebsiteScanController : ControllerBase
    {
        private readonly IWebsiteScanRepository _scanRepository;
        public WebsiteScanController(IWebsiteScanRepository scanRepository)
        {
            _scanRepository = scanRepository;
        }

        [HttpPost("analyze")]
        [Authorize]
        public async Task<IActionResult> AnalyzeWebsite(
            CreateScanRequestDto request,
            [FromServices] ScanAnalyzeService analyzeService)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await analyzeService.AnalyzeAsync(userId, request);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserScans()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId not found");

            int userId = int.Parse(userIdClaim.Value);

            var scans = await _scanRepository.GetUserScansAsync(userId);
            return Ok(scans);
        }

        [HttpGet("{scanId}/results")]
        public async Task<IActionResult> GetResults(int scanId)
        {
            var result = await _scanRepository.GetScanResultsAsync(scanId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{scanId}/download")]
        public async Task<IActionResult> DownloadReport(
            int scanId,
            [FromServices] PdfService pdfService)
        {
            var result = await _scanRepository.GetScanResultsAsync(scanId);

            if (result == null)
                return NotFound();

            var pdf = pdfService.GenerateReport(result);

            return File(
                pdf,
                "application/pdf",
                $"ScanReport_{scanId}.pdf"
            );
        }
    }
}
