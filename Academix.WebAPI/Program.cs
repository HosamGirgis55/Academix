
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
 
 
namespace Academix.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            // Configure Entity Framework
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Academix.Infrastructure")));

            // Register repositories
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register helpers
            builder.Services.AddScoped<FileUploaderHelper>();

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
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                
                // Create database if it doesn't exist
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.EnsureCreated();
                }
            }

            // Add localization middleware
            app.UseLocalization();
            
            app.UseAuthorization();

            // Map traditional controllers
            app.MapControllers();

            // Map all minimal API endpoints that implement IEndpoint
            app.MapEndpoints();

            app.Run();
        }
    }
}
