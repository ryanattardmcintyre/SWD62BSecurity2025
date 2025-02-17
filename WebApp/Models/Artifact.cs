using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Artifact
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        public string FilePath { get; set; }

        [ForeignKey("Article")]
        public Guid ArticleIdFK { get; set; }

        public virtual Article Article { get; set; }

        //its a measure how can we detect that the article was tampered with
        public string Digest { get; set; }

    }
}
