using WebsiteAnalyzer.Domain.Enums;

namespace WebsiteAnalyzer.Application.DTOs
{
    public class AnalyzeScanResponseDto
    {
        public int ScanId { get; set; }
        public ScanStatus ScanStatus { get; set; }

        public int Performance_Score { get; set; }
        public int Security_Score { get; set; }
        public int Code_Quality_Score { get; set; }
        public string Website_Url { get; set; }
    }
}
