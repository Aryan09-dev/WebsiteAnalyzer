using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Domain.Enums;

namespace WebsiteAnalyzer.Application.Interfaces
{
    public interface IWebsiteScanRepository
    {
        Task<int> CreateScanAsync(
            int userId,
            string websiteUrl,
            string scanType);
        Task<List<ScanListDto>> GetUserScansAsync(int userId);
    }
}
