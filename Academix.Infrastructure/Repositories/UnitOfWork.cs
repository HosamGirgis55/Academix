using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Student> Students => GetRepository<Student>();
        public IGenericRepository<Teacher> Teachers => GetRepository<Teacher>();
        public IGenericRepository<Country> Countries => GetRepository<Country>();
        public IGenericRepository<Level> Levels => GetRepository<Level>();
        public IGenericRepository<GraduationStatus> GraduationStatuses => GetRepository<GraduationStatus>();
        public IGenericRepository<Specialization> Specializations => GetRepository<Specialization>();
        public IGenericRepository<Skill> Skills => GetRepository<Skill>();
        public IGenericRepository<Experience> Experiences => GetRepository<Experience>();
        public IGenericRepository<StudentSkill> StudentSkills => GetRepository<StudentSkill>();
        public IGenericRepository<StudentExperience> StudentExperiences => GetRepository<StudentExperience>();
        public IGenericRepository<TeacherSkill> TeacherSkills => GetRepository<TeacherSkill>();

        public IGenericRepository<Interview> Interviews => GetRepository<Interview>();


        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            return GetRepository<T>();
        }

        private IGenericRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<T>(_context);
            }

            return (IGenericRepository<T>)_repositories[type];
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