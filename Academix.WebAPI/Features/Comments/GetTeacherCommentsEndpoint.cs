using Academix.Application.Features.Comments.Queries.GetTeacherComments;
using Academix.Application.Common.Models;
using Academix.WebAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Academix.Helpers;
using Academix.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Academix.WebAPI.Features.Comments
{
    public class GetTeacherCommentsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/comments/teacher/{teacherId}", HandleAsync)
                .WithName("GetTeacherComments")
                .WithTags("Comments")
                .Produces<ResponseHelper>(200)
                .Produces<ResponseHelper>(400)
                .Produces<ResponseHelper>(404);
        }

        private static async Task<IResult> HandleAsync(
            [FromRoute] Guid teacherId,
            [FromServices] IMediator mediator,
            [FromServices] ResponseHelper response,
            [FromServices] ILocalizationService localizationService,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetTeacherCommentsQuery { TeacherId = teacherId };
                var result = await mediator.Send(query, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(response.Success(result.Value));
                }

                return Results.BadRequest(response.BadRequest(result.Error));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(response.BadRequest(localizationService.GetLocalizedString("GetCommentsFailed")));
            }
        }
    }
} 