using Academix.Application.Common.Models;
using Academix.Application.Features.Teachers.Commands.RegisterTeacher;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Teachers.Commands.UpdateTeacher
{
    public class UpdateTeacherCommand : IRequest<Result>
    {
        // User Info
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Basic Info
        public string Bio { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public List<string> AdditionalInterests { get; set; } = new();


        // Education
        public List<TeacherEducationDto> Educations { get; set; } = new();
        public List<TeacherCertificateDto> Certificates { get; set; } = new();

        // Skills
        public List<TeacherSkillRegistrationDto> Skills { get; set; } = new();
        public decimal Salary { get; set; }

    }
}
