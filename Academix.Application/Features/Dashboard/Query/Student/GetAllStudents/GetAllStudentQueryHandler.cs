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
    internal class GetAllStudentQueryHandler : IRequestHandler<GetAllStudentQuery, Result<StudentsPagedResult>>
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

        public async Task<Result<StudentsPagedResult>> Handle(GetAllStudentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate pagination parameters
                if (request.PageNumber < 1)
                    request.PageNumber = 1;
                
                if (request.PageSize < 1)
                    request.PageSize = 10;
                
                if (request.PageSize > 100)
                    request.PageSize = 100;

                // Get all students
                var studentsQueryable = await _unitOfWork.Repository<Academix.Domain.Entities.Student>()
                    .GetAllAsync();

                var allStudents = studentsQueryable.Include(x=>x.User).ToList();

                // Get total count for pagination
                var totalCount = allStudents.Count;

                if (totalCount == 0)
                {
                    var notFoundMsg = _localizationService.GetLocalizedString("empty");
                    return Result<StudentsPagedResult>.Failure(notFoundMsg);
                }

                // Calculate pagination values
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var skipCount = (request.PageNumber - 1) * request.PageSize;

                // Apply pagination
                var paginatedStudents = allStudents
                    .Skip(skipCount)
                    .Take(request.PageSize)
                    .ToList();

                var studentDtos = paginatedStudents.Select(s => new StudentDto
                {
                    Id = s.Id,
                    FirstName = s?.User?.FirstName,
                    LastName = s?.User?.LastName,
                    Email = s.User.Email,
                    BirthDate = s.BirthDate,
                }).ToList();

                var result = new StudentsPagedResult
                {
                    Students = studentDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = request.PageNumber > 1,
                    HasNextPage = request.PageNumber < totalPages
                };

                return Result<StudentsPagedResult>.Success(result);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("StudentGetByIdFailed") + $": {ex.Message}";
                return Result<StudentsPagedResult>.Failure(error);
            }
        }
    }
}
