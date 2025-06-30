using Academix.Application.Features.Authentication.Commands.VerifyPasswordResetOtp;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;

namespace Academix.WebAPI.Features.Authentication;

public class VerifyPasswordResetOtpEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/reset-password-with-otp", async (
            VerifyPasswordResetOtpRequest request, 
            IMediator mediator) =>
        {
            var command = new VerifyPasswordResetOtpCommand
            {
                Email = request.Email,
                Otp = request.Otp,
                NewPassword = request.NewPassword
            };

            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                return Results.Ok(new ResponseHelper().Success(
                    result.Value,
                    "en"
                ).WithMassage(result.SuccessMessage ?? "Password reset successfully"));
            }

            return Results.BadRequest(new ResponseHelper().BadRequest(
                result.Error,
                "en"
            ));
        })
        .WithName("VerifyPasswordResetOtp")
        .WithTags("Authentication")
        .WithSummary("Reset password with OTP")
        .WithDescription("Resets the user's password using the OTP sent to their email")
        .Produces<object>(200)
        .Produces<object>(400);
    }
}

public class VerifyPasswordResetOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
} 