using Academix.Domain.Entities;

namespace Academix.Domain.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetStudentByUserIdAsync(string userId);
        Task UpdateAsync(Student student);
    }
} 