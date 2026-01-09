namespace WebsiteAnalyzer.Domain.Entities
{
    public class AIRecommendation
    {
        public int Id { get; set; }

        public int Issue_Id { get; set; }
        public string Recommendation_Text { get; set; } = null!;
        public int? Confidence_Score { get; set; }

        public DateTime Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
