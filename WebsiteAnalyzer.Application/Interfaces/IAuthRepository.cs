using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Domain.Entities;

namespace WebsiteAnalyzer.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> RegisterAsync(RegisterDto dto);
        Task<User> LoginAsync(LoginDto dto);
    }
}
