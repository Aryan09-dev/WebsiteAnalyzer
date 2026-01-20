namespace WebsiteAnalyzer.Application.DTOs
{
    public class StartScanDto
    {
        public int User_Id { get; set; }
        public string Website_Url { get; set; } = null!;
    }
}
