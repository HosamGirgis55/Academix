using Academix.Helper;
using Academix.WebAPI.Common;
using Academix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Academix.Domain.Interfaces;
using Academix.Infrastructure.Repositories;
using MediatR;
using FluentValidation;
using Academix.Application.Common.Interfaces;
using Academix.Infrastructure.Services;
using Academix.WebAPI.Common.Middleware;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Academix.Application.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Academix.Helpers;

namespace Academix.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
                { 
                    Title = "Academix API", 
                    Version = "v1",
                    Description = "Academic Management System API"
                });

                // Add JWT Bearer Authentication
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "Enter your JWT token in the text input below.\n\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
            
            // Configure Entity Framework
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Academix.Infrastructure")));

            // Configure Identity
            builder.Services.AddIdentity<Domain.Entities.ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;

                // User settings
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                // Sign-in settings
                options.SignIn.RequireConfirmedEmail = false; // Set to true if you want email confirmation
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Register repositories
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register helpers
            builder.Services.AddScoped<FileUploaderHelper>();
            builder.Services.AddScoped<ResponseHelper>();

            // Add MediatR configuration
            builder.Services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(Application.Common.Interfaces.ICommand).Assembly);
            });

            // Add FluentValidation
            builder.Services.AddValidatorsFromAssembly(typeof(Application.Common.Interfaces.ICommand).Assembly);

            // Add AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            // Add Localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddScoped<ILocalizationService, LocalizationService>();
            builder.Services.AddHttpContextAccessor();

            // Add TimeZone services
            builder.Services.AddScoped<ITimeZoneService, Infrastructure.Services.TimeZoneService>();
            
            // Configure Email Settings
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            
            // Add Email Service
            builder.Services.AddScoped<IEmailService, Infrastructure.Services.EmailService>();
            
            // Add session support for time zone storage and OTP caching
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddMemoryCache(); // For OTP storage
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Configure JWT Settings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // Add JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings!.Key);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();

            // Register Seed Data Service
            builder.Services.AddScoped<Infrastructure.Services.SeedDataService>();
            
            // Configure supported cultures
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en"),
                new CultureInfo("ar")
            };
            
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                
                // Add Accept-Language header provider
                options.RequestCultureProviders.Insert(0, new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Add session middleware (must be before localization and time zone)
            app.UseSession();
            
            // Add localization middleware
            app.UseRequestLocalization();
            
            // Add time zone middleware
            app.UseMiddleware<Academix.WebAPI.Common.Middleware.TimeZoneMiddleware>();
            
            app.UseAuthentication();
            app.UseAuthorization();

            // Map traditional controllers
            app.MapControllers();

            // Map all minimal API endpoints that implement IEndpoint
            app.MapEndpoints();

            // Create database if it doesn't exist and seed data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var seedService = services.GetRequiredService<Infrastructure.Services.SeedDataService>();
                    await context.Database.MigrateAsync();
                    await seedService.SeedAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                }
            }

            await app.RunAsync();
        }
    }
}
