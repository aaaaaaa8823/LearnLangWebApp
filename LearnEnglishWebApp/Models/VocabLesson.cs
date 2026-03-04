using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class VocabLesson
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(10)]
        public string Level { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Content { get; set; }

        //[Column(TypeName = "jsonb")]
        
        //TODO: Написать модель для вокаб тестов и связывать с этой моделью
        //public List<VocabQuestion> Questions { get; set; } // тест

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserVocabProgress> UserProgress { get; set; } = new List<UserVocabProgress>();

        public class VocabQuestion
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public List<string> Options { get; set; }
            public int CorrectAnswerIndex { get; set; }
        }

    }
}
