using Academix.Domain.Entities;
using Academix.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class SeedDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDataService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAllAsync()
        {
            await _context.Database.MigrateAsync();
            await SeedRolesAsync();
            await SeedCountriesAsync();
            await SeedNationalitiesAsync();
            await SeedFieldsAsync();
            await SeedLevelsAsync();
            await SeedSpecializationsAsync();
            await SeedPositionsAsync();
            await SeedSkillsAsync();
            await SeedExperiencesAsync();
            await SeedTeachingAreasAsync();
            await SeedAgeGroupsAsync();
            await SeedCommunicationMethodsAsync();
            await SeedTeachingLanguagesAsync();
            await _context.SaveChangesAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await _roleManager.RoleExistsAsync("Teacher"))
                await _roleManager.CreateAsync(new IdentityRole("Teacher"));
            if (!await _roleManager.RoleExistsAsync("Student"))
                await _roleManager.CreateAsync(new IdentityRole("Student"));
        }

        private async Task SeedCountriesAsync()
        {
            if (!await _context.Countries.AnyAsync())
            {
                var countries = new List<Country>
                {
                    new Country { NameAr = "المملكة العربية السعودية", NameEn = "Saudi Arabia" },
                    new Country { NameAr = "مصر", NameEn = "Egypt" },
                    new Country { NameAr = "الإمارات العربية المتحدة", NameEn = "United Arab Emirates" },
                    new Country { NameAr = "الكويت", NameEn = "Kuwait" },
                    new Country { NameAr = "قطر", NameEn = "Qatar" },
                    new Country { NameAr = "البحرين", NameEn = "Bahrain" },
                    new Country { NameAr = "عمان", NameEn = "Oman" },
                    new Country { NameAr = "الأردن", NameEn = "Jordan" },
                    new Country { NameAr = "لبنان", NameEn = "Lebanon" },
                    new Country { NameAr = "العراق", NameEn = "Iraq" }
                };
                await _context.Countries.AddRangeAsync(countries);
            }
        }

        private async Task SeedNationalitiesAsync()
        {
            if (!await _context.Nationalities.AnyAsync())
            {
                var nationalities = new List<Nationality>
                {
                    new Nationality { NameAr = "سعودي", NameEn = "Saudi" },
                    new Nationality { NameAr = "مصري", NameEn = "Egyptian" },
                    new Nationality { NameAr = "إماراتي", NameEn = "Emirati" },
                    new Nationality { NameAr = "كويتي", NameEn = "Kuwaiti" },
                    new Nationality { NameAr = "قطري", NameEn = "Qatari" },
                    new Nationality { NameAr = "بحريني", NameEn = "Bahraini" },
                    new Nationality { NameAr = "عماني", NameEn = "Omani" },
                    new Nationality { NameAr = "أردني", NameEn = "Jordanian" },
                    new Nationality { NameAr = "لبناني", NameEn = "Lebanese" },
                    new Nationality { NameAr = "عراقي", NameEn = "Iraqi" }
                };
                await _context.Nationalities.AddRangeAsync(nationalities);
            }
        }

        private async Task SeedFieldsAsync()
        {
            if (!await _context.Fields.AnyAsync())
            {
                var fields = new List<Field>
                {
                    new Field { NameAr = "علوم الحاسب", NameEn = "Computer Science", },
                    new Field { NameAr = "هندسة البرمجيات", NameEn = "Software Engineering" },
                    new Field { NameAr = "تقنية المعلومات", NameEn = "Information Technology" },
                    new Field { NameAr = "نظم المعلومات", NameEn = "Information Systems" },
                    new Field { NameAr = "هندسة الحاسب", NameEn = "Computer Engineering" }
                };
                await _context.Fields.AddRangeAsync(fields);
            }
        }

        private async Task SeedLevelsAsync()
        {
            if (!await _context.Levels.AnyAsync())
            {
                var levels = new List<Level>
                {
                    new Level { NameAr = "مبتدئ", NameEn = "Beginner" },
                    new Level { NameAr = "متوسط", NameEn = "Intermediate" },
                    new Level { NameAr = "متقدم", NameEn = "Advanced" },
                    new Level { NameAr = "خبير", NameEn = "Expert" }
                };
                await _context.Levels.AddRangeAsync(levels);
            }
        }

        private async Task SeedSpecializationsAsync()
        {
            if (!await _context.Specializations.AnyAsync())
            {
                var specializations = new List<Specialization>
                {
                    new Specialization { NameAr = "تطوير الواجهة الأمامية", NameEn = "Frontend Development" },
                    new Specialization { NameAr = "تطوير الخلفية", NameEn = "Backend Development" },
                    new Specialization { NameAr = "تطوير تطبيقات الموبايل", NameEn = "Mobile Development" },
                    new Specialization { NameAr = "علوم البيانات", NameEn = "Data Science" },
                    new Specialization { NameAr = "الذكاء الاصطناعي", NameEn = "Artificial Intelligence" },
                    new Specialization { NameAr = "أمن المعلومات", NameEn = "Information Security" },
                    new Specialization { NameAr = "الحوسبة السحابية", NameEn = "Cloud Computing" }
                };
                await _context.Specializations.AddRangeAsync(specializations);
            }
        }

        private async Task SeedPositionsAsync()
        {
            if (!await _context.Positions.AnyAsync())
            {
                var positions = new List<Position>
                {
                    new Position { NameAr = "مطور برمجيات", NameEn = "Software Developer" },
                    new Position { NameAr = "مهندس برمجيات", NameEn = "Software Engineer" },
                    new Position { NameAr = "مطور واجهة أمامية", NameEn = "Frontend Developer" },
                    new Position { NameAr = "مطور خلفية", NameEn = "Backend Developer" },
                    new Position { NameAr = "مطور تطبيقات موبايل", NameEn = "Mobile Developer" },
                    new Position { NameAr = "عالم بيانات", NameEn = "Data Scientist" },
                    new Position { NameAr = "مهندس ذكاء اصطناعي", NameEn = "AI Engineer" },
                    new Position { NameAr = "مهندس DevOps", NameEn = "DevOps Engineer" },
                    new Position { NameAr = "مهندس أمن سيبراني", NameEn = "Cybersecurity Engineer" },
                    new Position { NameAr = "مدير تقني", NameEn = "Technical Lead" }
                };
                await _context.Positions.AddRangeAsync(positions);
            }
        }

        private async Task SeedSkillsAsync()
        {
            if (!await _context.Skills.AnyAsync())
            {
                var skills = new List<Skill>
                {
                    new Skill { NameAr = "HTML", NameEn = "HTML" },
                    new Skill { NameAr = "CSS", NameEn = "CSS" },
                    new Skill { NameAr = "JavaScript", NameEn = "JavaScript" },
                    new Skill { NameAr = "TypeScript", NameEn = "TypeScript" },
                    new Skill { NameAr = "React", NameEn = "React" },
                    new Skill { NameAr = "Angular", NameEn = "Angular" },
                    new Skill { NameAr = "Vue.js", NameEn = "Vue.js" },
                    new Skill { NameAr = "Node.js", NameEn = "Node.js" },
                    new Skill { NameAr = "Python", NameEn = "Python" },
                    new Skill { NameAr = "Java", NameEn = "Java" },
                    new Skill { NameAr = "C#", NameEn = "C#" },
                    new Skill { NameAr = "SQL", NameEn = "SQL" },
                    new Skill { NameAr = "MongoDB", NameEn = "MongoDB" },
                    new Skill { NameAr = "Docker", NameEn = "Docker" },
                    new Skill { NameAr = "Kubernetes", NameEn = "Kubernetes" },
                    new Skill { NameAr = "AWS", NameEn = "AWS" },
                    new Skill { NameAr = "Azure", NameEn = "Azure" },
                    new Skill { NameAr = "Git", NameEn = "Git" }
                };
                await _context.Skills.AddRangeAsync(skills);
            }
        }

        private async Task SeedExperiencesAsync()
        {
            if (!await _context.Experiences.AnyAsync())
            {
                var experiences = new List<Experience>
                {
                    new Experience { NameAr = "أقل من سنة", NameEn = "Less than 1 year" },
                    new Experience { NameAr = "1-3 سنوات", NameEn = "1-3 years" },
                    new Experience { NameAr = "3-5 سنوات", NameEn = "3-5 years" },
                    new Experience { NameAr = "5-7 سنوات", NameEn = "5-7 years" },
                    new Experience { NameAr = "7-10 سنوات", NameEn = "7-10 years" },
                    new Experience { NameAr = "أكثر من 10 سنوات", NameEn = "More than 10 years" }
                };
                await _context.Experiences.AddRangeAsync(experiences);
            }
        }

        private async Task SeedTeachingAreasAsync()
        {
            if (!await _context.TeachingAreas.AnyAsync())
            {
                var areas = new List<TeachingArea>
                {
                    new TeachingArea { NameAr = "برمجة", NameEn = "Programming" },
                    new TeachingArea { NameAr = "تطوير الويب", NameEn = "Web Development" },
                    new TeachingArea { NameAr = "تطوير تطبيقات الموبايل", NameEn = "Mobile Development" },
                    new TeachingArea { NameAr = "قواعد البيانات", NameEn = "Databases" },
                    new TeachingArea { NameAr = "الذكاء الاصطناعي", NameEn = "Artificial Intelligence" }
                };
                await _context.TeachingAreas.AddRangeAsync(areas);
            }
        }

        private async Task SeedAgeGroupsAsync()
        {
            if (!await _context.AgeGroups.AnyAsync())
            {
                var ageGroups = new List<AgeGroup>
                {
                    new AgeGroup { 
                        NameAr = "أطفال", 
                        NameEn = "Children", 
                        
                         MinAge = 7,
                        MaxAge = 12
                    },
                    new AgeGroup { 
                        NameAr = "مراهقين", 
                        NameEn = "Teenagers", 
                        
                        MinAge = 13,
                        MaxAge = 17
                    },
                    new AgeGroup { 
                        NameAr = "بالغين", 
                        NameEn = "Adults", 
                       
                        MinAge = 18,
                        MaxAge = 100
                    }
                };
                await _context.AgeGroups.AddRangeAsync(ageGroups);
            }
        }

        private async Task SeedCommunicationMethodsAsync()
        {
            if (!await _context.CommunicationMethods.AnyAsync())
            {
                var methods = new List<CommunicationMethod>
                {
                    new CommunicationMethod { NameAr = "مكالمة فيديو", NameEn = "Video Call" },
                    new CommunicationMethod { NameAr = "مكالمة صوتية", NameEn = "Voice Call" },
                    new CommunicationMethod { NameAr = "رسائل نصية", NameEn = "Text Chat" }
                };
                await _context.CommunicationMethods.AddRangeAsync(methods);
            }
        }

        private async Task SeedTeachingLanguagesAsync()
        {
            if (!await _context.TeachingLanguages.AnyAsync())
            {
                var languages = new List<TeachingLanguage>
                {
                    new TeachingLanguage { NameAr = "العربية", NameEn = "Arabic" },
                    new TeachingLanguage { NameAr = "الإنجليزية", NameEn = "English" }
                };
                await _context.TeachingLanguages.AddRangeAsync(languages);
            }
        }
    }
} 