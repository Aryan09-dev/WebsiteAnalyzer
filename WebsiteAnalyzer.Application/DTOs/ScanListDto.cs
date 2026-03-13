using WebsiteAnalyzer.Domain.Enums;

namespace WebsiteAnalyzer.Application.DTOs
{
    public class ScanListDto
    {
        public int Scan_Id { get; set; }
        public string Website_Url { get; set; }
        public string Scan_Status { get; set; }
        public int? Performance_Score { get; set; }
        public int? Security_Score { get; set; }
        public int? Code_Quality_Score { get; set; }

        public int BugsFound { get; set; }
        public int SecurityIssues { get; set; }

        public DateTime Created_On { get; set; }
    }
}
