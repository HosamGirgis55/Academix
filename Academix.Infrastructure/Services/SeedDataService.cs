using Academix.Domain.Entities;
using Academix.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Academix.Infrastructure.Services
{
    public class SeedDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDataService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // Seed Roles
            await SeedRolesAsync();

            // Seed Countries
            await SeedCountriesAsync();

            await _context.SaveChangesAsync();
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[] { "Admin", "Student", "Teacher" };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task SeedCountriesAsync()
        {
            if (!await _context.Countries.AnyAsync())
            {
                var countries = new List<Country>
                {
                    new Country
                    {
                        Id = Guid.NewGuid(),
                        NameEn = "Egypt",
                        NameAr = "مصر",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Country
                    {
                        Id = Guid.NewGuid(),
                        NameEn = "Saudi Arabia",
                        NameAr = "السعودية",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Country
                    {
                        Id = Guid.NewGuid(),
                        NameEn = "United Arab Emirates",
                        NameAr = "الإمارات العربية المتحدة",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Country
                    {
                        Id = Guid.NewGuid(),
                        NameEn = "Jordan",
                        NameAr = "الأردن",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Country
                    {
                        Id = Guid.NewGuid(),
                        NameEn = "Lebanon",
                        NameAr = "لبنان",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Country
                    {
                        Id = Guid.NewGuid(),
                        NameEn = "Kuwait",
                        NameAr = "الكويت",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Country
                    {
                        Id = Guid.NewGuid(),
                        NameEn = "Qatar",
                        NameAr = "قطر",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Country
                    {
                        Id = Guid.NewGuid(),
                        NameEn = "Bahrain",
                        NameAr = "البحرين",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await _context.Countries.AddRangeAsync(countries);
            }
        }
    }
} 