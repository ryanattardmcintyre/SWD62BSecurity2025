using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.DataAccess;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication()
                .AddMicrosoftAccount(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
                    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
                });


            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // User locked out for 15 mins
                options.Lockout.MaxFailedAccessAttempts = 3; // Lock after 5 failed attempts
                options.Lockout.AllowedForNewUsers = true; // Apply lockout for new users

                options.Password.RequireDigit = true; // Require at least one digit
                options.Password.RequiredLength = 6; // Minimum length of 8 characters
                options.Password.RequireNonAlphanumeric = true; // Require at least one special character
                options.Password.RequireUppercase = true; // Require at least one uppercase letter
                options.Password.RequireLowercase = true; // Require at least one lowercase letter
            });


            builder.Services.AddScoped<ArtifactRepository>();
            builder.Services.AddScoped<ArticleRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
