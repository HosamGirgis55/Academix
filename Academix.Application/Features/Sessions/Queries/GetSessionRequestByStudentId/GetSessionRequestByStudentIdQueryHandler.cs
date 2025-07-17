using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Teachers.Query.GetAll;
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

namespace Academix.Application.Features.Sessions.Queries.GetSessionRequestByStudentId
{
    internal class GetSessionRequestByStudentIdQueryHandler : IRequestHandler<GetSessionRequestByStudentIdQuery ,Result<SessionRequestPageResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public GetSessionRequestByStudentIdQueryHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result<SessionRequestPageResult>> Handle(GetSessionRequestByStudentIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get base query for teachers with includes
                var sessionRequestQuery = await _unitOfWork.Repository<SessionRequest>()
                    .GetAllAsync();

                var baseQuery = sessionRequestQuery
                    .Include(s => s.Teacher)
                    .Include(s => s.Student)
                        .ThenInclude(s => s.User)
                    .Where(s => s.Status == Domain.Enums.SessionRequestStatus.Pending&& s.StudentId == request.StudentId);

                // Get total count before pagination
                var totalCount = await baseQuery.CountAsync(cancellationToken);

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var skip = (request.PageNumber - 1) * request.PageSize;

                var sessionRequests = await baseQuery
                    .Skip(skip)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var sessionRequestDtos = new List<SessionRequestDto>();

                foreach (var sessionRequest in sessionRequests)
                {
                    var dto = new SessionRequestDto
                    {
                        Id = sessionRequest.Id,
                        StudentId = sessionRequest.StudentId,
                        StudentName = $"{sessionRequest.Student.User.FirstName} {sessionRequest.Student.User.LastName}",
                        TeacherId = sessionRequest.TeacherId,

                        PointsAmount = sessionRequest.PointsAmount,
                        Subject = sessionRequest.Subject,
                        Description = sessionRequest.Description,
                        EstimatedDurationMinutes = sessionRequest.EstimatedDurationMinutes,
                        RequestedDateTime = sessionRequest.RequestedDateTime,

                        Status = sessionRequest.Status,
                        AcceptedAt = sessionRequest.AcceptedAt,
                        RejectedAt = sessionRequest.RejectedAt,
                        RejectionReason = sessionRequest.RejectionReason
                    };

                    sessionRequestDtos.Add(dto);
                }



                // Create paged result
                var result = new SessionRequestPageResult
                {
                    SessionRequest = sessionRequestDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = request.PageNumber > 1,
                    HasNextPage = request.PageNumber < totalPages
                };

                return Result<SessionRequestPageResult>.Success(result);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("Failed") + $": {ex.Message}";
                return Result<SessionRequestPageResult>.Failure(error);
            }
        }

    }
}
