using Academix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Academix.Domain.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<List<Comment>> GetCommentsByTeacherIdAsync(Guid teacherId);
        Task<List<Comment>> GetCommentsByStudentIdAsync(Guid studentId);
        Task<bool> HasStudentCommentedOnTeacherAsync(Guid studentId, Guid teacherId);
        Task<double> GetAverageRatingForTeacherAsync(Guid teacherId);
        Task<int> GetCommentCountForTeacherAsync(Guid teacherId);
    }
} 