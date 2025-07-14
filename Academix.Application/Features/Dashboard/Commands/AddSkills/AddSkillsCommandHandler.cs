using Academix.Application.Common.Models;
using Academix.Application.Features.Students.Commands.RegisterStudent;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Commands.AddSkills
{
    public class AddSkillsCommandHandler : IRequestHandler<AddSkillsCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddSkillsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddSkillsCommand request, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Skills
                .AnyAsync(s => s.NameAr.ToLower() == request.Name.ToLower());

            if (exists)
                return Result.Failure("Skill already exists");

            var skill = new Skill
            {
                NameAr = request.Name
            };

            await _unitOfWork.Skills.AddAsync(skill);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }

}
