using Academix.Application.Common.Models;
using Academix.Application.Features.Teachers.Query.GetById;
using Academix.Domain.DTOs;
using MediatR;
using Academix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Academix.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Students.Query.GetById
{
    internal class GetTeacherByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;
        private readonly ICommentRepository _commentRepository;

        public GetTeacherByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            ICommentRepository commentRepository)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
            _commentRepository = commentRepository;
        }

        public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Use generic repository to get student with User navigation property
                var studentQuery = await _unitOfWork.Repository<Student>()
                    .GetAllAsync();
                
                var student = await studentQuery
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == request.Id);

                if (student == null)
                {
                    var notFoundMsg = _localizationService.GetLocalizedString("StudentNotFound");
                    return Result<StudentDto>.Failure(notFoundMsg);
                }

                
                var dto = new StudentDto
                {
                    Id = student.Id,
                    FirstName = student.User.FirstName,
                    LastName = student.User.LastName,
                    ProfilePictureUrl = student.ProfilePictureUrl,
                    Email = student.User.Email,
                    BirthDate = student.BirthDate
                };

                return Result<StudentDto>.Success(dto);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("StudentGetByIdFailed") + $": {ex.Message}";
                return Result<StudentDto>.Failure(error);
            }
        }
    }
}
