using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.DTOs;
using MediatR;

namespace Academix.Application.Features.Authentication.Queries.GetProfile;

public class GetProfileQuery : IQuery<UserProfileDto>, IRequest<Result<UserProfileDto>>
{
    public string UserId { get; set; } = string.Empty;
} 