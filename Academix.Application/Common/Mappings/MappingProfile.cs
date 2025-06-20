using AutoMapper;
using Academix.Domain.Entities;
using Academix.Application.Features.Students.Queries.GetStudentById;
using Academix.Application.Features.Students.Queries.GetStudentsList;
using Academix.Application.Features.Students.Commands.CreateStudent;
using Academix.Application.Features.Students.Commands.UpdateStudent;

namespace Academix.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Student mappings - Basic mapping without culture-specific logic
            // Culture-specific logic will be handled in handlers using helper methods
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.FullName, opt => opt.Ignore()) // Will be set by handler
                .ReverseMap();
            
            CreateMap<Student, StudentListDto>()
                .ForMember(dest => dest.FullName, opt => opt.Ignore()) // Will be set by handler
                .ForMember(dest => dest.Age, 
                    opt => opt.MapFrom(src => CalculateAge(src.DateOfBirth)))
                .ForMember(dest => dest.AgeGroup, opt => opt.Ignore()); // Will be set by extension method

            // Command mappings
            CreateMap<CreateStudentCommand, Student>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateStudentCommand, Student>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.StudentNumber, opt => opt.Ignore());

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