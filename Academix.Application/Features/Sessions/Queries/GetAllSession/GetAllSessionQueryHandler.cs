using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Sessions.Queries.GetAllSessionForTeacher;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Sessions.Queries.GetAllSession
{
    internal class GetAllSessionQueryHandler : IRequestHandler<GetAllSessionQuery,Result<SessionPageResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public GetAllSessionQueryHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result<SessionPageResult>> Handle(GetAllSessionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get base query for teachers with includes
                var sessionQuery = await _unitOfWork.Repository<SessionRequest>()
                    .GetAllAsync();
              var x =  sessionQuery.ToList();
                var baseQuery = sessionQuery
                    .Include(s => s.Teacher)
                        .ThenInclude(s=>s.User)
                    .Include(s => s.Student)
                        .ThenInclude(s => s.User);

                // Get total count before pagination
                var totalCount = await baseQuery.CountAsync(cancellationToken);

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var skip = (request.PageNumber - 1) * request.PageSize;

                var sessions = await baseQuery
                    .Skip(skip)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var sessionDtos = new List<SessionDto>();

                foreach (var session in sessions)
                {
                    var dto = new SessionDto
                    {
                        StudentId = session.StudentId,
                        TeacherId = session.TeacherId,
                        SessionRequestId = session.Id,
                        StudentName = session.Student.User.FirstName + " " + session.Student.User.LastName,
                        TeacherName = session.Teacher.User.FirstName + " " + session.Teacher.User.LastName,
                        Subject = session.Subject,
                        Description = session.Description,
                        PointsAmount = session.PointsAmount,

                        ScheduledStartTime = session.RequestedDateTime,
                        PlannedDurationMinutes = session.EstimatedDurationMinutes,
                       

                        
                    };

                    sessionDtos.Add(dto);
                }



                // Create paged result
                var result = new SessionPageResult
                {
                    Sessions = sessionDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = request.PageNumber > 1,
                    HasNextPage = request.PageNumber < totalPages
                };

                return Result<SessionPageResult>.Success(result);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("Failed") + $": {ex.Message}";
                return Result<SessionPageResult>.Failure(error);
            }
        }

    }
}
