namespace WebsiteAnalyzer.Application.Interfaces
{
    public interface IScanProcessingService
    {
        Task ProcessScanAsync(int scanId);
    }
}
