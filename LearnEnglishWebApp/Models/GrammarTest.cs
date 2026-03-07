using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class GrammarTest
    {
        [Key]
        public long Id { get; set; }

        public  long GrammarTopicId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set;  }

        public string Level { get; set; }

        public int QuestionCount { get; set; }

        [Required]
        [MaxLength(100)]
        public string TestKey { get; set; } // например: "present-simple-test-1"

        [Range(0, 100)]
        public int PassingScore { get; set; } = 50;
        public int OrderIndex { get; set; }

        public int TimeLimitMinutes { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("GrammarTopicId")]
        public GrammarTopic GrammarTopic { get; set; }

    }
}
