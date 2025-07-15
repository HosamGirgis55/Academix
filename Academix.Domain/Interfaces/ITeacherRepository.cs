using Academix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Interfaces
{
    public interface ITeacherRepository : IGenericRepository<Teacher>
    {
        Task<Teacher?> GetByEmailAsync(string email);
        Task<Teacher?> GetTeacherByUserIdAsync(string userId);
        Task UpdateAsync(Teacher teacher);
    }
}
