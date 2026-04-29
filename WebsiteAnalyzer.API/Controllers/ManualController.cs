using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Domain.Entities;

namespace WebsiteAnalyzer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManualController : ControllerBase
    {
        private readonly IManualBugRepository _manualBugRepository;

        public ManualController(IManualBugRepository manualBugRepository)
        {
            _manualBugRepository = manualBugRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateManualBug([FromBody] CreateManualBugDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var bug = new ManualBug
            {
                Scan_Id = dto.Scan_Id,
                Page_Url = dto.Page_Url,
                Bug_Title = dto.Bug_Title,
                Bug_Description = dto.Bug_Description,
                Severity = dto.Severity,
                Screenshot_Path = dto.Screenshot_Path,
                Reported_By = userId
            };

            var result = await _manualBugRepository.CreateAsync(bug);

            return Ok(new
            {
                message = "Manual bug created successfully",
                data = result
            });
        }

        [HttpGet("get-by-user")]
        public async Task<IActionResult> GetMyBugs()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var bugs = await _manualBugRepository.GetByUserIdAsync(userId);

            return Ok(new
            {
                message = "Bugs fetched successfully",
                data = bugs
            });
        }
    }
}
