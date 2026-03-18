using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LearnEnglishWebApp.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(225)]
        public string PasswordHash { get; set; }

        [MaxLength(10)]
        public string Level { get; set; } = "A1";

        public DateTime CreatedAt { get; set; }

        public ICollection<UserWord> UserWords { get; set; } = new List<UserWord>();
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();

        public ICollection<UserGrammarProgress> GrammarProgress { get; set; } = new List<UserGrammarProgress>();

        public ICollection<UserVocabProgress> VocabProgress { get; set; } = new List<UserVocabProgress>();

        public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();


    }
    
}
