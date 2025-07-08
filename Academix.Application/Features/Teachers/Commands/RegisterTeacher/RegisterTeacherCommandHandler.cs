using System;
using System.Threading;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Academix.Application.Features.Teachers.Commands.RegisterTeacher
{
    public class RegisterTeacherCommandHandler : IRequestHandler<RegisterTeacherCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;
        private readonly IEmailService _emailService;

        public RegisterTeacherCommandHandler(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
            _emailService = emailService;
        }

        public async Task<Result> Handle(RegisterTeacherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender =  request.Gender,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("RegistrationFailed"));
                }

                // Add to Teacher role
                await _userManager.AddToRoleAsync(user, "Teacher");

                // Create Teacher
                var teacher = new Teacher
                {
                    UserId = user.Id,
                    User = user,
                    
                     ProfilePictureUrl = request.ProfilePictureUrl,
                    NationalityId = request.NationalityId,
                    CountryId = request.CountryId,
                    Certificates = request.Certificates.ConvertAll(c => new TeacherCertificate
                    {
                        Name = c.Name,
                        CertificateUrl = c.CertificateUrl,
                        Description = c.Description,
                        IssuedDate = c.IssuedDate,
                        IssuedBy = c.IssuedBy
                    }),
                    Educations = request.Educations.ConvertAll(e => new TeacherEducation
                    {
                        Degree = e.Degree,
                        Institution = e.Institution,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        FieldOfStudy = e.FieldOfStudy
                    }),
                    Exames = request.TeacherExams.ConvertAll(e => new Exame
                    {
                        Name = e.Name,
                        ExameCertificateUrl = e.ExameCertificateUrl,
                        ExamResult = e.ExamResult,
                        IssuedBy = e.IssuedBy,
                        IssuedDate = e.IssuedDate,
                        
                    })
                };

                await _unitOfWork.Repository<Teacher>().AddAsync(teacher);
                await _unitOfWork.SaveChangesAsync();

                // Send verification email
                var otp = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.UpdateAsync(user);

                var emailSent = await _emailService.SendRegistrationConfirmationAsync(user.Email!, otp);

                 

                return Result.Success(_localizationService.GetLocalizedString("RegistrationSuccessful"));
            }
            catch (Exception)
            {
                return Result.Failure(_localizationService.GetLocalizedString("RegistrationFailed"));
            }
        }
    }
} 