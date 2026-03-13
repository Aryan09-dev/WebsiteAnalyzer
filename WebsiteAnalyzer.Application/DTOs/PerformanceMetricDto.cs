namespace WebsiteAnalyzer.Application.DTOs
{
    public class PerformanceMetricDto
    {
        public string Page_Url { get; set; }
        public int? Load_Time_MS { get; set; }
        public int? Page_Size_KB { get; set; }
    }
}
