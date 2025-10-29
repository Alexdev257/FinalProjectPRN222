using FPP.Domain.Entities;
using FPP.Infrastructure.DependencyInjection;
using FPP.Infrastructure.Persistence;
using FPP.Presentation.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace FPP.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDatabase(builder.Configuration);
            builder.Services.AddScopeInterface();
            builder.Services.AddRedisConfiguration(builder.Configuration);
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) 
                .AddCookie(options => 
                {
                    options.Cookie.Name = "FppAuthCookie"; 
                    options.LoginPath = "/Login";        
                    options.AccessDeniedPath = "/AccessDenied"; 
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
                    options.SlidingExpiration = true; 
                });

            builder.Services.AddSignalR();

            builder.Services.AddAuthorization();

            builder.Services.AddRazorPages();

            var app = builder.Build();
            await using (var scope = app.Services.CreateAsyncScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var db = services.GetRequiredService<ApplicationDbContext>();

                    var conn = db.Database.GetConnectionString();
                    Console.WriteLine($"?? Connection string: {conn}");
                    logger.LogInformation("?? Connection string retrieved.");

                    // Dùng await
                    var pending = await db.Database.GetPendingMigrationsAsync();
                    var pendingList = pending.ToList();
                    Console.WriteLine($"?? Pending migrations: {pendingList.Count}");
                    logger.LogInformation("?? Pending migrations count: {Count}", pendingList.Count);

                    if (pendingList.Any())
                    {
                        Console.WriteLine("?? Running database migrations...");
                        logger.LogInformation("?? Running database migrations...");
                        // Dùng await
                        await db.Database.MigrateAsync();
                        Console.WriteLine("? Migration completed.");
                        logger.LogInformation("? Migration completed.");
                    }
                    else
                    {
                        Console.WriteLine("? No pending migrations.");
                        logger.LogInformation("? No pending migrations.");
                    }

                    // --- B?T ??U SEED DATA ---
                    Console.WriteLine("?? Checking database for seed data...");
                    logger.LogInformation("?? Checking database for seed data...");

                    // Dùng await
                    if (!await db.Users.AnyAsync() && !await db.Labs.AnyAsync() && !await db.ActivityTypes.AnyAsync())
                    {
                        Console.WriteLine("?? Seeding initial data...");
                        logger.LogInformation("?? Seeding initial data...");

                        // 1. Seed Activity Types
                        var activityTypes = new List<ActivityType>
                        {
                            new ActivityType { Name = "Class Lecture", Description = "Scheduled class session" },
                            new ActivityType { Name = "Workshop", Description = "Hands-on training session" },
                            new ActivityType { Name = "Personal Project", Description = "Individual student project work" },
                            new ActivityType { Name = "Team Meeting", Description = "Collaborative group meeting" },
                            new ActivityType { Name = "Research", Description = "Faculty or student research activities" }
                        };
                        db.ActivityTypes.AddRange(activityTypes);
                        await db.SaveChangesAsync(); // Dùng await
                        logger.LogInformation("? Activity Types seeded.");

                        // 2. Seed Users (NH? HASH M?T KH?U TH?C T?)
                        var users = new List<User>
                        {
                            new User { Name = "Alice Student", Email = "student@fpt.edu.vn", PasswordHash = "$2a$11$mpVvDZKE.t6WdFVlT3C/suguGEehj1u/VG3erzBehJubKEC/bzNoi", Role = 1, CreatedAt = DateTime.UtcNow }, //Thang2507@
                            new User { Name = "Bob Manager", Email = "manager@fpt.edu.vn", PasswordHash = "$2a$11$mpVvDZKE.t6WdFVlT3C/suguGEehj1u/VG3erzBehJubKEC/bzNoi", Role = 2, CreatedAt = DateTime.UtcNow }, //Thang2507@
                            new User { Name = "Charlie Security", Email = "security@fpt.edu.vn", PasswordHash = "$2a$11$mpVvDZKE.t6WdFVlT3C/suguGEehj1u/VG3erzBehJubKEC/bzNoi", Role = 3, CreatedAt = DateTime.UtcNow } //Thang2507@
                        };
                        db.Users.AddRange(users);
                        await db.SaveChangesAsync(); // Dùng await
                        logger.LogInformation("? Users seeded.");

                        // Get Manager ID (Dùng await)
                        var managerUser = await db.Users.FirstOrDefaultAsync(u => u.Role == 2);
                        if (managerUser == null)
                        {
                            logger.LogError("!! Manager user not found after seeding. Aborting further seeding.");
                            return; // D?ng l?i
                        }

                        // 3. Seed Labs (Dùng await)
                        var labs = new List<Lab>
                        {
                            new Lab { Name = "AI & Machine Learning Lab", Location = "Room 301, Alpha Building", Description = "Equipped for AI research and development.", ManagerId = managerUser.UserId },
                            new Lab { Name = "IoT & Robotics Lab", Location = "Room 302, Alpha Building", Description = "Hardware and software for IoT projects.", ManagerId = managerUser.UserId },
                            new Lab { Name = "Network Security Lab", Location = "Room 405, Beta Building", Description = "Isolated network environment for security testing.", ManagerId = managerUser.UserId },
                            new Lab { Name = "Software Development Hub", Location = "Room 408, Beta Building", Description = "Collaboration space for software projects.", ManagerId = managerUser.UserId },
                            new Lab { Name = "Cloud Computing Center", Location = "Room 510, Gamma Building", Description = "Access to various cloud platforms.", ManagerId = managerUser.UserId },
                            new Lab { Name = "Data Science & Analytics Lab", Location = "Room 512, Gamma Building", Description = "High-performance computing for data analysis.", ManagerId = managerUser.UserId }
                        };
                        db.Labs.AddRange(labs);
                        await db.SaveChangesAsync(); // Dùng await
                        logger.LogInformation("? Labs seeded.");

                        // 4. Seed Lab Zones (Dùng await)
                        var zones = new List<LabZone>();
                        foreach (var lab in labs) // labs gi? ?ã có ID
                        {
                            zones.Add(new LabZone { LabId = lab.LabId, Name = "Main Area", Description = $"Primary working area of {lab.Name}" });
                            if (lab.Name.Contains("IoT"))
                            {
                                zones.Add(new LabZone { LabId = lab.LabId, Name = "Soldering Station", Description = "Area for hardware assembly" });
                            }
                        }
                        db.LabZones.AddRange(zones);
                        await db.SaveChangesAsync(); // Dùng await
                        logger.LogInformation("? Lab Zones seeded.");

                        // --- OPTIONAL: Seed Events/Bookings (Dùng await) ---
                        var studentUser = await db.Users.FirstOrDefaultAsync(u => u.Role == 1);
                        var aiLab = await db.Labs.FirstOrDefaultAsync(l => l.Name.Contains("AI"));
                        var aiLabMainZone = aiLab != null ? await db.LabZones.FirstOrDefaultAsync(z => z.LabId == aiLab.LabId && z.Name == "Main Area") : null;
                        var classActivity = await db.ActivityTypes.FirstOrDefaultAsync(at => at.Name == "Class Lecture");
                        var projectActivity = await db.ActivityTypes.FirstOrDefaultAsync(at => at.Name == "Personal Project");

                        if (studentUser != null && aiLab != null && aiLabMainZone != null && classActivity != null && projectActivity != null && managerUser != null)
                        {
                            var events = new List<LabEvent> {
                                new LabEvent {
                                    LabId = aiLab.LabId, ZoneId = aiLabMainZone.ZoneId, ActivityTypeId = classActivity.ActivityTypeId, OrganizerId = managerUser.UserId,
                                    Title = "Ongoing AI Lecture", Description = "Current lecture session",
                                    StartTime = DateTime.Now.AddHours(-1), EndTime = DateTime.Now.AddHours(1), Status = "Approved", CreatedAt = DateTime.UtcNow
                                },
                                new LabEvent {
                                    LabId = aiLab.LabId, ZoneId = aiLabMainZone.ZoneId, ActivityTypeId = projectActivity.ActivityTypeId, OrganizerId = studentUser.UserId,
                                    Title = "Student Project Work", Description = "Working on final project",
                                    StartTime = DateTime.Now.AddDays(1).Date.AddHours(14), EndTime = DateTime.Now.AddDays(1).Date.AddHours(16), Status = "Approved", CreatedAt = DateTime.UtcNow
                                }
                            };
                            db.LabEvents.AddRange(events);
                            await db.SaveChangesAsync(); // Dùng await

                            var upcomingEvent = await db.LabEvents.FirstOrDefaultAsync(e => e.OrganizerId == studentUser.UserId && e.StartTime > DateTime.Now);
                            if (upcomingEvent != null)
                            {
                                bool participantExists = await db.EventParticipants.AnyAsync(ep => ep.EventId == upcomingEvent.EventId && ep.UserId == studentUser.UserId);
                                if (!participantExists)
                                {
                                    db.EventParticipants.Add(new EventParticipant { EventId = upcomingEvent.EventId, UserId = studentUser.UserId, Role = 1 });
                                    await db.SaveChangesAsync(); // Dùng await
                                    logger.LogInformation("? Sample events and participants seeded.");
                                }
                                else
                                {
                                    logger.LogInformation("? Sample participant already exists.");
                                }
                            }
                            else
                            {
                                logger.LogWarning("?? Could not find upcoming event for student to add participant.");
                            }
                        }
                        else
                        {
                            logger.LogWarning("?? Could not seed sample events due to missing related data.");
                        }

                        Console.WriteLine("? Initial data seeding completed.");
                        logger.LogInformation("? Initial data seeding completed.");
                    }
                    else
                    {
                        Console.WriteLine("? Database already contains data. Skipping seed.");
                        logger.LogInformation("? Database already contains data. Skipping seed.");
                    }
                    // --- K?T THÚC SEED DATA ---
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "!! An error occurred during database migration or seeding.");
                    // Có th? ném l?i l?i ho?c x? lý khác tùy thu?c vào yêu c?u
                    // throw;
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<NotificationBookingHub>("/notificationBookingHub");

            app.Run();
        }
    }
}
