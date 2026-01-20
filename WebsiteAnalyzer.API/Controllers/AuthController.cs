using Microsoft.AspNetCore.Mvc;
using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Application.Interfaces;

namespace WebsiteAnalyzer.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        // ---------------------------
        // REGISTER
        // ---------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authRepository.RegisterAsync(dto);

            if (result == null)
                return BadRequest("User already exists.");

            return Ok(new
            {
                Message = "User registered successfully",
                result.Id,
                result.Email
            });
        }

        // ---------------------------
        // LOGIN
        // ---------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _authRepository.LoginAsync(dto);

            if (user == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                Message = "Login successful",
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role_Id
            });
        }
    }
}
