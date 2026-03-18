    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace LearnEnglishWebApp.Models
    {
        public class CollectionWord
        {
            public long CollectionId { get; set; }

            public long UserWordId {  get; set; }

            public DateTime AddedAt { get; set; }

            [ForeignKey("CollectionId")]
            public Collection Collection { get; set; }

            [ForeignKey("UserWordId")]
            public UserWord UserWord { get; set; }

        }
    }
