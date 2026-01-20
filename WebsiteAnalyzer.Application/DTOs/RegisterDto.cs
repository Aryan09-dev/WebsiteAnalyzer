namespace WebsiteAnalyzer.Application.DTOs
{
    public class RegisterDto
    {
        public string Full_Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Role_Id { get; set; }
    }
}
