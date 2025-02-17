using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(100)]
        [Required]
        public string Title { get; set; }
        public string Text { get; set; }

        [ForeignKey("User")]
        public string Author { get; set; }
       
        //navigational property will give us access to the IdentityUser (built-in class) additonal info
        public virtual IdentityUser User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
