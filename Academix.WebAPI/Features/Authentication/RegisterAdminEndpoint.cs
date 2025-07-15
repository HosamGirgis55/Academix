using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.Commands.RegisterAdmin;
using Academix.Helpers;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Academix.WebAPI.Features.Authentication;

public class RegisterAdminEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register-admin", RegisterAdminAsync)
            .WithName("RegisterAdmin")
            .WithTags("Authentication")
            .Produces<object>(200)
            .Produces<object>(400)
            .WithSummary("Register a new admin user")
            .WithDescription("Registers a new admin user in the system");
    }

    private static async Task<IResult> RegisterAdminAsync(
        IMediator mediator, 
        [FromBody] RegisterAdminCommand command,
        HttpContext httpContext)
    {
        try
        {
            var culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";

            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                var successMessage = culture == "ar" ? "تم تسجيل المدير بنجاح" : "Admin registered successfully";
                return Results.Ok(new ResponseHelper()
                    .Success(result.Value, culture)
                    .WithMassage(successMessage));
            }
            else
            {
                return Results.Ok(new ResponseHelper()
                    .BadRequest(result.Error, culture));
            }
        }
        catch (Exception ex)
        {
            var culture = httpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";
            var errorMessage = culture == "ar" ? "حدث خطأ غير متوقع أثناء تسجيل المدير" : "An unexpected error occurred during admin registration";
            
            return Results.Ok(new ResponseHelper()
                .ServerError($"{errorMessage}: {ex.Message}", culture));
        }
    }
} 