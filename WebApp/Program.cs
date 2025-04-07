using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Security.Cryptography;
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

            //builder.Services.AddDefaultIdentity<IdentityUser >(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>() // Enables role management
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
            builder.Services.AddScoped<WebApp.Utilities.EncryptionUtility>();
            builder.Services.AddScoped<KeysRepository>();

            var app = builder.Build();
           
            using (var scope = app.Services.CreateScope())
            {
                var encryptionUtility = scope.ServiceProvider.GetRequiredService<WebApp.Utilities.EncryptionUtility>();

                var myKeys = encryptionUtility.GenerateSymmetricKeys(Aes.Create());

                if(System.IO.File.Exists("mySymmetricKeys.txt") == false)
                    using (var myFile = System.IO.File.CreateText("mySymmetricKeys.txt"))
                    {
                        myFile.WriteLine(Convert.ToBase64String(myKeys.SecretKey));
                        myFile.WriteLine(Convert.ToBase64String(myKeys.IV));

                        myFile.Flush();
                    }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                //fallback plan:
                app.UseExceptionHandler("/Home/Error"); //<<<< make sure that Error (cshtml) but doesn't contain server-side code
                //with error pages be careful and its recommended that they are static pages not using some
                //layout page (which might contain dynamic code)
                //reason is: if an error occurred, so user is beign redirected to the error page but first layout is executed 
                //           to be rendered and there is another unforseen exception while layout is loading, what's going to
                //            happen then?? another uncaught failure? - end result will be disclosure of layout C# code


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
