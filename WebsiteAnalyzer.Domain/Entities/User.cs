namespace WebsiteAnalyzer.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Full_Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password_Hash { get; set; } = null!;
        public int Role_Id { get; set; }

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }

        public Role Role { get; set; }
    }
}
