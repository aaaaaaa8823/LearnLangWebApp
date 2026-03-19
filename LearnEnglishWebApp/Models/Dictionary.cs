using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class Dictionary
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Word { get; set; }

        [Required]
        [MaxLength(100)]
        public string Translation { get; set; }

        public string? Definition { get; set; }

        [MaxLength(30)]
        public string PartOfSpeech { get; set; }

        [MaxLength(10)]
        public string DifficultyLevel { get; set; }

        [Column(TypeName = "jsonb")]
        public List<string> Examples { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserWord> UserWords { get; set; } = new List<UserWord>();



    }
}
