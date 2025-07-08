using Academix.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Academix.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Specialization> specializations  { get; set; }
        public DbSet<Experiences> Experiences { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Communication> Communication { get; set; }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<GraduationStatus> GraduationStatuses { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<StudentSkill> StudentSkills { get; set; }
        public DbSet<StudentExperience> StudentExperiences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student configuration
            modelBuilder.Entity<Teacher>(entity =>
            {
               
                entity.HasOne(s => s.User)
                      .WithOne()
                      .HasForeignKey<Teacher>(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Configure relationships
                entity.HasOne(s => s.Nationality)
                      .WithMany()
                      .HasForeignKey(s => s.NationalityId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.ResidenceCountry)
                      .WithMany()
                      .HasForeignKey(s => s.ResidenceCountryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Level)
                      .WithMany()
                      .HasForeignKey(s => s.LevelId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.GraduationStatus)
                      .WithMany()
                      .HasForeignKey(s => s.GraduationStatusId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Specialist)
                      .WithMany()
                      .HasForeignKey(s => s.SpecialistId)
                      .OnDelete(DeleteBehavior.Restrict);

                
               
            });

            // Teacher configuration
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasOne(t => t.User)
                      .WithOne()
                      .HasForeignKey<Teacher>(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Nationality)
                      .WithMany()
                      .HasForeignKey(t => t.NationalityId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Country)
                      .WithMany()
                      .HasForeignKey(t => t.CountryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.Certificates)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                          v => JsonSerializer.Deserialize<List<TeacherCertificate>>(v, (JsonSerializerOptions)null));

                entity.Property(t => t.Educations)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                          v => JsonSerializer.Deserialize<List<TeacherEducation>>(v, (JsonSerializerOptions)null));
            });

            // ApplicationUser configuration
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Country)
                      .WithMany()
                      .HasForeignKey(u => u.CountryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Country configuration
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasMany(c => c.Students)
                      .WithOne()
                      .HasForeignKey("CountryId")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // StudentSkill configuration
            modelBuilder.Entity<StudentSkill>(entity =>
            {
                entity.HasKey(ss => new { ss.StudentId, ss.SkillId });

                entity.HasOne(ss => ss.Student)
                      .WithMany(s => s.Skills)
                      .HasForeignKey(ss => ss.StudentId);

                entity.HasOne(ss => ss.Skill)
                      .WithMany(s => s.StudentSkills)
                      .HasForeignKey(ss => ss.SkillId);
            });

            // StudentExperience configuration
            modelBuilder.Entity<StudentExperience>(entity =>
            {
                entity.HasKey(se => new { se.StudentId, se.ExperienceId });

                entity.HasOne(se => se.Student)
                      .WithMany(s => s.Experiences)
                      .HasForeignKey(se => se.StudentId);

                entity.HasOne(se => se.Experience)
                      .WithMany(e => e.StudentExperiences)
                      .HasForeignKey(se => se.ExperienceId);
            });

            // Seed some initial data
           // SeedData(modelBuilder);
        }

        //private void SeedData(ModelBuilder modelBuilder)
        //{
        //    // Seed Students
        //    var student1Id = Guid.NewGuid();
        //    var student2Id = Guid.NewGuid();
            
        //    modelBuilder.Entity<Student>().HasData(
        //        new Student
        //        {
        //            Id = student1Id,
        //            FirstName = "John",
        //            FirstNameAr = "جون",
        //            LastName = "Doe",
        //            LastNameAr = "دو",
        //            Email = "john.doe@example.com",
        //            DateOfBirth = new DateTime(2000, 1, 15),
        //            StudentNumber = "20240001",
        //            CreatedAt = DateTime.UtcNow
        //        },
        //        new Student
        //        {
        //            Id = student2Id,
        //            FirstName = "Jane",
        //            FirstNameAr = "جين",
        //            LastName = "Smith",
        //            LastNameAr = "سميث",
        //            Email = "jane.smith@example.com",
        //            DateOfBirth = new DateTime(2001, 3, 20),
        //            StudentNumber = "20240002",
        //            CreatedAt = DateTime.UtcNow
        //        }
        //    );

           
            
        //}
    }
} 