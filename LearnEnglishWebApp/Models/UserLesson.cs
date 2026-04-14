using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class UserLesson
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }

        public long LessonId { get; set; }

        [MaxLength(20)]
        public string LessonType { get; set; } = "grammar";

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
