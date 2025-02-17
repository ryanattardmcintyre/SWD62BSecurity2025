using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Permission
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int Id { get; set; }

        public string User { get; set; }

        [ForeignKey("Article")]
        public Guid ArticleFk { get; set; }
        public virtual Article Article { get; set; }

        public bool HasAccess { get; set; }


    }
}
