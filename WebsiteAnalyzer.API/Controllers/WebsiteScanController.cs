using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Application.Services;
using WebsiteAnalyzer.Domain.Enums;

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
        public async Task<IActionResult> GetUserScans()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId not found");

            int userId = int.Parse(userIdClaim.Value);

            var scans = await _scanRepository.GetUserScansAsync(userId);
            return Ok(scans);
        }
    }
}
