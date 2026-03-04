using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class UserGrammarProgress
    {
        public long UserId {get; set;}
        public long GrammarTopicId {get; set;}
        public bool Completed { get; set;} = false;

        public DateTime? CompletedAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("GrammarTopicId")]
        public GrammarTopic GrammarTopic { get; set; }

    }
}
