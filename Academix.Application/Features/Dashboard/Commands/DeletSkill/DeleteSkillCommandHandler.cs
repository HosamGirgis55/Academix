using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Dashboard.Commands.DeleteSpecialization;
using Academix.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Commands.DeletSkill
{
    public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public DeleteSkillCommandHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var skill = await _unitOfWork.Skills.GetByIdAsync(request.Id);
                if (skill == null)
                {
                    var notFoundMessage = _localizationService.GetLocalizedString("SkillNotFound");
                    return Result.Failure(notFoundMessage);
                }

                // احذف العلاقة من المدرسين
                var teachers = await _unitOfWork.Teachers.GetAllAsync();
                foreach (var teacher in teachers)
                {
                    if (teacher.Skills != null && teacher.Skills.Any(s => s.Id == request.Id))
                    {
                        var skillToRemove = teacher.Skills.First(s => s.Id == request.Id);
                        teacher.Skills.Remove(skillToRemove);
                    }
                }

                // احذف العلاقة من الطلاب
                var students = await _unitOfWork.Students.GetAllAsync();
                foreach (var student in students)
                {
                    if (student.Skills != null && student.Skills.Any(s => s.Id == request.Id))
                    {
                        var skillToRemove = student.Skills.First(s => s.Id == request.Id);
                        student.Skills.Remove(skillToRemove);
                    }
                }

                _unitOfWork.Skills.Delete(skill);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var successMessage = _localizationService.GetLocalizedString("SkillDeletedSuccessfully");
                return Result.Success(successMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = _localizationService.GetLocalizedString("SkillDeleteFailed");
                return Result.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

    }
}
