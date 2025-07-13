using Academix.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace Academix.Application.Features.Comments.Queries.GetTeacherComments
{
    public class GetTeacherCommentsQuery : IRequest<Result<List<CommentDto>>>
    {
        public Guid TeacherId { get; set; }
    }

    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public StudentInfoDto Student { get; set; } = null!;
        public TeacherRatingDto TeacherRating { get; set; } = null!;
    }

    public class StudentInfoDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }

    public class TeacherRatingDto
    {
        public double AverageRating { get; set; }
        public int TotalComments { get; set; }
    }
} 