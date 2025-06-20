using AutoMapper;
using Academix.Domain.Entities;
using Academix.Application.Features.Students.Queries.GetStudentById;
using Academix.Application.Features.Students.Queries.GetStudentsList;

namespace Academix.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Student mappings
            CreateMap<Student, StudentDto>()
                .ReverseMap();
            
            CreateMap<Student, StudentListDto>()
                .ForMember(dest => dest.Age, 
                    opt => opt.MapFrom(src => CalculateAge(src.DateOfBirth)));

            // Course mappings
            // Add course mappings here when needed

            // Enrollment mappings
            // Add enrollment mappings here when needed
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) 
                age--;
            return age;
        }
    }
} 