using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class UserTest
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }

        public long TestId { get; set; }

        [MaxLength(20)]
        public string TestType { get; set; } = "grammar";

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
