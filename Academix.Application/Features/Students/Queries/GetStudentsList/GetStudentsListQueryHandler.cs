using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Academix.Application.Common.Mappings;

namespace Academix.Application.Features.Students.Queries.GetStudentsList
{
    public class GetStudentsListQueryHandler : IQueryHandler<GetStudentsListQuery, Result<StudentsListVm>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GetStudentsListQueryHandler(IStudentRepository studentRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Result<StudentsListVm>> Handle(GetStudentsListQuery request, CancellationToken cancellationToken)
        {
            // Get culture from HttpContext
            var culture = _httpContextAccessor.HttpContext?.Items["Culture"]?.ToString() ?? "en";
            
            // Get all students as queryable
            var allStudents = await _studentRepository.GetAllAsync();
            var query = allStudents.AsQueryable();

            // Use comprehensive extension method for filtering, ordering, and pagination
            var paginatedResult = await query.GetStudentsWithFiltersAsync(
                _mapper,
                request.SearchTerm,
                request.OrderBy,
                request.PageNumber,
                request.PageSize,
                culture,
                cancellationToken);

            // Create response using the paginated result
            var response = new StudentsListVm
            {
                Students = paginatedResult.Items,
                PageNumber = paginatedResult.PageNumber,
                PageSize = paginatedResult.PageSize,
                TotalCount = paginatedResult.TotalCount,
                TotalPages = paginatedResult.TotalPages,
                HasPreviousPage = paginatedResult.HasPreviousPage,
                HasNextPage = paginatedResult.HasNextPage
            };

            return Result<StudentsListVm>.Success(response);
        }
    }
} 