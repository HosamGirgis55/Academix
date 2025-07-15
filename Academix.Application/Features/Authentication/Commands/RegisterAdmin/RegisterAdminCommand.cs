using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Authentication.Commands.RegisterAdmin
{
    public class RegisterAdminCommand : IRequest<Result<AuthenticationResult>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public int Gender { get; set; }
        public Guid CountryId { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
} 