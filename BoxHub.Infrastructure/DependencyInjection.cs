using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Repositories.Pricings;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Application.Interfaces.Services.Pricings;
using BoxHub.Application.Services;
using BoxHub.Application.Services.Pricings;
using BoxHub.Infrastructure.Auth;
using BoxHub.Infrastructure.BackgroundJobs;
using BoxHub.Infrastructure.Repositories;
using BoxHub.Infrastructure.Repositories.Pricings;
using BoxHub.Infrastructure.Services;
using BoxHub.Shared.Helpers.Photos;
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
            services.AddScoped<IBoxTypePriceLimitRepository, BoxTypePriceLimitRepository>();
            services.AddScoped<ISystemPriceRuleRepository, SystemPriceRuleRepository>();

            //SERVICE

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IHostRegistrationService, HostRegistrationService>();
            services.AddScoped<ICreateHostAccountWorker, CreateHostAccountWorker>();
            services.AddScoped <IFacilityService, FacilityService>();
            services.AddScoped<IBoxTypePriceLimitService, BoxTypePriceLimitService>();
            services.AddScoped<ISystemPriceRuleService, SystemPriceRuleService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddSingleton<IHostRegistrationJobClient, HostRegistrationJobClient>();
            
            services.AddTransient<CreateHostAccountJob>();

            

            // Phải khớp key trong appsettings.json (đang là "CloudSettings", không phải "Cloudinary").
            services.Configure<CloudSettings>(config.GetSection("CloudSettings"));
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddHttpClient<IEmailService, EmailService>();

            return services;
        }
    }
}
