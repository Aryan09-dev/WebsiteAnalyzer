namespace WebsiteAnalyzer.Domain.Entities
{
    public class PerformanceMetric
    {
        public int Id { get; set; }

        public int Scan_Id { get; set; }
        public string? Page_Url { get; set; }
        public int? Load_Time_MS { get; set; }
        public int? Page_Size_KB { get; set; }
        public int? Total_Requests { get; set; }

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
