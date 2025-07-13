using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Teachers.Query.GetById
{
    internal class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, Result<TeacherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public GetTeacherByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result<TeacherDto>> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.Id);

                if (teacher == null)
                {
                    var notFoundMsg = _localizationService.GetLocalizedString("TeacherNotFound");
                    return Result<TeacherDto>.Failure(notFoundMsg);
                }

                var dto = new TeacherDto
                {
                    Id = teacher.Id,
                    FirstName = teacher.User.FirstName,
                    LastName = teacher.User.LastName,
                    Bio = teacher.Bio,
                    ProfilePictureUrl = teacher.ProfilePictureUrl,
                    //Skills = teacher.Skills.Select(s => s.Name).ToList()
                };

                return Result<TeacherDto>.Success(dto);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("TeacherGetByIdFailed") + $": {ex.Message}";
                return Result<TeacherDto>.Failure(error);
            }
        }
    }
}
