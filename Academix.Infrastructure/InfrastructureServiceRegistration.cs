using Academix.Application.Common.Interfaces;
using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using Academix.Infrastructure.Repositories;
using Academix.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Academix.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // HttpClient for PayPal
            services.AddHttpClient<IPayPalService, PayPalService>();

            // Services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IPayPalService, PayPalService>();
            services.AddScoped<IPointsService, PointsService>();
            services.AddScoped<SeedDataService>();

            // Background Services
            services.AddHostedService<PaymentStatusCheckService>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IFieldRepository, FieldRepository>();
            services.AddScoped<ILevelRepository, LevelRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            services.AddScoped<IExperienceRepository, ExperienceRepository>();
            services.AddScoped<IGraduationStatusRepository, GraduationStatusRepository>();
            services.AddScoped<ISKilleRpository, SkilleRepository>();

            return services;
        }
    }
} 