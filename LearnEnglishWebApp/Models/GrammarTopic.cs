using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class GrammarTopic
    {
        //TODO: Написать модель для грамматическиз тестов и связать
        [Key]
        public long Id { get; set; }

        [MaxLength(10)]
        public string Level {  get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string? Content { get; set; } //сюда html страницуу наверное?

        [Column(TypeName = "jsonb")]
        public List<string> Examples { get; set; }

        public int OrderIndex { get; set; } //для контроля порядка отображения грамматических тем

        public ICollection<UserGrammarProgress> UserGrammarProgresses { get; set; } = new List<UserGrammarProgress>();

    }
}
