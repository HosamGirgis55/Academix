using Academix.Application.Common.Models;
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
    public class GetTeacherQuery : IRequest<Result<List<TeacherDto>>>
    {
        public Status Status { get; set; }
    }
}
