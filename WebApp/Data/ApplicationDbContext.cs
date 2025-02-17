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

        public DbSet<Article> Articles { get; set; } //<<this is going to be the table name
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<Permission> Permissions { get; set; }
    }
}
