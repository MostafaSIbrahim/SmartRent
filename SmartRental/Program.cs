using ApartmentRental.Reporisitory.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartRental.Data;
using SmartRental.Models.Entites.Identity;
using SmartRental.Reporisitory;
using SmartRental.Repository;
//using SmartRental.Services;
using System.Text;

namespace SmartRental
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            #region Dabase Conncetion
            builder.Services.AddDbContext<SmartRentalContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );
            });

            // Identity Database
            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("IdentityConnection")
                );
            });
            #endregion
            #region  and Authentication
            builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";

                options.User.RequireUniqueEmail = true;
            })
                             .AddEntityFrameworkStores<AppIdentityDbContext>()
                             .AddDefaultTokenProviders();



            #endregion
           builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
           builder.Services.AddScoped<IApartmentRepository,ApartmentRepository>();
            var app = builder.Build();
            #region Update Database
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                var dbContext = services.GetRequiredService<SmartRentalContext>();
                await dbContext.Database.MigrateAsync();
                var dbContextIdentity = services.GetRequiredService<AppIdentityDbContext>();
                  await dbContextIdentity.Database.MigrateAsync();

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await AppIdentityDbContextSeed.SeedRolesAsync(roleManager);

                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger("Program");
                logger.LogError(ex, "An error occurred during applying the migration.");
            }
            #endregion
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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

          await  app.RunAsync();    
        }
    }
}
