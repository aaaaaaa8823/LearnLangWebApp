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

        public string? TheoryContent { get; set; }  
        public string? ExamplesContent { get; set; }

        public int OrderIndex { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GrammarTest> GrammarTests { get; set; } = new List<GrammarTest>();
        public ICollection<UserGrammarProgress> UserGrammarProgresses { get; set; } = new List<UserGrammarProgress>();
    }
}