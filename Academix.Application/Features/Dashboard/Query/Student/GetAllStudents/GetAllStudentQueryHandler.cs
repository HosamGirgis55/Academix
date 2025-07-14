using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Academix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Query.Student.GetAllStudents
{
    internal class GetAllStudentQueryHandler : IRequestHandler<GetAllStudentQuery, Result<List<StudentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;
        private readonly ICommentRepository _commentRepository;

        public GetAllStudentQueryHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            ICommentRepository commentRepository)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
            _commentRepository = commentRepository;
        }

        public async Task<Result<List<StudentDto>>> Handle(GetAllStudentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var students = await _unitOfWork.Repository<Academix.Domain.Entities.Student>()
                    .GetAllAsync();

                if (students == null || !students.Any())
                {
                    var notFoundMsg = _localizationService.GetLocalizedString("empty");
                    return Result<List<StudentDto>>.Failure(notFoundMsg);
                }

                var studentDtos = students.Select(s => new StudentDto
                {
                    Id = s.Id,
                    FirstName = s.User.FirstName,
                    LastName = s.User.LastName,
                    Email = s.User.Email,
                    PhoneNumber = s.User.PhoneNumber,
                }).ToList();

                return Result<List<StudentDto>>.Success(studentDtos);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("StudentGetByIdFailed") + $": {ex.Message}";
                return Result<List<StudentDto>>.Failure(error);
            }
        }





    }
}
