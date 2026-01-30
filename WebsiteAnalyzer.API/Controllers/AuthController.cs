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
        private readonly IJwtService _jwtService;

        public AuthController(
            IAuthRepository authRepository,
            IJwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        // ---------------------------
        // REGISTER
        // ---------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authRepository.RegisterAsync(dto);

            if (user == null)
                return BadRequest("User already exists");

            return Ok(new
            {
                Message = "User registered successfully",
                UserId = user.Id,
                Email = user.Email
            });
        }

        // ---------------------------
        // LOGIN
        // ---------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authRepository.LoginAsync(dto);

            if (user == null)
                return Unauthorized("Invalid email or password");

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                Message = "Login successful",
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role_Id
            });
        }
    }
}
