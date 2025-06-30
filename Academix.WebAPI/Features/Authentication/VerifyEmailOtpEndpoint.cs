using Academix.Application.Features.Authentication.Commands.VerifyEmailOtp;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;

namespace Academix.WebAPI.Features.Authentication;

public class VerifyEmailOtpEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/verify-email", async (
            VerifyEmailOtpRequest request, 
            IMediator mediator) =>
        {
            var command = new VerifyEmailOtpCommand
            {
                Email = request.Email,
                Otp = request.Otp
            };

            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                return Results.Ok(new ResponseHelper().Success(
                    result.Value,
                    "en"
                ).WithMassage(result.SuccessMessage ?? "Email verified successfully"));
            }

            return Results.BadRequest(new ResponseHelper().BadRequest(
                result.Error,
                "en"
            ));
        })
        .WithName("VerifyEmailOtp")
        .WithTags("Authentication")
        .WithSummary("Verify email with OTP")
        .WithDescription("Verifies the user's email address using the OTP sent during registration")
        .Produces<object>(200)
        .Produces<object>(400);
    }
}

public class VerifyEmailOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
} 