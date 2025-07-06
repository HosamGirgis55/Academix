using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using Academix.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public LookupController(IUnitOfWork unitOfWork, ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        [HttpGet]
        public async Task<ActionResult<Result<List<LookupItemDto>>>> GetLookup(
            [FromQuery] string type,
            [FromQuery] string lang = "en")
        {
            _localizationService.SetCulture(lang);

            switch (type?.ToLower())
            {
                case "gender":
                    var genderNames = lang.ToLower() == "ar"
                    ? new[] { "ذكر", "أنثى" }
                    : new[] { "Male", "Female" };

                    var genders = genderNames.Select((name, index) => new LookupItemDto
                    {
                        Name = name
                    }).ToList();
                    return Ok(Result<List<LookupItemDto>>.Success(genders));

                case "countries":
                    var countries = await _unitOfWork.Countries.GetAllAsync();
                    var result = countries.Select(c => new LookupItemDto
                    {
                        Id = c.Id,
                        Name = lang.ToLower() == "ar" ? c.NameAr : c.NameEn,
                        Code = c.Code
                    }).ToList();
                    return Ok(Result<List<LookupItemDto>>.Success(result));

                case "nationalities":
                    var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
                    var nationalityResult = nationalities.Select(n => new LookupItemDto
                    {
                        Id = n.Id,
                        Name = lang.ToLower() == "ar" ? n.NameAr : n.NameEn,
                        Code = n.Code
                    }).ToList();

                    return Ok(Result<List<LookupItemDto>>.Success(nationalityResult));


                case "positions":
                    var Positions = await _unitOfWork.Positions.GetAllAsync();
                    var PositionResult = Positions.Select(n => new LookupItemDto
                    {
                        Id = n.Id,
                        Name = lang.ToLower() == "ar" ? n.NameAr : n.NameEn,
                        Code = n.Code
                    }).ToList();

                    return Ok(Result<List<LookupItemDto>>.Success(PositionResult));

                case "specialization":
                    var Specialization = await _unitOfWork.Specialization.GetAllAsync();
                    var SpecializationResult = Specialization.Select(n => new LookupItemDto
                    {
                        Id = n.Id,
                        Name = lang.ToLower() == "ar" ? n.NameAr : n.NameEn,
                        Code = n.Code
                    }).ToList();

                    return Ok(Result<List<LookupItemDto>>.Success(SpecializationResult));

                case "experiences":
                    var Experiences = await _unitOfWork.Experiences.GetAllAsync();
                    var ExperiencesResult = Experiences.Select(n => new LookupItemDto
                    {
                        Id = n.Id,
                        Name = lang.ToLower() == "ar" ? n.NameAr : n.NameEn,
                        Code = n.Code
                    }).ToList();

                    return Ok(Result<List<LookupItemDto>>.Success(ExperiencesResult));

                case "levels":
                    var Levels = await _unitOfWork.Level.GetAllAsync();
                    var LevelsResult = Levels.Select(n => new LookupItemDto
                    {
                        Id = n.Id,
                        Name = lang.ToLower() == "ar" ? n.NameAr : n.NameEn,
                        Code = n.Code
                    }).ToList();

                    return Ok(Result<List<LookupItemDto>>.Success(LevelsResult));

                case "field":
                    var Fields = await _unitOfWork.Field.GetAllAsync();
                    var FieldsResult = Fields.Select(n => new LookupItemDto
                    {
                        Id = n.Id,
                        Name = lang.ToLower() == "ar" ? n.NameAr : n.NameEn,
                        Code = n.Code
                    }).ToList();

                    return Ok(Result<List<LookupItemDto>>.Success(FieldsResult));

                case "communication":
                    var Communications = await _unitOfWork.Communication.GetAllAsync();
                    var CommunicationsResult = Communications.Select(n => new LookupItemDto
                    {
                        Id = n.Id,
                        Name = lang.ToLower() == "ar" ? n.NameAr : n.NameEn,
                        Code = n.Code
                    }).ToList();

                    return Ok(Result<List<LookupItemDto>>.Success(CommunicationsResult));



                default:
                    return BadRequest(Result<List<LookupItemDto>>.Failure("Invalid type"));
            }
        }
    }
}
