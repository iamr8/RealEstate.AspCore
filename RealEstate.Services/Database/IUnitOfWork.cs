using Microsoft.EntityFrameworkCore;
using RealEstate.Services.Database.Base;
using RealEstate.Services.ViewModels;
using System;
using System.Threading.Tasks;

namespace RealEstate.Services.Database
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();

        TEntity Update<TEntity>(TEntity entity) where TEntity : class;

        TEntity Update<TEntity>(TEntity entity, UserViewModel user) where TEntity : BaseEntity;

        //        TEntity Update<TEntity>(TEntity entity, string userId) where TEntity : class;
        TEntity UnDelete<TEntity>(TEntity entity, UserViewModel user) where TEntity : BaseEntity;

        TEntity Add<TEntity>(TEntity entity) where TEntity : class;

        TEntity Add<TEntity>(TEntity entity, UserViewModel user) where TEntity : BaseEntity;

        //        TEntity Add<TEntity>(TEntity entity, string userId) where TEntity : class;

        TEntity Delete<TEntity>(TEntity entity, UserViewModel user) where TEntity : BaseEntity;

        //        TEntity UnDelete<TEntity>(TEntity entity, string userId) where TEntity : class;

        //        TEntity Delete<TEntity>(TEntity entity, string userId) where TEntity : class;
    }
}