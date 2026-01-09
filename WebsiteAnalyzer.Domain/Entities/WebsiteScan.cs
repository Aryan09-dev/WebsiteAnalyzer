namespace WebsiteAnalyzer.Domain.Entities
{
    public class WebsiteScan
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public string Website_Url { get; set; } = null!;
        public string Scan_Type { get; set; } = null!;
        public string Scan_Status { get; set; } = null!;

        public int? Performance_Score { get; set; }
        public int? Security_Score { get; set; }
        public int? Code_Quality_Score { get; set; }

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
