using Academix.Application.Common.Models;
using Academix.Application.Features.Teachers.Query.GetAll;
using Academix.Domain.DTOs;
using Academix.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Query.Teacher.GetTeachers
{
    public class GetTeacherQuery : IRequest<Result<TeachersPagedResult>>
    {
        public Status Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
