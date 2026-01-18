namespace WebsiteAnalyzer.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }

        public string Role_Name { get; set; } = null!;
        public string? Role_Description { get; set; }

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
