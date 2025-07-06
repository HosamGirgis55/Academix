using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Academix.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private TInterface GetRepository<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : TInterface
        {
            var type = typeof(TInterface);
            if (_repositories.TryGetValue(type, out var repo))
            {
                return (TInterface)repo;
            }

            var instance = (TInterface)Activator.CreateInstance(typeof(TImplementation), _context)!;
            _repositories[type] = instance;
            return instance;
        }

        public ICountryRepository Countries => GetRepository<ICountryRepository, CountryRepository>();
        public INationalityRepository Nationalities => GetRepository<INationalityRepository, NationalityRepository>();
        public IPositionRepository Positions => GetRepository<IPositionRepository, PositionRepository>();
        public ISpecializationRepository Specialization => GetRepository<ISpecializationRepository, SpecializationRepository>();
        public IExperiencesRepository Experiences => GetRepository<IExperiencesRepository, ExperiencesRepository>();
        public ILevelRepository Level => GetRepository<ILevelRepository, LevelRepository>();
        public IFieldRepository Field => GetRepository<IFieldRepository, FieldRepository>();

        public ICommunicationRepository Communication => GetRepository<ICommunicationRepository,CommunicationRepository>();


        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IGenericRepository<T>)_repositories[typeof(T)];
            }

            var repository = new GenericRepository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
} 