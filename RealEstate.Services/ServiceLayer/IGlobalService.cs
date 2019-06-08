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

            var query = _items.IgnoreQueryFilters();

            if (currentUser.Role != Role.SuperAdmin && currentUser.Role != Role.Admin)
                query = query.Where(x => x.Audits.Find(c => c.Type == LogTypeEnum.Create).UserId == currentUser.Id);

            if (searchModel != null)
                query = _baseService.AdminSeachConditions(query, searchModel);

            var models = await query.ToListAsync().ConfigureAwait(false);
            if (models?.Any() != true)
                return default;

            var viewModels = models?.Select(x => x.Map<Item, ItemViewModel>(ent =>
            {
                ent.Include<Property, PropertyViewModel>(x.Property, ent2 => ent2.Include<Category, CategoryViewModel>(ent2.Entity.Category));
                ent.Include<Category, CategoryViewModel>(x.Category);
            })).ToList();
            if (viewModels?.Any() != true)
                return default;

            var result = new List<StatisticsViewModel>();
            if (!string.IsNullOrEmpty(searchModel?.CreationDateFrom))
            {
                var items = viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= searchModel.CreationDateFrom.PersianToGregorian());
                if (!string.IsNullOrEmpty(searchModel?.CreationDateTo))
                    items = items.Where(x => x.Logs?.Create?.DateTime.Date <= searchModel.CreationDateTo.PersianToGregorian());
                if (!string.IsNullOrEmpty(searchModel?.CreatorId))
                    items = items.Where(x => x.Logs?.Create?.UserId == searchModel.CreatorId);

                result.Add(new StatisticsViewModel
                {
                    Details = Map(items.ToList()),
                    From = searchModel.CreationDateFrom.PersianToGregorian(),
                    To = DateTime.Today,
                    Range = StatisticsRangeEnum.Custom
                });
            }
            else if (!string.IsNullOrEmpty(searchModel?.CreationDateTo))
            {
                var items = viewModels.Where(x => x.Logs?.Create?.DateTime.Date <= searchModel.CreationDateTo.PersianToGregorian());
                if (!string.IsNullOrEmpty(searchModel?.CreationDateFrom))
                    items = viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= searchModel.CreationDateFrom.PersianToGregorian());
                if (!string.IsNullOrEmpty(searchModel?.CreatorId))
                    items = items.Where(x => x.Logs?.Create?.UserId == searchModel.CreatorId);
                result.Add(new StatisticsViewModel
                {
                    Details = Map(items.ToList()),
                    To = searchModel.CreationDateTo.PersianToGregorian(),
                    Range = StatisticsRangeEnum.Custom
                });
            }
            else if (!string.IsNullOrEmpty(searchModel?.CreatorId))
            {
                var items = viewModels.Where(x => x.Logs?.Create?.UserId == searchModel.CreatorId);
                if (!string.IsNullOrEmpty(searchModel?.CreationDateFrom))
                    items = viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= searchModel.CreationDateFrom.PersianToGregorian());
                if (!string.IsNullOrEmpty(searchModel?.CreationDateFrom))
                    items = viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= searchModel.CreationDateFrom.PersianToGregorian());
                result.Add(new StatisticsViewModel
                {
                    Details = Map(items.ToList()),
                    To = searchModel.CreationDateTo.PersianToGregorian(),
                    Range = StatisticsRangeEnum.Custom
                });
            }
            else
            {
                var today = viewModels.Where(x => x.Logs?.Create?.DateTime.Date == DateTime.Today.Date).ToList();
                var thisWeek = viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= DateTime.Today.AddDays(-7).Date).ToList();
                var thisMonth = viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= DateTime.Today.AddMonths(-1).Date).ToList();
                result.Add(new StatisticsViewModel
                {
                    Details = Map(today),
                    From = DateTime.Today.Date.AddDays(-1),
                    To = DateTime.Today.Date,
                    Range = StatisticsRangeEnum.Today
                });
                result.Add(new StatisticsViewModel
                {
                    Details = Map(thisWeek),
                    From = DateTime.Today.Date.AddDays(-7),
                    To = DateTime.Today.Date,
                    Range = StatisticsRangeEnum.ThisWeek
                });
                result.Add(new StatisticsViewModel
                {
                    Details = Map(thisMonth),
                    From = DateTime.Today.Date.AddDays(-30),
                    To = DateTime.Today.Date,
                    Range = StatisticsRangeEnum.ThisMonth
                });
            }

            if (result?.Any() != true)
                return default;

            return result;
        }
    }
}