using Academix.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Interview.Commands
{
    public class CreateInterViewCommand : IRequest<Result>
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Link { get; set; }

        public TimeSpan Time { get; set; }
        public Guid TeacherId { get; set; }

    }
}
