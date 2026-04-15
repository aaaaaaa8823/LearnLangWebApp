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

        public int OrderIndex { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
        public ICollection<VocabTest> VocabTests { get; set; } = new List<VocabTest>();
        public ICollection<UserVocabProgress> UserProgress { get; set; } = new List<UserVocabProgress>();
    }
}