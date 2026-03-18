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

        //не уверена буду ли вставлять ссылки на произношение для каждого слова. Пока сложно но оставлю на будущее
        //[MaxLength(255)]
        //public string PronunciationUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<UserWord> UserWords { get; set; } = new List<UserWord>();



    }
}
