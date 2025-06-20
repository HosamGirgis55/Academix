using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Academix.Application.Common.Mappings;

namespace Academix.Application.Features.Students.Queries.GetStudentById
{
    public class GetStudentByIdQueryHandler : IQueryHandler<GetStudentByIdQuery, Result<StudentDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GetStudentByIdQueryHandler(IStudentRepository studentRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId);
            
            if (student == null)
            {
                return Result<StudentDto>.Failure($"Student with ID {request.StudentId} not found.");
            }

            // Get culture from HttpContext
            var culture = _httpContextAccessor.HttpContext?.Items["Culture"]?.ToString() ?? "en";

            // Use extension method for culture-aware mapping
            var studentDto = student.ToStudentDto(_mapper, culture);
            
            return Result<StudentDto>.Success(studentDto);
        }
    }
} 