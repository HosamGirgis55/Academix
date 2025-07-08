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

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Nationality> Nationalities { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<Specialization> Specializations { get; set; } = null!;
        public DbSet<Experiences> Experiences { get; set; } = null!;
        public DbSet<Level> Levels { get; set; } = null!;
        public DbSet<Field> Fields { get; set; } = null!;
        public DbSet<Communication> Communication { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<GraduationStatus> GraduationStatuses { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<StudentSkill> StudentSkills { get; set; } = null!;
        public DbSet<StudentExperience> StudentExperiences { get; set; } = null!;
        public DbSet<LearningInterest> LearningInterests { get; set; } = null!;
        public DbSet<LearningInterestsStudent> LearningInterestsStudents { get; set; } = null!;
        public DbSet<Exame> Exames { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasOne(s => s.User)
                      .WithOne()
                      .HasForeignKey<Student>(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Nationality)
                      .WithMany()
                      .HasForeignKey(s => s.NationalityId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.ResidenceCountry)
                      .WithMany()
                      .HasForeignKey(s => s.ResidenceCountryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Level)
                      .WithMany(l => l.Students)
                      .HasForeignKey(s => s.LevelId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.GraduationStatus)
                      .WithMany(g => g.Students)
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

                entity.OwnsMany(t => t.Certificates, certificateBuilder =>
                {
                    certificateBuilder.ToJson();
                });

                entity.OwnsMany(t => t.Educations, educationBuilder =>
                {
                    educationBuilder.ToJson();
                });

                entity.HasMany(t => t.Exames)
                      .WithOne(e => e.Teacher)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Level configuration
            modelBuilder.Entity<Level>(entity =>
            {
                entity.HasMany(l => l.Students)
                      .WithOne(s => s.Level)
                      .HasForeignKey(s => s.LevelId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // GraduationStatus configuration
            modelBuilder.Entity<GraduationStatus>(entity =>
            {
                entity.HasMany(g => g.Students)
                      .WithOne(s => s.GraduationStatus)
                      .HasForeignKey(s => s.GraduationStatusId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ApplicationUser configuration
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Country)
                      .WithMany()
                      .HasForeignKey(u => u.CountryId)
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

            // LearningInterestsStudent configuration
            modelBuilder.Entity<LearningInterestsStudent>(entity =>
            {
                entity.HasKey(lis => new { lis.StudentId, lis.LearningInterestId });

                entity.HasOne(lis => lis.Students)
                      .WithMany(s => s.LearningInterests)
                      .HasForeignKey(lis => lis.StudentId);

                entity.HasOne(lis => lis.LearningInterests)
                      .WithMany(li => li.LearningInterests)
                      .HasForeignKey(lis => lis.LearningInterestId);
            });
        }
    }
} 