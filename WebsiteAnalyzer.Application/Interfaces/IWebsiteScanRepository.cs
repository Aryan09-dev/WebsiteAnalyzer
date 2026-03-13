using WebsiteAnalyzer.Application.DTOs;

namespace WebsiteAnalyzer.Application.Interfaces
{
    public interface IWebsiteScanRepository
    {
        Task<int> CreateScanAsync(
            int userId,
            string websiteUrl,
            string scanType);
        Task<List<ScanListDto>> GetUserScansAsync(int userId);
        Task<ScanResultDto> GetScanResultsAsync(int scanId);
    }
}
