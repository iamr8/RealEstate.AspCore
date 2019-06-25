using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using RealEstate.Services.Database.Base;
using RealEstate.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstate.Services.Database
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();

        void Detach(bool isNew = false);

        IDbContextServices GetDbContextServices();

        EntityEntry<TEntity> GetEntityEntry<TEntity>(TEntity entity) where TEntity : class;

        void Detach<TEntity>(bool isNew = false) where TEntity : class;

        DatabaseFacade Db { get; }
        ChangeTracker Tracker { get; }

        TEntity Update<TEntity>(TEntity entity) where TEntity : class;

        TEntity Update<TEntity>(TEntity entity, CurrentUserViewModel user, Dictionary<string, string> changes = null) where TEntity : BaseEntity;

        //        TEntity Update<TEntity>(TEntity entity, string userId) where TEntity : class;
        TEntity UnDelete<TEntity>(TEntity entity, CurrentUserViewModel user) where TEntity : BaseEntity;

        void Delete<TEntity>(TEntity entity) where TEntity : class;

        TEntity Add<TEntity>(TEntity entity) where TEntity : class;

        TEntity Add<TEntity>(TEntity entity, CurrentUserViewModel user) where TEntity : BaseEntity;

        //        TEntity Add<TEntity>(TEntity entity, string userId) where TEntity : class;

        TEntity Delete<TEntity>(TEntity entity, CurrentUserViewModel user) where TEntity : BaseEntity;

        //        TEntity UnDelete<TEntity>(TEntity entity, string userId) where TEntity : class;

        //        TEntity Delete<TEntity>(TEntity entity, string userId) where TEntity : class;
    }
}