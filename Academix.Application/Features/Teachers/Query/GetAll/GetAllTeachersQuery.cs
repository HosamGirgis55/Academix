using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;

namespace Academix.Application.Features.Teachers.Query.GetAll
{
    public class GetAllTeachersQuery : IRequest<Result<TeachersPagedResult>>
    {
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        // Skills filtering (optional)
        public List<Guid> SkillIds { get; set; } = new();
        
        // Ordering
        public bool OrderByRating { get; set; } = true; // Default to order by rating
    }
    
    public class TeachersPagedResult
    {
        public List<TeacherDto> Teachers { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
