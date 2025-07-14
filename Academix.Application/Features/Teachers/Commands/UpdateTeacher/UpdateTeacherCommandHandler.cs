using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Teachers.Commands.UpdateTeacher
{
    internal class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public UpdateTeacherCommandHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //// 1. جيب المدرس بالإيميل (باستخدام الميثود الجنريك)
                //var teacher = await _unitOfWork.Repository<Domain.Entities.Teacher>()
                //    .GetByEmailAsync(
                //        request.Email,
                //        t => t.User,
                //        t => t.Educations,
                //        t => t.Certificates,
                //        t => t.Skills
                //    );

                //if (teacher == null)
                //{
                //    var notFoundMessage = _localizationService.GetLocalizedString("TeacherNotFound");
                //    return Result.Failure(notFoundMessage);
                //}

                //// 2. تحديث بيانات الـ User
                //teacher.User.FirstName = request.FirstName;
                //teacher.User.LastName = request.LastName;
                //teacher.User.PhoneNumber = request.PhoneNumber;

                //// 3. تحديث بيانات المدرّس
                //teacher.Bio = request.Bio;
                //teacher.ProfilePictureUrl = request.ProfilePictureUrl;
                //teacher.Salary = request.Salary;
                //teacher.AdditionalInterests = request.AdditionalInterests;

                //// 4. تحديث التعليم
                //teacher.Educations.Clear();
                //foreach (var edu in request.Educations)
                //{
                //    teacher.Educations.Add(new Domain.Entities.TeacherEducation
                //    {
                //        Id = Guid.NewGuid(),
                //        School = edu.School,
                //        Degree = edu.Degree,
                //        Year = edu.Year,
                //        TeacherId = teacher.Id
                //    });
                //}

                //// 5. تحديث الشهادات
                //teacher.Certificates.Clear();
                //foreach (var cert in request.Certificates)
                //{
                //    teacher.Certificates.Add(new Domain.Entities.TeacherCertificate
                //    {
                //        Id = Guid.NewGuid(),
                //        Name = cert.Name,
                //        Issuer = cert.Issuer,
                //        Year = cert.Year,
                //        TeacherId = teacher.Id
                //    });
                //}

                //// 6. تحديث المهارات
                //teacher.Skills.Clear();
                //foreach (var skill in request.Skills)
                //{
                //    teacher.Skills.Add(new Domain.Entities.TeacherSkill
                //    {
                //        Id = Guid.NewGuid(),
                //        SkillId = skill.SkillId,
                //        TeacherId = teacher.Id
                //    });
                //}

                //// 7. حفظ التغييرات
                //await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("TeacherUpdateFailed") + $": {ex.Message}";
                return Result.Failure(error);
            }
        }


    }
}
