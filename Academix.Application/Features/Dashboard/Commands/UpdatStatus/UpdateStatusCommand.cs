using Academix.Application.Common.Models;
using Academix.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Commands.UpdatStatus
{
    public class UpdateStatusCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Status Status { get; set; }
    }
}
