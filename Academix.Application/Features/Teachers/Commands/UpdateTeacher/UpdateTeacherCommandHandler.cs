using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Teachers.Commands.UpdateTeacher
{
    internal class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateTeacherCommandHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
            _userManager = userManager;
        }

        public async Task<Result> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Get teacher by user ID with all related entities
                var teacherQuery = await _unitOfWork.Repository<Teacher>()
                    .GetAllAsync();
                    
                var teacher = await teacherQuery
                    .Include(t => t.User)
                    .Include(t => t.Skills)
                    .Include(t => t.TeacherTeachingAreas)
                    .Include(t => t.TeacherAgeGroups)
                    .Include(t => t.TeacherCommunicationMethods)
                    .Include(t => t.TeacherTeachingLanguages)
                    .FirstOrDefaultAsync(t => t.UserId == request.UserId, cancellationToken);

                if (teacher == null)
                {
                    var notFoundMessage = _localizationService.GetLocalizedString("TeacherNotFound");
                    return Result.Failure(notFoundMessage);
                }

                // 2. Check if country exists
                var countryExists = await _unitOfWork.Repository<Country>()
                    .AnyAsync(c => c.Id == request.CountryId);
                if (!countryExists)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("InvalidCountry"));
                }

                // 3. Update User properties
                teacher.User.FirstName = request.FirstName;
                teacher.User.LastName = request.LastName;
                teacher.User.ProfilePictureUrl = request.ProfilePictureUrl;
                teacher.User.CountryId = request.CountryId;
                teacher.User.UpdatedAt = DateTime.UtcNow;
                
                if (Enum.TryParse<Gender>(request.Gender, true, out var gender))
                {
                    teacher.User.Gender = gender;
                }

                // Update user through UserManager
                var userUpdateResult = await _userManager.UpdateAsync(teacher.User);
                if (!userUpdateResult.Succeeded)
                {
                    var errors = string.Join(", ", userUpdateResult.Errors.Select(e => e.Description));
                    return Result.Failure($"User update failed: {errors}");
                }

                // 4. Update Teacher-specific properties
                teacher.Bio = request.Bio;
                teacher.ProfilePictureUrl = request.ProfilePictureUrl ?? string.Empty;
                teacher.Salary = request.Salary;
                teacher.AdditionalInterests = request.AdditionalInterests;
                teacher.CountryId = request.CountryId;
                teacher.UpdatedAt = DateTime.UtcNow;

                // 5. Update Educations (owned entities)
                teacher.Educations.Clear();
                foreach (var edu in request.Educations)
                {
                    teacher.Educations.Add(new TeacherEducation
                    {
                        Institution = edu.Institution,
                        Degree = edu.Degree,
                        Field = edu.Field,
                        StartDate = edu.StartDate,
                        EndDate = edu.EndDate,
                        Description = edu.Description
                    });
                }

                // 6. Update Certificates (owned entities)
                teacher.Certificates.Clear();
                foreach (var cert in request.Certificates)
                {
                    teacher.Certificates.Add(new Certificate
                    {
                        Name = cert.Name,
                        CertificateUrl = cert.CertificateUrl,
                        IssuedBy = cert.IssuedBy,
                        IssuedDate = cert.IssuedDate,
                        ExamResult = cert.ExamResult
                    });
                }

                // 7. Update Skills - remove existing and add new ones
                if (teacher.Skills != null)
                {
                    foreach (var skill in teacher.Skills)
                    {
                        _unitOfWork.Repository<TeacherSkill>().Delete(skill);
                    }
                }

                foreach (var skill in request.Skills)
                {
                    // Check if skill exists
                    var skillExists = await _unitOfWork.Repository<Skill>()
                        .AnyAsync(s => s.Id == skill.SkillId);
                    if (skillExists)
                    {
                        await _unitOfWork.Repository<TeacherSkill>().AddAsync(new TeacherSkill
                        {
                            TeacherId = teacher.Id,
                            SkillId = skill.SkillId
                        });
                    }
                }

                // 8. Update Teaching Areas
                foreach (var area in teacher.TeacherTeachingAreas)
                {
                    _unitOfWork.Repository<TeacherTeachingArea>().Delete(area);
                }

                foreach (var areaId in request.TeachingAreaIds)
                {
                    var areaExists = await _unitOfWork.Repository<TeachingArea>()
                        .AnyAsync(a => a.Id == areaId);
                    if (areaExists)
                    {
                        await _unitOfWork.Repository<TeacherTeachingArea>().AddAsync(new TeacherTeachingArea
                        {
                            TeacherId = teacher.Id,
                            TeachingAreaId = areaId
                        });
                    }
                }

                // 9. Update Age Groups
                foreach (var ageGroup in teacher.TeacherAgeGroups)
                {
                    _unitOfWork.Repository<TeacherAgeGroup>().Delete(ageGroup);
                }

                foreach (var ageGroupId in request.AgeGroupIds)
                {
                    var ageGroupExists = await _unitOfWork.Repository<AgeGroup>()
                        .AnyAsync(a => a.Id == ageGroupId);
                    if (ageGroupExists)
                    {
                        await _unitOfWork.Repository<TeacherAgeGroup>().AddAsync(new TeacherAgeGroup
                        {
                            TeacherId = teacher.Id,
                            AgeGroupId = ageGroupId
                        });
                    }
                }

                // 10. Update Communication Methods
                foreach (var commMethod in teacher.TeacherCommunicationMethods)
                {
                    _unitOfWork.Repository<TeacherCommunicationMethod>().Delete(commMethod);
                }

                foreach (var commMethodId in request.CommunicationMethodIds)
                {
                    var commMethodExists = await _unitOfWork.Repository<CommunicationMethod>()
                        .AnyAsync(c => c.Id == commMethodId);
                    if (commMethodExists)
                    {
                        await _unitOfWork.Repository<TeacherCommunicationMethod>().AddAsync(new TeacherCommunicationMethod
                        {
                            TeacherId = teacher.Id,
                            CommunicationMethodId = commMethodId
                        });
                    }
                }

                // 11. Update Teaching Languages
                foreach (var teachingLang in teacher.TeacherTeachingLanguages)
                {
                    _unitOfWork.Repository<TeacherTeachingLanguage>().Delete(teachingLang);
                }

                foreach (var langId in request.TeachingLanguageIds)
                {
                    var langExists = await _unitOfWork.Repository<TeachingLanguage>()
                        .AnyAsync(l => l.Id == langId);
                    if (langExists)
                    {
                        await _unitOfWork.Repository<TeacherTeachingLanguage>().AddAsync(new TeacherTeachingLanguage
                        {
                            TeacherId = teacher.Id,
                            TeachingLanguageId = langId
                        });
                    }
                }

                // 12. Update teacher entity
                _unitOfWork.Repository<Teacher>().Update(teacher);

                // 13. Save all changes
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var successMessage = _localizationService.GetLocalizedString("TeacherProfileUpdatedSuccessfully");
                return Result.Success(successMessage);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("TeacherUpdateFailed") + $": {ex.Message}";
                return Result.Failure(error);
            }
        }
    }
}
