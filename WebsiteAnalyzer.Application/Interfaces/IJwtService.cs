using WebsiteAnalyzer.Domain.Entities;

namespace WebsiteAnalyzer.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
