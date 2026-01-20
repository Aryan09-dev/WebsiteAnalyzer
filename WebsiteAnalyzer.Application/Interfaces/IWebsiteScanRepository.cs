using WebsiteAnalyzer.Application.DTOs;

namespace WebsiteAnalyzer.Application.Interfaces
{
    public interface IWebsiteScanRepository
    {
        Task<object> AnalyzeAsync(StartScanDto dto);
    }
}
