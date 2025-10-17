using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
using FPP.Infrastructure.Implements.Repositories;
using FPP.Infrastructure.Implements.Services;
using FPP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IUserService, UserService>();
        }
    }
}
