using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using Academix.Infrastructure.Repositories;
using Academix.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Academix.Application.Common.Interfaces;

namespace Academix.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<SeedDataService>();

            // Add localization service
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ITimeZoneService, TimeZoneService>();
            services.AddScoped<IEmailService, EmailService>();

            // Add repositories
            services.AddScoped<IFieldRepository, FieldRepository>();
            services.AddScoped<ILevelRepository, LevelRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();


            return services;
        }
    }
} 