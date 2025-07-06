using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Students.Commands.RegisterStudent
{
    public class RegisterStudentCommandHandler : ICommandHandler<RegisterStudentCommand, Result<StudentRegistrationResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ILocalizationService _localizationService;

        public RegisterStudentCommandHandler(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ILocalizationService localizationService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _localizationService = localizationService;
        }

        public async Task<Result<StudentRegistrationResponse>> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Result<StudentRegistrationResponse>.Failure(_localizationService.GetLocalizedString("UserAlreadyExists"));
                }

                // Check if country exists
                var countryExists = await _unitOfWork.Repository<Country>()
                    .GetByIdAsync(request.CountryId);
                
                if (countryExists == null)
                {
                    return Result<StudentRegistrationResponse>.Failure(_localizationService.GetLocalizedString("CountryNotFound"));
                }

                // Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    NationalityId = request.NatinalityId,
                    gender = request.Gender,
                    CountryId = request.CountryId,
                    ProfilePictureUrl = request.ProfilePictureUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    EmailConfirmed = false // Email confirmation required via OTP
                };

                // Create user with password
                var result = await _userManager.CreateAsync(user, request.Password);
                
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result<StudentRegistrationResponse>.Failure($"{_localizationService.GetLocalizedString("UserCreationFailed")}: {errors}");
                }
                List<ProblemSolving> problemSolving = new List<ProblemSolving>();
                foreach (var problem in request.ProblemSolveing ?? new List<ProblemSolveingDTO>())
                {
                    problemSolving.Add(new ProblemSolving
                    {
                        Id = problem.Id,

                    });
                }

              
                // Create Student entity
                var student = new Student
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,

                    UpdatedAt = DateTime.UtcNow,
                   Level = request.LevelId,
                    GraduationStatus = request.GraduationStatusId,
                    SpecialistId = request.SpecialistId,   
                    Bio = request.Bio,
                    Github = request.Github,
                    ConnectPrograming = request.ConnectPrograming,
                    ProblemSolveing = problemSolving,
                    




                };
                
                // Add student to repository
                await _unitOfWork.Repository<Student>().AddAsync(student);
                await _unitOfWork.SaveChangesAsync();


                List<StudentSkiles> StudentSkill = new List<StudentSkiles>();
                foreach (var problem in request.Skills ?? new List<StudentSkillDTO>())
                {
                    await _unitOfWork.Repository<StudentSkiles>().AddAsync(new StudentSkiles
                    {
                        StudentId = student.Id,
                        Id = problem.SkillId,

                    });

                   
                }

                // Add user to Student role (create role if needed)
                await _userManager.AddToRoleAsync(user, "Student");

                // Generate and send email verification OTP
                var otp = await _emailService.GenerateOtpAsync(user.Email!, "registration");
                user.EmailVerificationOtp = otp;
                user.EmailVerificationOtpExpiry = DateTime.UtcNow.AddMinutes(15);
                await _userManager.UpdateAsync(user);

                // Send verification email
                var emailSent = await _emailService.SendRegistrationConfirmationAsync(user.Email!, otp);

                var response = new StudentRegistrationResponse
                {
                    UserId = user.Id,
                    StudentId = student.Id,
                    Email = user.Email!,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Message = emailSent 
                        ? _localizationService.GetLocalizedString("StudentRegisteredSuccessfullyCheckEmail")
                        : _localizationService.GetLocalizedString("StudentRegisteredSuccessfully"),
                   
                     RequiresEmailVerification = true,
                  
                };

                return Result<StudentRegistrationResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<StudentRegistrationResponse>.Failure($"{_localizationService.GetLocalizedString("RegistrationFailed")}: {ex.Message}");
            }
        }
    }
} 