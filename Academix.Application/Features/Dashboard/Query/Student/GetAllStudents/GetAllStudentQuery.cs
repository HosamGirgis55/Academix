using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Query.Student.GetAllStudents
{
    public class GetAllStudentQuery : IRequest<Result<StudentsPagedResult>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
