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
using Microsoft.EntityFrameworkCore;

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
                // Validate all foreign key references first
               

                var residenceCountry = await _unitOfWork.Repository<Country>().GetByIdAsync(request.ResidenceCountryId);
                if (residenceCountry == null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("CountryNotFound"));
                }

                var level = await _unitOfWork.Repository<Level>().GetByIdAsync(request.LevelId);
                if (level == null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("LevelNotFound"));
                }

                var graduationStatus = await _unitOfWork.Repository<GraduationStatus>().GetByIdAsync(request.GraduationStatusId);
                if (graduationStatus == null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("GraduationStatusNotFound"));
                }

                var specialist = await _unitOfWork.Repository<Specialization>().GetByIdAsync(request.SpecialistId);
                if (specialist == null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("SpecializationNotFound"));
                }

                // Validate experiences
                if (request.Experiences != null && request.Experiences.Any())
                {
                    var experienceIds = request.Experiences.Select(e => e.Id).ToList();
                    var experiences = await _unitOfWork.Repository<Experience>().GetAllAsync();
                    var existingExperiences = experiences.Where(e => experienceIds.Contains(e.Id)).ToList();

                    if (existingExperiences.Count() != experienceIds.Count)
                    {
                        return Result.Failure(_localizationService.GetLocalizedString("InvalidExperience"));
                    }
                }

                // Validate skills
                if (request.Skills != null && request.Skills.Any())
                {
                    var skillIds = request.Skills.Select(s => s.SkillId).ToList();
                    var skills = await _unitOfWork.Repository<Skill>().GetAllAsync();
                    var existingSkills = skills.Where(s => skillIds.Contains(s.Id)).ToList();

                    if (existingSkills.Count() != skillIds.Count)
                    {
                        return Result.Failure(_localizationService.GetLocalizedString("InvalidSkill"));
                    }
                }

                // Validate learning interests
                if (request.LearningInterests != null && request.LearningInterests.Any())
                {
                    var learningInterestIds = request.LearningInterests.Select(l => l.LearningInterestId).ToList();
                    var interests = await _unitOfWork.Repository<LearningInterest>().GetAllAsync();
                    var existingInterests = interests.Where(l => learningInterestIds.Contains(l.Id)).ToList();

                    if (existingInterests.Count() != learningInterestIds.Count)
                    {
                        return Result.Failure(_localizationService.GetLocalizedString("InvalidLearningInterest"));
                    }
                }

                // Check if user exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("UserAlreadyExists"));
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

                // Create Student
                var student = new Student
                {
                    UserId = user.Id,
                    User = user,
                    Bio = request.Bio,
                    Github = request.Github,
                    ConnectProgramming = request.ConnectProgramming,
                    ProfilePictureUrl = request.ProfilePictureUrl,
                     ResidenceCountryId = request.ResidenceCountryId,
                    LevelId = request.LevelId,
                    GraduationStatusId = request.GraduationStatusId,
                    SpecialistId = request.SpecialistId
                };

                // Save student first to get the Id
                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.SaveChangesAsync();

                // Now add the related entities
                if (request.Skills != null && request.Skills.Any())
                {
                    foreach (var skill in request.Skills)
                    {
                        var studentSkill = new StudentSkill
                        {
                            StudentId = student.Id,
                            SkillId = skill.SkillId
                        };
                        await _unitOfWork.Repository<StudentSkill>().AddAsync(studentSkill);
                    }
                }

                if (request.Experiences != null && request.Experiences.Any())
                {
                    foreach (var exp in request.Experiences)
                    {
                        var studentExperience = new StudentExperience
                        {
                            StudentId = student.Id,
                            ExperienceId = exp.Id
                        };
                        await _unitOfWork.Repository<StudentExperience>().AddAsync(studentExperience);
                    }
                }

                if (request.LearningInterests != null && request.LearningInterests.Any())
                {
                    foreach (var interest in request.LearningInterests)
                    {
                        var learningInterest = new LearningInterestsStudent
                        {
                            StudentId = student.Id,
                            FieldId = interest.LearningInterestId
                        };
                        await _unitOfWork.Repository<LearningInterestsStudent>().AddAsync(learningInterest);
                    }
                }

                // Save all related entities
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