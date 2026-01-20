using Microsoft.AspNetCore.Mvc;
using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Application.Interfaces;

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

        // POST: api/scan/start
        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeWebsite([FromBody] StartScanDto dto)
        {
            var result = await _scanRepository.AnalyzeAsync(dto);

            return Ok(result);
        }
    }
}
