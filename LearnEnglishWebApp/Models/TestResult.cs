using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class TestResult
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TestType { get; set; } 

        [Required]
        public long TestId { get; set; } 
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public double Percentage { get; set; }
        public bool IsPassed { get; set; }
        public int AttemptNumber { get; set; } = 1;
        public int TimeSpentSeconds { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; }
    }

    public class TestAnswerDetail
    {
        public int QuestionId { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public int MaxPoints { get; set; }
    }
}