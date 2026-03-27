using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Application.Services;
using BoxHub.Infrastructure.Auth;
using BoxHub.Infrastructure.BackgroundJobs;
using BoxHub.Infrastructure.Repositories;
using BoxHub.Infrastructure.Services;
using BoxHub.Shared.Helpers.Photos;
using BoxHub.Shared.Helpers;
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
            //REPO
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHostRegistrationRepository, HostRegistrationRepository>();
            services.AddScoped<IFacilityRepository, FacilityRepository>();
            services.AddScoped<IAmenityRepository, AmenityRepository>();

            //SERVICE

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IHostRegistrationService, HostRegistrationService>();
            services.AddScoped<ICreateHostAccountWorker, CreateHostAccountWorker>();
            services.AddScoped <IFacilityService, FacilityService>();
            services.AddScoped<IAmenityService, AmenityService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IHostRegistrationJobClient, HostRegistrationJobClient>();
            
            services.AddTransient<CreateHostAccountJob>();

            

            services.Configure<CloudSettings>(config.GetSection("CloudSettings"));
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.Configure<MailSettings>(config.GetSection("MailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
