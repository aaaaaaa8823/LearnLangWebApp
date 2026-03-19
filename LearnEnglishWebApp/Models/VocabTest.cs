using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class VocabTest
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long VocabLessonId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(10)]
        public string Level { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        // Тип теста
        [MaxLength(20)]
        public string TestType { get; set; } = "comprehension"; 
        public int QuestionCount { get; set; }

        [Range(0, 100)]
        public int PassingScore { get; set; } = 50;

        public int TimeLimitMinutes { get; set; } = 0;

        public int OrderIndex { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("VocabLessonId")]
        public VocabLesson VocabLesson { get; set; }
    }
}