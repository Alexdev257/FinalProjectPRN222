using FPP.Infrastructure.DependencyInjection;
using FPP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FPP.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDatabase(builder.Configuration);
            builder.Services.AddScopeInterface();
            builder.Services.AddRedisConfiguration(builder.Configuration);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var conn = db.Database.GetConnectionString();
                Console.WriteLine($"?? Connection string: {conn}");

                var pending = db.Database.GetPendingMigrations().ToList();
                Console.WriteLine($"?? Pending migrations: {pending.Count}");

                if (pending.Any())
                {
                    Console.WriteLine("?? Running database migrations...");
                    db.Database.Migrate();
                    Console.WriteLine("? Migration completed.");
                }
                else
                {
                    Console.WriteLine("? No pending migrations.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
