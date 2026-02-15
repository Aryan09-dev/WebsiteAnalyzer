namespace WebsiteAnalyzer.Application.DTOs
{
    public class ScanSummaryDto
    {
        public int Scan_Id { get; set; }
        public string Website_Url { get; set; }
        public int Overall_Score { get; set; }
        public string Quality_Label { get; set; }
        public DateTime Scan_Date { get; set; }
    }
}
