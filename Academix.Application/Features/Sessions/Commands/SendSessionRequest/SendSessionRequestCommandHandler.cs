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

namespace Academix.Application.Features.Sessions.Commands.SendSessionRequest
{
    public class SendSessionRequestCommandHandler : IRequestHandler<SendSessionRequestCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseNotificationService _firebaseService;
        private readonly ILocalizationService _localizationService;

        public SendSessionRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IFirebaseNotificationService firebaseService,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _localizationService = localizationService;
        }

        public async Task<Result<Guid>> Handle(SendSessionRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get student with user information
                var studentQuery = await _unitOfWork.Students.GetAllAsync();
                var student = await studentQuery
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken);

                if (student == null)
                {
                    return Result<Guid>.Failure(_localizationService.GetLocalizedString("StudentNotFound"));
                }

                // Get teacher with user information
                var teacherQuery = await _unitOfWork.Teachers.GetAllAsync();
                var teacher = await teacherQuery
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == request.TeacherId, cancellationToken);

                if (teacher == null)
                {
                    return Result<Guid>.Failure(_localizationService.GetLocalizedString("TeacherNotFound"));
                }

                // Check if student has enough points
                if (student.Points < request.PointsAmount)
                {
                    return Result<Guid>.Failure(_localizationService.GetLocalizedString("InsufficientPoints"));
                }

                // Check if teacher is available (accepted status)
                if (teacher.Status != Status.Accepted)
                {
                    return Result<Guid>.Failure(_localizationService.GetLocalizedString("TeacherNotAvailable"));
                }

                // Begin transaction for atomicity
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Deduct points from student (hold them in pending state)
                    student.Points -= request.PointsAmount;
                    _unitOfWork.Students.Update(student);

                    // Create session request
                    var sessionRequest = new SessionRequest
                    {
                        StudentId = request.StudentId,
                        TeacherId = request.TeacherId,
                        PointsAmount = request.PointsAmount,
                        Subject = request.Subject,
                        Description = request.Description,
                        EstimatedDurationMinutes = request.EstimatedDurationMinutes,
                        RequestedDateTime = request.RequestedDateTime,
                        Status = SessionRequestStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Repository<SessionRequest>().AddAsync(sessionRequest);
                    await _unitOfWork.SaveChangesAsync();

                    // Send Firebase notification to teacher
                    if (!string.IsNullOrEmpty(teacher.User.DeviceToken))
                    {
                        var studentName = $"{student.User.FirstName} {student.User.LastName}";
                        await _firebaseService.SendSessionRequestNotificationAsync(
                            teacher.User.DeviceToken,
                            studentName,
                            request.Subject,
                            sessionRequest.Id);
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    return Result<Guid>.Success(sessionRequest.Id, 
                        _localizationService.GetLocalizedString("SessionRequestSentSuccessfully"));
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"{_localizationService.GetLocalizedString("SessionRequestFailed")}: {ex.Message}");
            }
        }
    }
} 