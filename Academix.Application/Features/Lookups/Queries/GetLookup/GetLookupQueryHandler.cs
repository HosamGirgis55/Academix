using System.Reflection;
using Academix.Application.Common.Interfaces;
using Academix.Domain.DTOs;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Lookups.Queries.GetLookup;

public class GetLookupQueryHandler : IQueryHandler<GetLookupQuery, List<LookupItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public GetLookupQueryHandler(IUnitOfWork unitOfWork, ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
    }

    public async Task<List<LookupItemDto>> Handle(GetLookupQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
            throw new ArgumentException("Lookup type cannot be empty", nameof(request.Type));

        _localizationService.SetCulture(request.Language);
        var isArabic = request.Language?.ToLower() == "ar";

        try
        {
            // First try to handle as enum
            var enumType = GetEnumType(request.Type);
            if (enumType != null)
            {
                return GetEnumLookup(enumType, isArabic);
            }

            // Then try to handle as entity
            var repository = GetRepositoryForType(request.Type);
            if (repository != null)
            {
                var method = repository.GetType().GetMethod("GetAllAsync");
                if (method == null)
                    throw new InvalidOperationException($"Repository for {request.Type} does not implement GetAllAsync method");

                var task = method.Invoke(repository, null) as Task<IEnumerable<object>>;
                if (task == null)
                    throw new InvalidOperationException($"Failed to invoke GetAllAsync on repository for {request.Type}");

                var entities = await task;
                if (entities == null)
                    return new List<LookupItemDto>();

                return entities
                    .Where(e => e != null)
                    .Select(e => CreateLookupItemFromEntity(e, isArabic))
                    .Where(item => item != null)
                    .ToList();
            }

            throw new ArgumentException($"No lookup found for type: {request.Type}. Ensure the type name matches an entity or enum name.");
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ApplicationException($"Error processing lookup for type: {request.Type}", ex);
        }
    }

    private Type? GetEnumType(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        // First check in Domain assembly for enums
        var domainAssembly = typeof(Domain.Enums.Gender).Assembly;
        var enumType = domainAssembly.GetTypes()
            .FirstOrDefault(t => t.IsEnum && 
                string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase));

        if (enumType != null)
            return enumType;

        // Then check other assemblies if not found
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location));

        foreach (var assembly in assemblies)
        {
            try
            {
                var type = assembly.GetTypes()
                    .FirstOrDefault(t => t.IsEnum && 
                        string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase));
                
                if (type != null)
                    return type;
            }
            catch (ReflectionTypeLoadException)
            {
                // Skip assemblies that can't be loaded
                continue;
            }
        }
        return null;
    }

    private dynamic? GetRepositoryForType(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        // Try both singular and plural forms
        var possibleNames = new[]
        {
            typeName,
            typeName + "s",
            typeName.TrimEnd('s')
        }.Distinct();

        var unitOfWorkType = typeof(IUnitOfWork);
        
        foreach (var name in possibleNames)
        {
            var property = unitOfWorkType.GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

            if (property != null)
                return property.GetValue(_unitOfWork);
        }

        return null;
    }

    private List<LookupItemDto> GetEnumLookup(Type enumType, bool isArabic)
    {
        var values = Enum.GetValues(enumType);
        var result = new List<LookupItemDto>();

        foreach (var value in values)
        {
            var name = value.ToString()!;
            var localizedKey = $"Enum_{enumType.Name}_{name}";
            var localizedName = _localizationService.GetLocalizedString(localizedKey);

            result.Add(new LookupItemDto
            {
                Id = Convert.ToInt32(value),  // Use actual enum value
                Name = string.IsNullOrEmpty(localizedName) ? name : localizedName,
                Code = name.ToLower()
            });
        }

        return result;
    }

    private LookupItemDto? CreateLookupItemFromEntity(object entity, bool isArabic)
    {
        try
        {
            var id = GetPropertyValue<int>(entity, "Id");
            
            // Try different naming conventions for localized names
            var arabicName = GetPropertyValue<string>(entity, "NameAr") ?? 
                           GetPropertyValue<string>(entity, "NameArabic") ?? 
                           GetPropertyValue<string>(entity, "ArabicName");
            
            var englishName = GetPropertyValue<string>(entity, "NameEn") ?? 
                            GetPropertyValue<string>(entity, "NameEnglish") ?? 
                            GetPropertyValue<string>(entity, "EnglishName");

            var code = GetPropertyValue<string>(entity, "code") ?? 
                      GetPropertyValue<string>(entity, "Code");

            if (string.IsNullOrEmpty(arabicName) || string.IsNullOrEmpty(englishName))
            {
                // Try getting a single Name property if localized names are not found
                var name = GetPropertyValue<string>(entity, "Name");
                if (!string.IsNullOrEmpty(name))
                {
                    arabicName = name;
                    englishName = name;
                }
            }

            return new LookupItemDto
            {
                Id = id,
                Name = isArabic ? arabicName : englishName,
                Code = code
            };
        }
        catch (Exception ex)
        {
            // Log the error but return null to skip this item
            return null;
        }
    }

    private static T? GetPropertyValue<T>(object? obj, string propertyName)
    {
        if (obj == null || string.IsNullOrEmpty(propertyName))
            return default;

        try
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
                return default;

            var value = property.GetValue(obj);
            if (value == null)
                return default;

            return (T)value;
        }
        catch
        {
            return default;
        }
    }
} 