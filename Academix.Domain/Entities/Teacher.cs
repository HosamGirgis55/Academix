using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    public class Teacher : BaseEntity
    {
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        // Basic Info
        public string Bio { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public List<string> AdditionalInterests { get; set; } = new();
        public decimal Salary { get; set; }

       
        public Guid CountryId { get; set; }
        [ForeignKey("CountryId")]
        public Country Country { get; set; } = null!;

        // Education and Certificates
        public List<TeacherEducation> Educations { get; set; } = new();
        public List<Certificate> Certificates { get; set; } = new();

        // Exams
        public List<Exame>? Exames { get; set; }

        // Teacher Preferences
        public ICollection<TeacherTeachingArea> TeacherTeachingAreas { get; set; } = new List<TeacherTeachingArea>();
        public ICollection<TeacherAgeGroup> TeacherAgeGroups { get; set; } = new List<TeacherAgeGroup>();
        public ICollection<TeacherCommunicationMethod> TeacherCommunicationMethods { get; set; } = new List<TeacherCommunicationMethod>();
        public ICollection<TeacherTeachingLanguage> TeacherTeachingLanguages { get; set; } = new List<TeacherTeachingLanguage>();
    }

    [Owned]
    public class TeacherEducation
    {
        public string Institution { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    [Owned]
    public class Certificate
    {
        public string Name { get; set; } = string.Empty;
        public string CertificateUrl { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public string ExamResult { get; set; } = string.Empty;
    }

    public class Exame : BaseEntity
    {
        public string Name { get; set; }
        public string ExamResult { get; set; }
        public string IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
        public string ExameCertificateUrl { get; set; }

        public Teacher Teacher { get; set; }
    }
}
