using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class DBAsymmetricKeys
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PublicKey { get; set; } //is to encrypt
        public string PrivateKey { get; set; } //is to decrypt
    }
}
