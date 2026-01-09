namespace WebsiteAnalyzer.Domain.Entities
{
    public class SecurityHeader
    {
        public int Id { get; set; }

        public int Scan_Id { get; set; }
        public string Header_Name { get; set; } = null!;
        public string? Header_Value { get; set; }
        public string Status { get; set; } = null!;
        public string Risk_Level { get; set; } = null!;

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
