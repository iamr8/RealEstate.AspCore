using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.ModelBind;
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

        Task<StatisticsViewModel> StatisticsAsync();
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

        private StatisticsDetailViewModel Map(ItemViewModel item)
        {
            var result = new StatisticsDetailViewModel
            {
                ItemId = item.Id,
                ItemCategory = item.Category.Value?.Name,
                PropertyCategory = item.Property.Value?.Category.Value?.Name,
                UserFullName = item.Logs.Create.UserFullName,
                UserId = item.Logs.Create.UserId
            };
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

        public async Task<StatisticsViewModel> StatisticsAsync()
        {
            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return default;

            var query = _items.IgnoreQueryFilters();

            if (currentUser.Role != Role.SuperAdmin && currentUser.Role != Role.Admin)
                query = query.Where(x => x.Audits.Find(c => c.Type == LogTypeEnum.Create).UserId == currentUser.Id);

            var models = await query.ToListAsync().ConfigureAwait(false);
            if (models?.Any() != true)
                return default;

            var viewModels = models?.Select(x => x.Map<Item, ItemViewModel>()).ToList();
            if (viewModels?.Any() != true)
                return default;

            var today = viewModels.Where(x => x.Logs?.Create?.DateTime.Date == DateTime.Today.Date).ToList();
            var thisWeek = viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= DateTime.Today.AddDays(-7).Date).ToList();
            var thisMonth = viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= DateTime.Today.AddMonths(-1).Date).ToList();

            var result = new StatisticsViewModel
            {
                Today = today?.Select(Map).ToList(),
                ThisMonth = thisMonth?.Select(Map).ToList(),
                ThisWeek = thisWeek?.Select(Map).ToList()
            };

            return result;
        }
    }
}