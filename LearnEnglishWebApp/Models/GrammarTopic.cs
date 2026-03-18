using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class GrammarTopic
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(10)]
        [Required]
        public string Level { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContentKey { get; set; } // например: "present-simple-theory"

        public int OrderIndex { get; set; }

        //Нужно чтобы отображать видимость темы для пользователя
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<GrammarTest> GrammarTests { get; set; } = new List<GrammarTest>();
        public ICollection<UserGrammarProgress> UserGrammarProgresses { get; set; } = new List<UserGrammarProgress>();
    }
}