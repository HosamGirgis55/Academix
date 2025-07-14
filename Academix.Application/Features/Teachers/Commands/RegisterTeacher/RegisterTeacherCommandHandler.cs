using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Academix.Domain.Interfaces;

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
                // Check if user exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("UserAlreadyExists"));
                }

                // Check if country exists
                var countryExists = await _unitOfWork.Repository<Country>()
                    .GetByIdAsync(request.CountryId);

                if (countryExists == null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("CountryNotFound"));
                }

                // Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Gender = (Gender)request.Gender,
                    CountryId = request.CountryId,
                    ProfilePictureUrl = request.ProfilePictureUrl,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result.Failure($"{_localizationService.GetLocalizedString("UserCreationFailed")}: {errors}");
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

                await _unitOfWork.Repository<Teacher>().AddAsync(teacher);
                await _unitOfWork.SaveChangesAsync();

                // Generate and send email verification OTP
                var otp = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var emailSent = await _emailService.SendRegistrationConfirmationAsync(user.Email!, otp);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"{_localizationService.GetLocalizedString("TeacherRegistrationFailed")}: {ex.Message}");
            }
        }
    }
} 