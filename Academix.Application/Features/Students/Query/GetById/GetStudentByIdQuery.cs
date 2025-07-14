using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Students.Query.GetById
{
    public class GetStudentByIdQuery : IRequest<Result<StudentDto>>
    {
        public Guid Id { get; set; }
    }
}
