using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data
{
    //inheriting from IdentityDbContext means, that when creating the database
    //it will automatically create tables which will manage the users for us
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {


        }
     /*   public ApplicationDbContext(string username)
       : base(GetOptions(username))
        {
        }
        private static DbContextOptions<ApplicationDbContext> GetOptions(string username)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            string connectionString = GetConnectionStringForUser(username);

            optionsBuilder.UseSqlServer(connectionString);

            return optionsBuilder.Options;
        }

        private static string GetConnectionStringForUser(string username)
        {
            if (username == "admin")
            {
                return "Server=LAPTOP-O0I9A16V\\SQLEXPRESS;Database=SWD62b2025Security;Trusted_Connection=True;MultipleActiveResultSets=true;trustservercertificate=true;";
            }
            else
            {
                return "Server=LAPTOP-O0I9A16V\\SQLEXPRESS;Database=SWD62b2025Security;User Id=leastprivilegeconnection;Password=123;MultipleActiveResultSets=true;trustservercertificate=true;";
            }
 
        }
     */

        public DbSet<Article> Articles { get; set; } //<<this is going to be the table name
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<Permission> Permissions { get; set; }
    }
}
