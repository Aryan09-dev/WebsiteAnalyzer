namespace WebsiteAnalyzer.Domain.Entities
{
    public class ManualBug
    {
        public int Id { get; set; }

        public int Scan_Id { get; set; }
        public string Page_Url { get; set; } = null!;
        public string Bug_Title { get; set; } = null!;
        public string? Bug_Description { get; set; }
        public string Severity { get; set; } = null!;
        public string? Screenshot_Path { get; set; }
        public int Reported_By { get; set; }

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
