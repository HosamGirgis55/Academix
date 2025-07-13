using Academix.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Academix.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Student> Students { get; }
        IGenericRepository<Teacher> Teachers { get; }
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<Level> Levels { get; }
        IGenericRepository<GraduationStatus> GraduationStatuses { get; }
        IGenericRepository<Specialization> Specializations { get; }
        IGenericRepository<Skill> Skills { get; }
        IGenericRepository<Experience> Experiences { get; }
        IGenericRepository<StudentSkill> StudentSkills { get; }
        IGenericRepository<StudentExperience> StudentExperiences { get; }
        IGenericRepository<TeacherSkill> TeacherSkills { get; }
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
} 