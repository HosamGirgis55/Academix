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
                // 1. Get teacher by user ID with all related entities (no tracking to avoid conflicts)
                var teacherQuery = await _unitOfWork.Repository<Teacher>()
                    .GetAllAsync();
                    
                var teacher = await teacherQuery
                    .AsNoTracking()
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

                // 2. Validate country if provided
                if (request.CountryId.HasValue)
                {
                    var countryExists = await _unitOfWork.Repository<Country>()
                        .AnyAsync(c => c.Id == request.CountryId.Value);
                    if (!countryExists)
                    {
                        return Result.Failure(_localizationService.GetLocalizedString("InvalidCountry"));
                    }
                }

                // 3. Update User properties using UserManager (get fresh instance to avoid tracking conflicts)
                bool userNeedsUpdate = false;
                ApplicationUser? userToUpdate = null;
                
                if (!string.IsNullOrWhiteSpace(request.FirstName) ||
                    !string.IsNullOrWhiteSpace(request.LastName) ||
                    request.ProfilePictureUrl != null ||
                    request.CountryId.HasValue ||
                    request.Gender.HasValue)
                {
                    // Get a fresh user instance from UserManager (this handles tracking properly)
                    userToUpdate = await _userManager.FindByIdAsync(request.UserId);
                    if (userToUpdate == null)
                    {
                        return Result.Failure(_localizationService.GetLocalizedString("UserNotFound"));
                    }

                    if (!string.IsNullOrWhiteSpace(request.FirstName))
                    {
                        userToUpdate.FirstName = request.FirstName;
                        userNeedsUpdate = true;
                    }
                    
                    if (!string.IsNullOrWhiteSpace(request.LastName))
                    {
                        userToUpdate.LastName = request.LastName;
                        userNeedsUpdate = true;
                    }
                    
                    if (request.ProfilePictureUrl != null)
                    {
                        userToUpdate.ProfilePictureUrl = request.ProfilePictureUrl;
                        userNeedsUpdate = true;
                    }
                    
                    if (request.CountryId.HasValue)
                    {
                        userToUpdate.CountryId = request.CountryId.Value;
                        userNeedsUpdate = true;
                    }
                    
                    // Handle Gender enum directly
                    if (request.Gender.HasValue)
                    {
                        userToUpdate.Gender = request.Gender.Value;
                        userNeedsUpdate = true;
                    }

                    if (userNeedsUpdate)
                    {
                        userToUpdate.UpdatedAt = DateTime.UtcNow;
                        
                        // Update user through UserManager
                        var userUpdateResult = await _userManager.UpdateAsync(userToUpdate);
                        if (!userUpdateResult.Succeeded)
                        {
                            var errors = string.Join(", ", userUpdateResult.Errors.Select(e => e.Description));
                            return Result.Failure($"User update failed: {errors}");
                        }
                    }
                }

                // 4. Now get a tracked teacher instance for updating teacher-specific properties
                var trackedTeacher = await _unitOfWork.Repository<Teacher>()
                    .GetByIdAsync(teacher.Id);

                if (trackedTeacher == null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("TeacherNotFound"));
                }

                // 5. Update Teacher-specific properties only if provided
                if (request.Bio != null)
                {
                    trackedTeacher.Bio = request.Bio;
                }
                
                if (request.ProfilePictureUrl != null)
                {
                    trackedTeacher.ProfilePictureUrl = request.ProfilePictureUrl;
                }
                
                if (request.Salary.HasValue)
                {
                    trackedTeacher.Salary = request.Salary.Value;
                }
                
                if (request.AdditionalInterests != null)
                {
                    trackedTeacher.AdditionalInterests = request.AdditionalInterests;
                }
                
                if (request.CountryId.HasValue)
                {
                    trackedTeacher.CountryId = request.CountryId.Value;
                }

                // 6. Update Educations if provided
                if (request.Educations != null)
                {
                    trackedTeacher.Educations.Clear();
                    foreach (var edu in request.Educations)
                    {
                        trackedTeacher.Educations.Add(new TeacherEducation
                        {
                            Institution = edu.Institution,
                            Degree = edu.Degree,
                            Field = edu.Field,
                            StartDate = edu.StartDate,
                            EndDate = edu.EndDate,
                            Description = edu.Description
                        });
                    }
                }

                // 7. Update Certificates if provided
                if (request.Certificates != null)
                {
                    trackedTeacher.Certificates.Clear();
                    foreach (var cert in request.Certificates)
                    {
                        trackedTeacher.Certificates.Add(new Certificate
                        {
                            Name = cert.Name,
                            CertificateUrl = cert.CertificateUrl,
                            IssuedBy = cert.IssuedBy,
                            IssuedDate = cert.IssuedDate,
                            ExamResult = cert.ExamResult
                        });
                    }
                }

                // 8. Load teacher's skills separately to avoid tracking conflicts
                var teacherSkills = await _unitOfWork.Repository<TeacherSkill>()
                    .GetAllAsync();
                var currentSkills = await teacherSkills
                    .Where(ts => ts.TeacherId == teacher.Id)
                    .ToListAsync();

                // 9. Update Skills if provided
                if (request.Skills != null)
                {
                    // Remove existing skills
                    foreach (var skill in currentSkills)
                    {
                        _unitOfWork.Repository<TeacherSkill>().Delete(skill);
                    }

                    // Add new skills
                    foreach (var skill in request.Skills)
                    {
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
                }

                // 10. Handle Teaching Areas
                if (request.TeachingAreaIds != null)
                {
                    var currentAreas = await _unitOfWork.Repository<TeacherTeachingArea>()
                        .GetAllAsync();
                    var teacherAreas = await currentAreas
                        .Where(ta => ta.TeacherId == teacher.Id)
                        .ToListAsync();

                    foreach (var area in teacherAreas)
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
                }

                // 11. Handle Age Groups
                if (request.AgeGroupIds != null)
                {
                    var currentAgeGroups = await _unitOfWork.Repository<TeacherAgeGroup>()
                        .GetAllAsync();
                    var teacherAgeGroups = await currentAgeGroups
                        .Where(ag => ag.TeacherId == teacher.Id)
                        .ToListAsync();

                    foreach (var ageGroup in teacherAgeGroups)
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
                }

                // 12. Handle Communication Methods
                if (request.CommunicationMethodIds != null)
                {
                    var currentCommMethods = await _unitOfWork.Repository<TeacherCommunicationMethod>()
                        .GetAllAsync();
                    var teacherCommMethods = await currentCommMethods
                        .Where(cm => cm.TeacherId == teacher.Id)
                        .ToListAsync();

                    foreach (var commMethod in teacherCommMethods)
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
                }

                // 13. Handle Teaching Languages
                if (request.TeachingLanguageIds != null)
                {
                    var currentLangs = await _unitOfWork.Repository<TeacherTeachingLanguage>()
                        .GetAllAsync();
                    var teacherLangs = await currentLangs
                        .Where(tl => tl.TeacherId == teacher.Id)
                        .ToListAsync();

                    foreach (var teachingLang in teacherLangs)
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
                }

                // 14. Update teacher entity and timestamp
                trackedTeacher.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Teacher>().Update(trackedTeacher);

                // 15. Save all changes
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
