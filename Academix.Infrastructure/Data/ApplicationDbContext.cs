using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace Academix.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.FirstNameAr)
                    .HasMaxLength(50);
                
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.LastNameAr)
                    .HasMaxLength(50);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.HasIndex(e => e.Email)
                    .IsUnique();
                
                entity.Property(e => e.StudentNumber)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.HasIndex(e => e.StudentNumber)
                    .IsUnique();
            });

            
           

            // Seed some initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Students
            var student1Id = Guid.NewGuid();
            var student2Id = Guid.NewGuid();
            
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = student1Id,
                    FirstName = "John",
                    FirstNameAr = "جون",
                    LastName = "Doe",
                    LastNameAr = "دو",
                    Email = "john.doe@example.com",
                    DateOfBirth = new DateTime(2000, 1, 15),
                    StudentNumber = "20240001",
                    CreatedAt = DateTime.UtcNow
                },
                new Student
                {
                    Id = student2Id,
                    FirstName = "Jane",
                    FirstNameAr = "جين",
                    LastName = "Smith",
                    LastNameAr = "سميث",
                    Email = "jane.smith@example.com",
                    DateOfBirth = new DateTime(2001, 3, 20),
                    StudentNumber = "20240002",
                    CreatedAt = DateTime.UtcNow
                }
            );

           
            
        }
    }
} 