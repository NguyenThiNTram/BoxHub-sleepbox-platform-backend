using BoxHub.Application.Auth;
using BoxHub.Application.Common;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Infrastructure.Data;
using BoxHub.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql.NameTranslation;
using System.Text;

namespace BoxHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://*:{port}");

            var configuration = builder.Configuration;
            var environment = builder.Environment;

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // ===============================
            // Controllers & Swagger
            // ===============================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ===============================
            // Database (PostgreSQL - Supabase)
            // ===============================
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Create data source builder
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

            // Map Postgres enum
            dataSourceBuilder.MapEnum<UserRole>(
                "core.user_role",
                new NpgsqlNullNameTranslator()
            );

            var dataSource = dataSourceBuilder.Build();

            builder.Services.AddDbContext<BoxHubDbContext>(options =>
            {
                options.UseNpgsql(dataSource);

                if (environment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }
            });

            // Mapping interface -> implementation
            builder.Services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<BoxHubDbContext>());

            // ===============================
            // JWT Authentication
            // ===============================
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("JWT Key is not configured");

            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

            if (keyBytes.Length < 32)
                throw new InvalidOperationException("JWT Key must be at least 256 bits (32 characters)");

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSection["Issuer"],
                        ValidAudience = jwtSection["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                        ClockSkew = TimeSpan.Zero
                    };
                });

            // ===============================
            // Authorization (Policy-ready)
            // ===============================
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly",
                    policy => policy.RequireRole("ADMIN"));
            });

            // ===============================
            // Application services
            // ===============================
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // ===============================
            // Middleware pipeline
            // ===============================
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseForwardedHeaders();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BoxHub API V1");
                c.RoutePrefix = "swagger";
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
