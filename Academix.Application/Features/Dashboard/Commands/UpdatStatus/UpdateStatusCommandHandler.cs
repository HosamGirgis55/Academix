using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Dashboard.Commands.UpdateSpecialization;
using Academix.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Commands.UpdatStatus
{
    internal class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, Result>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public UpdateStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get existing specialization
                var Teacher = await _unitOfWork.Teachers.GetByIdAsync(request.Id);
                if (Teacher == null)
                {
                    var notFoundMessage = _localizationService.GetLocalizedString("TeacherNotFound");
                    return Result.Failure(notFoundMessage);
                }


                // Update specialization
                Teacher.Status = request.Status;

                _unitOfWork.Teachers.Update(Teacher);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var successMessage = _localizationService.GetLocalizedString("TeacherUpdatedSuccessfully");
                return Result.Success(successMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = _localizationService.GetLocalizedString("TeacherUpdateFailed");
                return Result.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
