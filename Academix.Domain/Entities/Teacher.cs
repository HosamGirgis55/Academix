using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    public class Teacher: BaseEntity
    {
        // Navigation property to ApplicationUser
        public List<Educations>? Educations { get; set; }
        public List<Certificate>? Certificate { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
    [Owned]
    public class Certificate
    {
        public string Name { get; set; }
        public string CertificateUrl { get; set; }
        public string Description { get; set; }
        public DateTime IssuedDate { get; set; }
        public string IssuedBy { get; set; }
    }
    [Owned]
    public class Educations
    {
        public string Degree { get; set; }
        public string Institution { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FieldOfStudy { get; set; }
    }

}
