using System;
using System.Threading;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Sessions.Commands.AcceptSessionRequest
{
    public class AcceptSessionRequestCommandHandler : IRequestHandler<AcceptSessionRequestCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseNotificationService _firebaseService;
        private readonly ILocalizationService _localizationService;

        public AcceptSessionRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IFirebaseNotificationService firebaseService,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _localizationService = localizationService;
        }

        public async Task<Result<Guid>> Handle(AcceptSessionRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get session request with related entities
                var sessionRequestQuery = await _unitOfWork.Repository<SessionRequest>().GetAllAsync();
                var sessionRequest = await sessionRequestQuery
                    .Include(sr => sr.Student)
                        .ThenInclude(s => s.User)
                    .Include(sr => sr.Teacher)
                        .ThenInclude(t => t.User)
                    .FirstOrDefaultAsync(sr => sr.Id == request.SessionRequestId, cancellationToken);

                if (sessionRequest == null)
                {
                    return Result<Guid>.Failure(_localizationService.GetLocalizedString("SessionRequestNotFound"));
                }

                // Verify the teacher is authorized to accept this request
                if (sessionRequest.TeacherId != request.TeacherId)
                {
                    return Result<Guid>.Failure(_localizationService.GetLocalizedString("UnauthorizedToAcceptRequest"));
                }

                // Check if request is still pending
                if (sessionRequest.Status != SessionRequestStatus.Pending)
                {
                    return Result<Guid>.Failure(_localizationService.GetLocalizedString("RequestAlreadyProcessed"));
                }

                // Begin transaction
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Update session request status
                    sessionRequest.Status = SessionRequestStatus.Accepted;
                    sessionRequest.AcceptedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<SessionRequest>().Update(sessionRequest);

                    // Create the session
                    var session = new Session
                    {
                        StudentId = sessionRequest.StudentId,
                        TeacherId = sessionRequest.TeacherId,
                        SessionRequestId = sessionRequest.Id,
                        Subject = sessionRequest.Subject,
                        Description = sessionRequest.Description,
                        PointsAmount = sessionRequest.PointsAmount,
                        ScheduledStartTime = request.ScheduledStartTime,
                        PlannedDurationMinutes = sessionRequest.EstimatedDurationMinutes,
                        Status = SessionStatus.Scheduled,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Repository<Session>().AddAsync(session);

                    // Link session to request
                    sessionRequest.SessionId = session.Id;
                    _unitOfWork.Repository<SessionRequest>().Update(sessionRequest);

                    await _unitOfWork.SaveChangesAsync();

                    // Send notification to student
                    if (!string.IsNullOrEmpty(sessionRequest.Student.User.DeviceToken))
                    {
                        var teacherName = $"{sessionRequest.Teacher.User.FirstName} {sessionRequest.Teacher.User.LastName}";
                        await _firebaseService.SendSessionAcceptedNotificationAsync(
                            sessionRequest.Student.User.DeviceToken,
                            teacherName,
                            request.ScheduledStartTime);
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    return Result<Guid>.Success(session.Id, 
                        _localizationService.GetLocalizedString("SessionRequestAcceptedSuccessfully"));
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"{_localizationService.GetLocalizedString("AcceptSessionRequestFailed")}: {ex.Message}");
            }
        }
    }
} 