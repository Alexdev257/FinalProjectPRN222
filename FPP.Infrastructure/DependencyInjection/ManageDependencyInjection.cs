using FPP.Application.Interface.IHelpers;
using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
using FPP.Infrastructure.Implements.Helpers;
using FPP.Infrastructure.Implements.Repositories;
using FPP.Infrastructure.Implements.Services;
using FPP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Infrastructure.DependencyInjection
{
    public static class ManageDependencyInjection
    {
        public static void AddDatabase(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddScopeInterface(this IServiceCollection service)
        {
            // Add scoped services here
            service.AddScoped<IRedisHelper, RedisHelper>();
            service.AddScoped<IOtpHelper, OtpHelper>();
            service.AddScoped<IBcryptHelper, BCryptHelper>();
            service.AddScoped<IEmailHelper, EmailHelper>();
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<ILabService, LabService>();
            service.AddScoped<INotificationService, NotificationService>();
            service.AddScoped<IEventParticipantService, EventParticipantService>();
            service.AddScoped<ILabEventService, LabEventService>();
            service.AddScoped<IActivityTypeService, ActivityTypeService>();
            service.AddScoped<ISecurityLogService, SecurityLogService>();
            service.AddScoped<IUserService, UserService>();
        }

        public static void AddRedisConfiguration(this IServiceCollection service, IConfiguration configuration)
        {

            //var redisConnection = configuration.GetConnectionString("Redis");
            //var options = ConfigurationOptions.Parse(redisConnection);
            //options.Ssl = false;
            //options.AbortOnConnectFail = false;
            //options.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            //service.AddSingleton<IConnectionMultiplexer>(sp =>
            //    ConnectionMultiplexer.Connect(options));

            var redisConnection = configuration.GetConnectionString("UptashRedis");
            service.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(redisConnection));
        }
    }
}
