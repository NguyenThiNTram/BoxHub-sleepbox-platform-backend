using AutoMapper;
using BoxHub.Application.Mappers;
using BoxHub.Application.Validators.Hosts;
using BoxHub.Infrastructure;
using BoxHub.Infrastructure.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

            // ===============================
            // Render PORT config
            // ===============================

            if (!builder.Environment.IsDevelopment())
            {
                var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
                builder.WebHost.UseUrls($"http://*:{port}");
            }

            var configuration = builder.Configuration;
            var environment = builder.Environment;

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // ===============================
            // Controllers
            // ===============================
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<VerifyOtpRequestValidator>();

            // ===============================
            // Swagger
            // ===============================

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "BoxHub API",
                    Version = "v1.2"
                });

                // Khai baoBearer Authentication
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter token: Bearer {your JWT token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // ===============================
            // CORS
            // ===============================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFE", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:8081",
                            "http://localhost:5173", // FE local
                            "http://localhost:3000", // React
                            "https://your-fe-domain.vercel.app" // FE production
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // ===============================
            // Database (PostgreSQL - Supabase)
            // ===============================
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Create data source builder
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

            // Map Postgres enum
            //dataSourceBuilder.MapEnum<UserRole>(
            //    "core.user_role",
            //    new NpgsqlNullNameTranslator()
            //);

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
            // Authorization
            // ===============================
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly",
                    policy => policy.RequireRole("ADMIN"));
            });

            // ===============================
            // Infrastructure services
            // ===============================
            builder.Services.AddInfrastructure(builder.Configuration);

            var hangfireConn = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string required for Hangfire.");
            builder.Services.AddHangfire(cfg => cfg
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(hangfireConn));
            builder.Services.AddHangfireServer();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<AdminProfile>();
                cfg.AddProfile<PricingProfile>();
            }, builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>());

            IMapper mapper = mapperConfig.CreateMapper();

            builder.Services.AddSingleton(mapper);


            // ===============================
            // Build & Middleware pipeline
            // ===============================
            var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseForwardedHeaders();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BoxHub API V1.2");
                c.RoutePrefix = "swagger";
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowFE");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
