namespace LearnEnglishWebApp.DTOs.Response
{
    public class TestResultDto
    {
        public long Id { get; set; }
        public long TestId { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public double Percentage { get; set; }
        public bool IsPassed { get; set; }
        public int AttemptNumber { get; set; }
        public int TimeSpentSeconds { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
