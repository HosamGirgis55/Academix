using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.DTOs;
using MediatR;

namespace Academix.Application.Features.Authentication.Commands.UpdateProfile;

public class UpdateProfileCommand : ICommand<UserProfileDto>, IRequest<Result<UserProfileDto>>
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public Guid CountryId { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? TimeZone { get; set; }
} 