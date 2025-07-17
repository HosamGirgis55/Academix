using Academix.Application.Common.Models;
using Academix.Application.Features.Sessions.Queries.GetAllSessionForTeacher;
using Academix.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Sessions.Queries.GetAllSession
{
    public class GetAllSessionQuery : IRequest<Result<SessionPageResult>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


    public class SessionPageResult
    {
        public List<SessionDto> Sessions { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
