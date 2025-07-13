using Academix.Application.Common.Models;
using MediatR;
using System;

namespace Academix.Application.Features.Comments.Commands.AddComment
{
    public class AddCommentCommand : IRequest<Result>
    {
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-5 star rating
        public Guid TeacherId { get; set; }
        public Guid StudentId { get; set; } // This will be set from the authenticated user
    }
} 