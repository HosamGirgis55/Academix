using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Common.Queries.GetGenders;

public class GetGendersQuery : IQuery<List<GenderDto>>, IRequest<Result<List<GenderDto>>>
{
}

public class GenderDto
{
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
} 