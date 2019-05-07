using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.Database
{
    public partial class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Applicant> Applicant { get; set; }
        public virtual DbSet<ApplicantFeature> ApplicantFeature { get; set; }

        public virtual DbSet<Beneficiary> Beneficiary { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Deal> Deal { get; set; }
        public virtual DbSet<DealRequest> DealRequest { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<Division> Division { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeDivision> EmployeeDivision { get; set; }
        public virtual DbSet<EmployeeStatus> EmployeeStatus { get; set; }

        public virtual DbSet<Facility> Facility { get; set; }
        public virtual DbSet<Feature> Feature { get; set; }
        public virtual DbSet<FixedSalary> FixedSalary { get; set; }
        public virtual DbSet<Insurance> Insurance { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemFeature> ItemFeature { get; set; }
        public virtual DbSet<Leave> Leave { get; set; }
        public virtual DbSet<ManagementPercent> ManagementPercent { get; set; }

        public virtual DbSet<Ownership> Ownership { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<Picture> Picture { get; set; }
        public virtual DbSet<Presence> Presence { get; set; }
        public virtual DbSet<Property> Property { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<PropertyFacility> PropertyFacility { get; set; }
        public virtual DbSet<PropertyFeature> PropertyFeature { get; set; }
        public virtual DbSet<PropertyOwnership> PropertyOwnership { get; set; }
        public virtual DbSet<Reminder> Reminder { get; set; }
        public virtual DbSet<Sms> Sms { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserItemCategory> UserItemCategory { get; set; }
        public virtual DbSet<UserPropertyCategory> UserPropertyCategory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.SeedDatabase();

            base.OnModelCreating(modelBuilder);
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void Detach<TEntity>(bool isNew = false) where TEntity : class
        {
            var entries = ChangeTracker.Entries().ToList();
            if (entries.Count == 0)
                return;

            foreach (var entry in ChangeTracker.Entries<TEntity>())
            {
                if (entry.Entity == null)
                    continue;

                if (!isNew)
                {
                    if (entry.State == EntityState.Modified
                        || entry.State == EntityState.Deleted)
                    {
                        Entry(entry.Entity).State = EntityState.Unchanged;
                    }
                }
                else
                {
                    if (entry.State == EntityState.Added)
                        Entry(entry.Entity).State = EntityState.Detached;
                }
            }
        }

        public void Detach(bool isNew = false)
        {
            var entries = ChangeTracker.Entries().ToList();
            if (entries.Count == 0)
                return;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (!isNew)
                {
                    if (entry.State == EntityState.Modified
                        || entry.State == EntityState.Deleted)
                    {
                        Entry(entry.Entity).State = EntityState.Unchanged;
                    }
                }
                else
                {
                    if (entry.State == EntityState.Added)
                        Entry(entry.Entity).State = EntityState.Detached;
                }
            }
        }

        public override int SaveChanges()
        {
            var result = base.SaveChanges();
            return result;
        }

        public async Task<int> SaveChangesAsync()
        {
            var result = await base.SaveChangesAsync().ConfigureAwait(false);
            return result;
        }

        //public TEntity Update<TEntity>(TEntity entity, string userId) where TEntity : class
        //{
        //    AddTrack(entity, userId, LogTypeEnum.Modify);
        //    return entity;
        //}
        public TEntity Update<TEntity>(TEntity entity, CurrentUserViewModel user, Dictionary<string, string> changes = null) where TEntity : BaseEntity
        {
            AddTrack(entity, user, LogTypeEnum.Modify, changes);
            return entity;
        }

        public new TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Modified;
            return entity;
        }

        private void AddTrack<TEntity>(TEntity model, CurrentUserViewModel user, LogTypeEnum type, Dictionary<string, string> changes = null) where TEntity : BaseEntity
        {
            if (user == null)
                return;

            var currentLogs = model.Audits;
            if (currentLogs?.Any() != true)
                currentLogs = new List<LogJsonEntity>();

            currentLogs.Add(new LogJsonEntity
            {
                Type = type,
                UserId = user.Id,
                DateTime = DateTime.Now,
                UserFullName = $"{user.FirstName} {user.LastName}",
                UserMobile = user.Mobile,
                Modifies = type == LogTypeEnum.Modify ? changes?.Select(x => new LogModifyDetailJsonEntity
                {
                    Key = x.Key,
                    Value = x.Value
                }).ToList() : default
            });
            model.Audits = currentLogs;
        }

        //private void AddTrack<TEntity>(TEntity model, string userId, LogTypeEnum type) where TEntity : class
        //{
        //    var track = new Log
        //    {
        //        Type = type,
        //        CreatorId = userId,
        //    };

        //    var trackType = track.GetType();
        //    var typeName = model.GetType().Name.Replace("Proxy", "");
        //    var findProperty = trackType.GetProperty($"{typeName}Id");
        //    if (findProperty == null)
        //        return;

        //    var propertyId = model.GetType().GetProperty("Id");
        //    var id = propertyId.GetValue(model);
        //    findProperty.SetValue(track, id);
        //    Add(track);
        //}

        public TEntity Add<TEntity>(TEntity entity, CurrentUserViewModel user) where TEntity : BaseEntity
        {
            var model = Add(entity);
            AddTrack(entity, user, LogTypeEnum.Create);
            return model;
        }

        //public TEntity Add<TEntity>(TEntity entity, string userId) where TEntity : class
        //{
        //    var model = Add(entity);
        //    AddTrack(entity, userId, LogTypeEnum.Create);
        //    return model;
        //}
        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public TEntity Delete<TEntity>(TEntity entity, CurrentUserViewModel user) where TEntity : BaseEntity
        {
            AddTrack(entity, user, LogTypeEnum.Delete);
            return entity;
        }

        public new TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            var model = base.Add(entity);
            return model.Entity;
        }

        public TEntity UnDelete<TEntity>(TEntity entity, CurrentUserViewModel user) where TEntity : BaseEntity
        {
            AddTrack(entity, user, LogTypeEnum.Undelete);
            return entity;
        }

        //public TEntity UnDelete<TEntity>(TEntity entity, string userId) where TEntity : class
        //{
        //    AddTrack(entity, userId, LogTypeEnum.Undelete);
        //    return entity;
        //}

        //public TEntity Delete<TEntity>(TEntity entity, string userId) where TEntity : class
        //{
        //    AddTrack(entity, userId, LogTypeEnum.Delete);
        //    return entity;
        //}
    }
}