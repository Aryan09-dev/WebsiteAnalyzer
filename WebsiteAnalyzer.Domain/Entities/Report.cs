namespace WebsiteAnalyzer.Domain.Entities
{
    public class Report
    {
        public int Report_Id { get; set; }

        public int Scan_Id { get; set; }
        public string Report_Type { get; set; } = null!;
        public string File_Path { get; set; } = null!;

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
