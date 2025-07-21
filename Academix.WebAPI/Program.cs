using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.SignalR;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using Academix.Helpers;
using Academix.Infrastructure;
using Academix.Infrastructure.Data;
using Academix.Infrastructure.Services;
using Academix.WebAPI.Common;
using Academix.WebAPI.Common.Middleware;
using Academix.WebAPI.Hubs;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

//Allow CROS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://127.0.0.1:5173", "http://localhost:5173") // ضع هنا الـ Origin الصحيح فقط
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // مهم عند وجود الكوكيز أو المصادقة
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Academix API", 
        Version = "v1",
        Description = "API for Academix platform"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (without 'Bearer' prefix)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddScoped<IChatHubService, ChatHubService>();


// Add ResponseHelper
builder.Services.AddScoped<ResponseHelper>();

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddDistributedMemoryCache();

// Configure supported cultures
var supportedCultures = new[] { "en", "ar" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

// Add string localizer
builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
builder.Services.AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddSignalR();
// Add JWT Configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

// Validate JWT Configuration at startup
var jwtConfig = new JwtSettings();
builder.Configuration.GetSection(nameof(JwtSettings)).Bind(jwtConfig);
if (string.IsNullOrEmpty(jwtConfig.Key))
{
    throw new InvalidOperationException("JWT Key is not configured in appsettings.json. Please ensure JwtSettings.Key has a value.");
}

// Add JWT Bearer Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});


// Add Authorization
builder.Services.AddAuthorization();

// Add Email Configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));

// Add Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<ITimeZoneService, TimeZoneService>();
builder.Services.AddScoped<ResponseHelper>();

// Add File Upload Service
builder.Services.AddScoped<Academix.Helper.FileUploaderHelper>();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ICommand<>).Assembly));

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(ICommand<>).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var seedDataService = scope.ServiceProvider.GetRequiredService<SeedDataService>();
    await seedDataService.SeedAllAsync();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Add request localization
app.UseRequestLocalization();

// Use custom middleware
app.UseMiddleware<LocalizationMiddleware>();
app.UseMiddleware<TimeZoneMiddleware>();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapEndpoints();
app.MapHub<ChatHub>("/chatHub");

//builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();


// Map controllers
app.MapControllers();

app.Run();
