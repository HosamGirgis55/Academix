using Academix.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Commands.DeletSkill
{
    public class DeleteSkillCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

    }
}
