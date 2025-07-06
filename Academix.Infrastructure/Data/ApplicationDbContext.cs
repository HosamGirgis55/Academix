using Academix.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

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
        public DbSet<Nationality> Nationalities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student configuration
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.OwnsMany(e => e.Educations, builder =>
                {
                    builder.ToJson(); // EF Core 7+ only
                });

                entity.OwnsMany(e => e.Certificate, builder =>
                {
                    builder.ToJson(); // EF Core 7+ only
                });

                entity.HasOne(s => s.User)
                      .WithOne()
                      .HasForeignKey<Teacher>(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Teacher configuration
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasOne(t => t.User)
                      .WithOne()
                      .HasForeignKey<Teacher>(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
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