using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Academix.Application.Features.Students.Commands.RegisterStudent
{
    public class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;
        private readonly IEmailService _emailService;

        public RegisterStudentCommandHandler(
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

        public async Task<Result> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
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
                    .GetByIdAsync(request.ResidenceCountryId);
                
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
                    Gender = (Gender)request.Gender,
                    CountryId = request.ResidenceCountryId,
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

                // Add to Student role
                await _userManager.AddToRoleAsync(user, "Student");
                var LearningInterestsRequests = new List<LearningInterestsStudent>();
                foreach (var G in request.LearningInterests)
                {
                    LearningInterestsRequests.Add(new LearningInterestsStudent
                    {
                        LearningInterestId = G.LearningInterestId,

                    });
                }

                // Create Student
                var student = new Student
                {
                    UserId = user.Id,
                    User = user,
                    Bio = request.Bio,
                    Github = request.Github,
                    ConnectProgramming = request.ConnectProgramming,
                    ProfilePictureUrl = request.ProfilePictureUrl,
                    NationalityId = request.NationalityId,
                    ResidenceCountryId = request.ResidenceCountryId,
                    LevelId = request.LevelId,
                    GraduationStatusId = request.GraduationStatusId,
                    SpecialistId = request.SpecialistId,
                    Skills = new List<StudentSkill>(),
                    Experiences = new List<StudentExperience>(),
                    LearningInterests = LearningInterestsRequests
                };

                // Add Skills
                if (request.Skills != null)
                {
                    foreach (var skill in request.Skills)
                    {
                        student.Skills.Add(new StudentSkill
                        {
                            StudentId = student.Id,
                            SkillId = skill.SkillId
                         });
                    }
                }

                // Add Experiences
                if (request.Experiences != null)
                {
                    foreach (var platform in request.Experiences)
                    {
                        student.Experiences.Add(new StudentExperience
                        {
                            StudentId = student.Id,
                            ExperienceId = platform.Id,
                         
                        });
                    }
                }

                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.SaveChangesAsync();

                // Generate and send email verification OTP
                var otp = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var emailSent = await _emailService.SendRegistrationConfirmationAsync(user.Email!, otp);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"{_localizationService.GetLocalizedString("RegistrationFailed")}: {ex.Message}");
            }
        }
    }
} 