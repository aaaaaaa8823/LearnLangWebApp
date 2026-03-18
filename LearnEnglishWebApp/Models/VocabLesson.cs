using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class VocabLesson
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Level { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContentKey { get; set; } // например: "my-daily-routine"

        // Слова для изучения (можно хранить список ID словарных слов)
        [Column(TypeName = "jsonb")]
        public List<long> VocabularyWordIds { get; set; } // ID слов из Dictionary

        public int OrderIndex { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }


        public ICollection<VocabTest> VocabTests { get; set; } = new List<VocabTest>();
        public ICollection<UserVocabProgress> UserProgress { get; set; } = new List<UserVocabProgress>();
    }
}