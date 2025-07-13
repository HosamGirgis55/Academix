using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Comment>> GetCommentsByTeacherIdAsync(Guid teacherId)
        {
            return await _context.Comments
                .Where(c => c.TeacherId == teacherId)
                .Include(c => c.Student)
                .ThenInclude(s => s.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByStudentIdAsync(Guid studentId)
        {
            return await _context.Comments
                .Where(c => c.StudentId == studentId)
                .Include(c => c.Teacher)
                .ThenInclude(t => t.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasStudentCommentedOnTeacherAsync(Guid studentId, Guid teacherId)
        {
            return await _context.Comments
                .AnyAsync(c => c.StudentId == studentId && c.TeacherId == teacherId);
        }

        public async Task<double> GetAverageRatingForTeacherAsync(Guid teacherId)
        {
            var comments = await _context.Comments
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.Rating)
                .ToListAsync();

            return comments.Any() ? comments.Average() : 0;
        }

        public async Task<int> GetCommentCountForTeacherAsync(Guid teacherId)
        {
            return await _context.Comments
                .CountAsync(c => c.TeacherId == teacherId);
        }
    }
} 