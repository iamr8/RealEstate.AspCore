using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IGlobalService
    {
        Task<List<LogDetailViewModel>> Logs(string entityName, string entityId);

        Task<List<StatisticsViewModel>> StatisticsAsync(StatisticsSearchViewModel searchModel);
    }

    public class GlobalService : IGlobalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IBaseService _baseService;
        private readonly DbSet<Item> _items;

        public GlobalService(
            IBaseService baseService,
            IUnitOfWork unitOfWork,
            ApplicationDbContext applicationDbContext
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _applicationDbContext = applicationDbContext;
            _items = _unitOfWork.Set<Item>();
        }

        private List<StatisticsDetailViewModel> Map(List<ItemViewModel> item)
        {
            if (item?.Any() != true)
                return default;

            var result = item.Select(x => new StatisticsDetailViewModel
            {
                ItemId = x.Id,
                ItemCategory = x.Category?.Name,
                PropertyCategory = x.Property?.Category?.Name,
                UserFullName = x.Logs.Create.UserFullName,
                UserId = x.Logs.Create.UserId
            }).ToList();
            return result;
        }

        private List<LogJsonEntity> GetAudits(IReflect entityType, string entityId, List<object> model)
        {
            if (model?.Any() != true)
                return default;

            var result = new List<LogJsonEntity>();
            Applicant testModel;
            foreach (var row in model)
            {
                var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                if (properties?.Any() != true)
                    continue;

                var idProperty = row.GetProperty(nameof(testModel.Id));
                if (idProperty == null || !(idProperty.GetValue(row) is string id))
                    continue;

                if (string.IsNullOrEmpty(id) || id != entityId)
                    continue;

                var auditProperty = row.GetProperty(nameof(testModel.Audit));
                if (auditProperty == null || !(auditProperty.GetValue(row) is string audit))
                    continue;

                if (string.IsNullOrEmpty(audit))
                    continue;

                var audits = audit.JsonGetAccessor<LogJsonEntity>();
                if (audits?.Any() != true)
                    continue;

                result = audits;
            }

            return result;
        }

        public async Task<List<LogDetailViewModel>> Logs(string entityName, string entityId)
        {
            if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entityId))
                return default;

            var query = _applicationDbContext.AsQueryable(entityName);
            var type = query.ElementType;
            var model = await query.ToListAsync().ConfigureAwait(false);
            var result = GetAudits(type, entityId, model);
            if (result?.Any() != true)
                return default;

            var audits = result.Select(x => x.MapLog()).Reverse().ToList();
            return audits;
        }

        public async Task<List<StatisticsViewModel>> StatisticsAsync(StatisticsSearchViewModel searchModel)
        {
            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return default;

            var query = _items
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(x => x.Property)
                .ThenInclude(x => x.Category)
                .Include(x => x.Category)
                .AsQueryable();

            if (currentUser.Role != Role.SuperAdmin && currentUser.Role != Role.Admin)
                query = query.Where(x => x.Audits.Find(c => c.Type == LogTypeEnum.Create).UserId == currentUser.Id);

            if (searchModel != null)
                query = _baseService.AdminSeachConditions(query, searchModel);

            var result = new List<StatisticsViewModel>();

            var dateTimeToday = DateTime.Today.Date;
            var dateTimeWeek = DateTime.Today.AddDays(-7);
            var dateTimeMonth = DateTime.Today.AddMonths(-1);
            var dateTimeMonthString = dateTimeMonth.ToString("yyyy/MM/dd", new CultureInfo("en-US"));

            var monthQuery = await query
                .Where(x => CustomDbFunctionsExtensions.DateDiff("DAY", dateTimeMonthString, CustomDbFunctionsExtensions.JsonValue(x.Audit, "$[0].d")) <= 0)
                .Cacheable()
                .ToListAsync();
            if (monthQuery?.Any() != true)
                return default;

            var viewModels = monthQuery?.Select(x => x.Map<ItemViewModel>(ent =>
            {
                ent.IncludeAs<Property, PropertyViewModel>(x.Property, ent2 => ent2.IncludeAs<Category, CategoryViewModel>(ent2.Entity.Category));
                ent.IncludeAs<Category, CategoryViewModel>(x.Category);
            })).ToList();
            if (viewModels?.Any() != true)
                return default;

            result.Add(new StatisticsViewModel
            {
                Details = Map(viewModels.Where(x => x.Logs?.Create?.DateTime.Date == dateTimeToday).ToList()),
                From = dateTimeToday,
                To = DateTime.Today.Date,
                Range = StatisticsRangeEnum.Today
            });
            result.Add(new StatisticsViewModel
            {
                Details = Map(viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= dateTimeWeek.Date).ToList()),
                From = dateTimeWeek,
                To = DateTime.Today.Date,
                Range = StatisticsRangeEnum.ThisWeek
            });
            result.Add(new StatisticsViewModel
            {
                Details = Map(viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= dateTimeMonth.Date).ToList()),
                From = dateTimeMonth,
                To = DateTime.Today.Date,
                Range = StatisticsRangeEnum.ThisMonth
            });

            if (result?.Any() != true)
                return default;

            return result;
        }
    }
}