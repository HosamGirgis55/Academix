using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Sessions.Queries.GetAllSessionForTeacher
{
    public class GetSessionRequestByTeacherIdQuery  :IRequest<Result<SessionRequestPageResult>>
    {
        public Guid TeacherId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SessionRequestPageResult
    {
        public List<SessionRequestDto> SessionRequest { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
