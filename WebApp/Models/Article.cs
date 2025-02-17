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

        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool PublicAccess { get; set; }
    }
}
