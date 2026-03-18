using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Application.Services;
using BoxHub.Infrastructure.Auth;
using BoxHub.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure( this IServiceCollection services,
            IConfiguration config)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
