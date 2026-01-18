namespace WebsiteAnalyzer.Domain.Entities
{
    public class AutomatedIssue
    {
        public int Id { get; set; }

        public int Scan_Id { get; set; }
        public int? Page_Id { get; set; }

        public string Issue_Category { get; set; } = null!;
        public string Issue_Title { get; set; } = null!;
        public string? Issue_Description { get; set; }
        public string Severity { get; set; } = null!;
        public string? AI_Explanation { get; set; }
        public string? Suggested_Fix { get; set; }

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
