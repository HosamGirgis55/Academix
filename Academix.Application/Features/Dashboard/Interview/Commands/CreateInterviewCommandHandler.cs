using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Interview.Commands
{
    internal class CreateInterviewCommandHandler : IRequestHandler<CreateInterViewCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;
        private readonly IEmailService _emailService;

        public CreateInterviewCommandHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
            _emailService = emailService;
        }


        public async Task<Result> Handle(CreateInterViewCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var interview = new Domain.Entities.Interview
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    Name = request.Name,
                    Date = request.Date,
                    Time = request.Time,
                    Link = request.Link,
                    TeacherId = request.TeacherId
                };

                await _unitOfWork.Interviews.AddAsync(interview);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                string subject = "دعوة لحضور مقابلة";
                string body = $@"
                <p>مرحبًا {request.Name}،</p>
                <p>تم تحديد مقابلة لك بتاريخ <strong>{request.Date:yyyy-MM-dd}</strong> في الساعة <strong>{request.Time}</strong>.</p>
                <p>رابط المقابلة: <a href='{request.Link}'>{request.Link}</a></p>
                <p>نتمنى لك التوفيق!</p>";

                await _emailService.SendEmailAsync(request.Email, subject, body);

                var successMessage = _localizationService.GetLocalizedString("InterviewCreatedSuccessfully");
                return Result.Success(successMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = _localizationService.GetLocalizedString("InterviewCreateFailed");
                return Result.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
