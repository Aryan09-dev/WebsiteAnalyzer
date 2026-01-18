namespace WebsiteAnalyzer.Domain.Entities
{
    public class ScanPage
    {
        public int Id { get; set; }

        public int Scan_Id { get; set; }
        public string Page_Url { get; set; } = null!;
        public int? Http_Status_Code { get; set; }
        public int? Page_Load_Time_MS { get; set; }

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
