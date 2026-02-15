namespace WebsiteAnalyzer.Application.DTOs
{
    public class CreateScanRequestDto
    {
        public string Website_Url { get; set; }
        public string Scan_Type { get; set; } = "Full";
    }
}
