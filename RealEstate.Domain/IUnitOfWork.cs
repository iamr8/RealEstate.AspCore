using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RealEstate.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();

        TEntity Update<TEntity>(TEntity entity) where TEntity : class;

        TEntity Update<TEntity>(TEntity entity, string userId) where TEntity : class;

        TEntity Add<TEntity>(TEntity entity) where TEntity : class;

        TEntity Add<TEntity>(TEntity entity, string userId) where TEntity : class;

        TEntity UnDelete<TEntity>(TEntity entity, string userId) where TEntity : class;

        TEntity Delete<TEntity>(TEntity entity, string userId) where TEntity : class;
    }
}