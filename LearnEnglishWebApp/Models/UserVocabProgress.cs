using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class UserVocabProgress
    {
        public long UserId { get; set; }
        public long LessonId { get; set; }

        public bool Completed { get; set; } = false;

        public int? Score { get; set; }

        public DateTime? CompletedAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("LessonId")]
        public VocabLesson VocabLesson { get; set; }
    }
}
