using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Academix.Domain.Entities
{
    public class Student : BaseEntity
    {
        
        
         
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Basic Info
        public string Bio { get; set; }
        public string Github { get; set; }
        public bool ConnectProgramming { get; set; }
        public string ProfilePictureUrl { get; set; }

        // Foreign Keys
        public Guid NationalityId { get; set; }
        public Country Nationality { get; set; }

        public Guid ResidenceCountryId { get; set; }
        public Country ResidenceCountry { get; set; }

        public Guid LevelId { get; set; }
        public Level Level { get; set; }

        public Guid GraduationStatusId { get; set; }
        public GraduationStatus GraduationStatus { get; set; }

        public Guid SpecialistId { get; set; }
        public Specialization Specialist { get; set; }

        // Collections
        public List<StudentSkill> Skills { get; set; } = new();
        public List<StudentExperience> Experiences { get; set; } = new();

        // Learning Interests
        public List<LearningInterestsStudent>? LearningInterests { get; set; } = new();
    }
    public class LearningInterestsStudent : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid LearningInterestId { get; set; }
        public Student Students { get; set; }
        public LearningInterest LearningInterests { get; set; }
    }

    public class LearningInterest:BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public List<LearningInterestsStudent>? LearningInterests { get; set; } = new();

    }


} 