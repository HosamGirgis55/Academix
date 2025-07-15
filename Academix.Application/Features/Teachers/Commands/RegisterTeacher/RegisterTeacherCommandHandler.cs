using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Common.Extensions;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Academix.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Academix.Application.Features.Teachers.Commands.RegisterTeacher
{
    public class RegisterTeacherCommandHandler : IRequestHandler<RegisterTeacherCommand, Result<AuthenticationResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;
        private readonly IEmailService _emailService;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public RegisterTeacherCommandHandler(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            IEmailService emailService,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
            _emailService = emailService;
            _jwtSettings = jwtSettings;
        }

        public async Task<Result<AuthenticationResult>> Handle(RegisterTeacherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if user exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Result<AuthenticationResult>.Failure(_localizationService.GetLocalizedString("UserAlreadyExists"));
                }

                // Check if country exists
                var countryExists = await _unitOfWork.Repository<Country>()
                    .GetByIdAsync(request.CountryId);

                if (countryExists == null)
                {
                    return Result<AuthenticationResult>.Failure(_localizationService.GetLocalizedString("CountryNotFound"));
                }

                // Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = (Gender)request.Gender,
                    CountryId = request.CountryId,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result<AuthenticationResult>.Failure($"{_localizationService.GetLocalizedString("UserCreationFailed")}: {errors}");
                }

                // Add to Teacher role
                await _userManager.AddToRoleAsync(user, "Teacher");

                // Create Teacher
                var teacher = new Teacher
                {
                    UserId = user.Id,
                    User = user,
                    Bio = request.Bio,
                    ProfilePictureUrl = request.ProfilePictureUrl,
                    AdditionalInterests = request.AdditionalInterests,
                    CountryId = request.CountryId,
                    Educations = request.Educations.Select(e => new TeacherEducation
                    {
                        Institution = e.Institution,
                        Degree = e.Degree,
                        Field = e.Field,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Description = e.Description
                    }).ToList(),
                    Certificates = request.Certificates.Select(c => new Certificate
                    {
                        Name = c.Name,
                        CertificateUrl = c.CertificateUrl,
                        IssuedBy = c.IssuedBy,
                        IssuedDate = c.IssuedDate,
                        ExamResult = c.ExamResult
                    }).ToList(),
                    Salary = request.Salary,
                };

                // Add Teaching Areas
                foreach (var areaId in request.TeachingAreaIds)
                {
                    teacher.TeacherTeachingAreas.Add(new TeacherTeachingArea
                    {
                        TeacherId = teacher.Id,
                        TeachingAreaId = areaId
                    });
                }

                // Add Age Groups
                foreach (var ageGroupId in request.AgeGroupIds)
                {
                    teacher.TeacherAgeGroups.Add(new TeacherAgeGroup
                    {
                        TeacherId = teacher.Id,
                        AgeGroupId = ageGroupId
                    });
                }

                // Add Communication Methods
                foreach (var methodId in request.CommunicationMethodIds)
                {
                    teacher.TeacherCommunicationMethods.Add(new TeacherCommunicationMethod
                    {
                        TeacherId = teacher.Id,
                        CommunicationMethodId = methodId
                    });
                }

                // Add Teaching Languages
                foreach (var languageId in request.TeachingLanguageIds)
                {
                    teacher.TeacherTeachingLanguages.Add(new TeacherTeachingLanguage
                    {
                        TeacherId = teacher.Id,
                        TeachingLanguageId = languageId
                    });
                }

                // Add Skills
                foreach (var skillDto in request.Skills)
                {
                    teacher.Skills.Add(new TeacherSkill
                    {
                        TeacherId = teacher.Id,
                        SkillId = skillDto.SkillId
                        
                    });
                }

                // Save teacher first to get the ID
                await _unitOfWork.Repository<Teacher>().AddAsync(teacher);
                await _unitOfWork.SaveChangesAsync();

                // Add Exams
                foreach (var examDto in request.Exams)
                {
                    var exam = new Exame
                    {
                        Name = examDto.Name,
                        ExamResult = examDto.ExamResult,
                        IssuedBy = examDto.IssuedBy,
                        IssuedDate = examDto.IssuedDate,
                        ExameCertificateUrl = examDto.ExameCertificateUrl,
                        Teacher = teacher
                    };
                    await _unitOfWork.Repository<Exame>().AddAsync(exam);
                }

                await _unitOfWork.SaveChangesAsync();

                // Generate and send email verification OTP
                var otp = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var emailSent = await _emailService.SendRegistrationConfirmationAsync(user.Email!, otp);

                // Generate JWT tokens for automatic login
                var authResult = await _userManager.GenerateTokenAsync(user, _jwtSettings);

                return Result<AuthenticationResult>.Success(authResult, _localizationService.GetLocalizedString("TeacherRegistrationSuccessful"));
            }
            catch (Exception ex)
            {
                return Result<AuthenticationResult>.Failure($"{_localizationService.GetLocalizedString("TeacherRegistrationFailed")}: {ex.Message}");
            }
        }
    }
} 