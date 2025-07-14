using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Sessions.Commands.EndSession
{
    public class EndSessionCommand : ICommand<Result>
    {
        public Guid SessionId { get; set; }
        public string? TeacherNotes { get; set; }
        public string? StudentNotes { get; set; }
        public int? StudentRating { get; set; } // 1-5 stars
        public int? TeacherRating { get; set; } // 1-5 stars
    }
} 