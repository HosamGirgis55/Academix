using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academix.Domain.Entities
{
    public class Teacher : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Bio { get; set; }
        public string Github { get; set; }
        public string ProfilePictureUrl { get; set; }

        public List<TeacherEducation> Educations { get; set; } = new();
        [Column(TypeName = "nvarchar(max)")]
        public List<TeacherCertificate> Certificates { get; set; } = new();

        public Guid NationalityId { get; set; }
        public Country Nationality { get; set; }

        public Guid CountryId { get; set; }
        public Country Country { get; set; }

        public List<Exame>? Exames { get; set; }
    }

    [Owned]
    public class TeacherEducation
    {
        public string Degree { get; set; }
        public string Institution { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FieldOfStudy { get; set; }
    }

    [Owned]
    public class TeacherCertificate
    {
        public string Name { get; set; }
        public string CertificateUrl { get; set; }
        public string Description { get; set; }
        public DateTime IssuedDate { get; set; }
        public string IssuedBy { get; set; }
    }

    public class Exame:BaseEntity
    {
        public string Name { get; set; }
        public string ExamResult { get; set; }
        public string IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
        public string ExameCertificateUrl { get; set; }

        public Teacher Teacher { get; set; }
    }
}
