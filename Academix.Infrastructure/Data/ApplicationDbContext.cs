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

        public DbSet<Connections> connections { get; set; }
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Experience> Experiences { get; set; } = null!;
        public DbSet<Field> Fields { get; set; } = null!;
        public DbSet<Level> Levels { get; set; } = null!;
        public DbSet<CommunicationMethod> CommunicationMethods { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<Specialization> Specializations { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<StudentExperience> StudentExperiences { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<GraduationStatus> GraduationStatuses { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<StudentSkill> StudentSkills { get; set; } = null!;
        public DbSet<TeacherSkill> TeacherSkills { get; set; } = null!;
         public DbSet<LearningInterestsStudent> LearningInterestsStudents { get; set; } = null!;
        public DbSet<Exame> Exames { get; set; } = null!;

        // Teacher Preferences Tables
        public DbSet<TeachingArea> TeachingAreas { get; set; } = null!;
        public DbSet<AgeGroup> AgeGroups { get; set; } = null!;
        public DbSet<TeachingLanguage> TeachingLanguages { get; set; } = null!;

        // Junction Tables
        public DbSet<TeacherTeachingArea> TeacherTeachingAreas { get; set; } = null!;
        public DbSet<TeacherAgeGroup> TeacherAgeGroups { get; set; } = null!;
        public DbSet<TeacherCommunicationMethod> TeacherCommunicationMethods { get; set; } = null!;
        public DbSet<TeacherTeachingLanguage> TeacherTeachingLanguages { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        
        // Session Management
        public DbSet<SessionRequest> SessionRequests { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        
        // Payment Management
        public DbSet<Payment> Payments { get; set; } = null!;

        //Chat
        public DbSet<ChatMessage> ChatMessages { get; set; }

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

                entity.Property(t => t.AdditionalInterests)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                          v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>());

                entity.HasMany(t => t.Exames)
                      .WithOne(e => e.Teacher)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Teacher Preferences configurations
            modelBuilder.Entity<TeacherTeachingArea>(entity =>
            {
                entity.HasKey(tta => new { tta.TeacherId, tta.TeachingAreaId });

                entity.HasOne(tta => tta.Teacher)
                      .WithMany(t => t.TeacherTeachingAreas)
                      .HasForeignKey(tta => tta.TeacherId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tta => tta.TeachingArea)
                      .WithMany(ta => ta.TeacherTeachingAreas)
                      .HasForeignKey(tta => tta.TeachingAreaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TeacherAgeGroup>(entity =>
            {
                entity.HasKey(tag => new { tag.TeacherId, tag.AgeGroupId });

                entity.HasOne(tag => tag.Teacher)
                      .WithMany(t => t.TeacherAgeGroups)
                      .HasForeignKey(tag => tag.TeacherId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tag => tag.AgeGroup)
                      .WithMany(ag => ag.TeacherAgeGroups)
                      .HasForeignKey(tag => tag.AgeGroupId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TeacherCommunicationMethod>(entity =>
            {
                entity.HasKey(tcm => new { tcm.TeacherId, tcm.CommunicationMethodId });

                entity.HasOne(tcm => tcm.Teacher)
                      .WithMany(t => t.TeacherCommunicationMethods)
                      .HasForeignKey(tcm => tcm.TeacherId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tcm => tcm.CommunicationMethod)
                      .WithMany(cm => cm.TeacherCommunicationMethods)
                      .HasForeignKey(tcm => tcm.CommunicationMethodId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TeacherTeachingLanguage>(entity =>
            {
                entity.HasKey(ttl => new { ttl.TeacherId, ttl.TeachingLanguageId });

                entity.HasOne(ttl => ttl.Teacher)
                      .WithMany(t => t.TeacherTeachingLanguages)
                      .HasForeignKey(ttl => ttl.TeacherId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ttl => ttl.TeachingLanguage)
                      .WithMany(tl => tl.TeacherTeachingLanguages)
                      .HasForeignKey(ttl => ttl.TeachingLanguageId)
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

            //// StudentSkill configuration
            //modelBuilder.Entity<StudentSkill>(entity =>
            //{
            //    entity.HasOne(ss => ss.Student)
            //          .WithMany(s => s.Skills)
            //          .HasForeignKey(ss => ss.StudentId)
            //          .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(ss => ss.Skill)
            //          .WithMany(s => s.StudentSkills)
            //          .HasForeignKey(ss => ss.SkillId)
            //          .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasIndex(ss => new { ss.StudentId, ss.SkillId })
            //          .IsUnique();
            //});

            //// StudentExperience configuration
            //modelBuilder.Entity<StudentExperience>(entity =>
            //{
            //    entity.HasOne(se => se.Student)
            //          .WithMany(s => s.Experiences)
            //          .HasForeignKey(se => se.StudentId)
            //          .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(se => se.Experience)
            //          .WithMany(e => e.StudentExperiences)
            //          .HasForeignKey(se => se.ExperienceId)
            //          .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasIndex(se => new { se.StudentId, se.ExperienceId })
            //          .IsUnique();
            //});

            // LearningInterestsStudent configuration
            modelBuilder.Entity<LearningInterestsStudent>(entity =>
            {
 
                entity.HasOne(lis => lis.Students)
                      .WithMany(s => s.LearningInterests)
                      .HasForeignKey(lis => lis.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(lis => lis.Field)
                      .WithMany(li => li.LearningInterests)
                      .HasForeignKey(lis => lis.FieldId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // TeacherSkill configuration
            modelBuilder.Entity<TeacherSkill>(entity =>
            {
                entity.HasOne(ts => ts.Teacher)
                      .WithMany(t => t.Skills)
                      .HasForeignKey(ts => ts.TeacherId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ts => ts.Skill)
                      .WithMany(s => s.TeacherSkills)
                      .HasForeignKey(ts => ts.SkillId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ts => new { ts.TeacherId, ts.SkillId })
                      .IsUnique();
            });

            // Comment configuration
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.Student)
                      .WithMany(s => s.Comments)
                      .HasForeignKey(c => c.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Teacher)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(c => c.TeacherId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(c => new { c.StudentId, c.TeacherId })
                      .IsUnique(); // Ensure one comment per student per teacher

                entity.Property(c => c.Content)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(c => c.Rating)
                      .IsRequired()
                      .HasDefaultValue(1);

                entity.Property(c => c.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
} 