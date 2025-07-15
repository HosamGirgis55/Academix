using Academix.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Commands.AddSkills
{
    public class AddSkillsCommand : IRequest<Result>
    {
        public string NameEn { get; set; } = string.Empty;
    }
}
