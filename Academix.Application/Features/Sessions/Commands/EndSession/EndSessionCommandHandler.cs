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

namespace Academix.Application.Features.Sessions.Commands.EndSession
{
    public class EndSessionCommandHandler : IRequestHandler<EndSessionCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseNotificationService _firebaseService;
        private readonly ILocalizationService _localizationService;

        public EndSessionCommandHandler(
            IUnitOfWork unitOfWork,
            IFirebaseNotificationService firebaseService,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _localizationService = localizationService;
        }

        public async Task<Result> Handle(EndSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get session with related entities
                var sessionQuery = await _unitOfWork.Repository<Session>().GetAllAsync();
                var session = await sessionQuery
                    .Include(s => s.Student)
                        .ThenInclude(s => s.User)
                    .Include(s => s.Teacher)
                        .ThenInclude(t => t.User)
                    .Include(s => s.SessionRequest)
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (session == null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("SessionNotFound"));
                }

                // Check if session is in progress or scheduled
                if (session.Status != SessionStatus.InProgress && session.Status != SessionStatus.Scheduled)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("SessionCannotBeEnded"));
                }

                // Check if points have already been transferred
                if (session.IsPointsTransferred)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("PointsAlreadyTransferred"));
                }

                // Begin transaction for atomic operations
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Update session details
                    session.Status = SessionStatus.Completed;
                    session.ActualEndTime = DateTime.UtcNow;
                    session.TeacherNotes = request.TeacherNotes;
                    session.StudentNotes = request.StudentNotes;
                    session.StudentRating = request.StudentRating;
                    session.TeacherRating = request.TeacherRating;

                    // Calculate actual duration if session was started
                    if (session.ActualStartTime.HasValue)
                    {
                        session.ActualDurationMinutes = (int)(session.ActualEndTime.Value - session.ActualStartTime.Value).TotalMinutes;
                    }

                    // Transfer points from student to teacher
                    session.Teacher.Points += session.PointsAmount;
                    session.IsPointsTransferred = true;
                    session.PointsTransferredAt = DateTime.UtcNow;

                    // Update session request status
                    if (session.SessionRequest != null)
                    {
                        session.SessionRequest.Status = SessionRequestStatus.Completed;
                        _unitOfWork.Repository<SessionRequest>().Update(session.SessionRequest);
                    }

                    // Save all changes
                    _unitOfWork.Repository<Session>().Update(session);
                    _unitOfWork.Teachers.Update(session.Teacher);
                    await _unitOfWork.SaveChangesAsync();

                    // Send notifications to both participants
                    if (!string.IsNullOrEmpty(session.Student.User.DeviceToken))
                    {
                        var teacherName = $"{session.Teacher.User.FirstName} {session.Teacher.User.LastName}";
                        await _firebaseService.SendSessionEndedNotificationAsync(
                            session.Student.User.DeviceToken,
                            teacherName);
                    }

                    if (!string.IsNullOrEmpty(session.Teacher.User.DeviceToken))
                    {
                        var studentName = $"{session.Student.User.FirstName} {session.Student.User.LastName}";
                        await _firebaseService.SendSessionEndedNotificationAsync(
                            session.Teacher.User.DeviceToken,
                            studentName);
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    return Result.Success(_localizationService.GetLocalizedString("SessionEndedSuccessfully"));
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Result.Failure($"{_localizationService.GetLocalizedString("EndSessionFailed")}: {ex.Message}");
            }
        }
    }
} 