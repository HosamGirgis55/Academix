using Academix.Domain.Entities;

namespace Academix.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICountryRepository Countries { get; }
        INationalityRepository Nationalities { get; }
        IPositionRepository Positions { get; }
        ISpecializationRepository Specialization { get; }
        IExperiencesRepository Experiences { get; }
        ILevelRepository Level { get; }
        IFieldRepository Field { get; }
        ICommunicationRepository Communication { get; }
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
} 