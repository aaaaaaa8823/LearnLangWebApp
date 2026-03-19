using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnEnglishWebApp.Models
{
    public class UserWord
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }
        public long WordId {  get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "learning";

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public string ContextSentence { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("WordId")]
        public Dictionary Word { get; set; }

        public ICollection<CollectionWord> CollectionWords { get; set; } = new List<CollectionWord>();

    }
}
