using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IGlobalService
    {
        Task<StatisticsViewModel> ListAsync();
    }

    public class GlobalService : IGlobalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<Item> _items;

        public GlobalService(
            IBaseService baseService,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _items = _unitOfWork.Set<Item>();
        }

        private StatisticsDetailViewModel Map(ItemViewModel item)
        {
            var result = new StatisticsDetailViewModel
            {
                ItemId = item.Id,
                ItemCategory = item.Category?.Name,
                PropertyCategory = item.Property?.Category?.Name,
                UserFullName = item.Logs.Create.UserFullName,
                UserId = item.Logs.Create.UserId
            };
            return result;
        }

        public async Task<StatisticsViewModel> ListAsync()
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

            var viewModels = models?.Select(x => x.Into<Item, ItemViewModel>(true, act =>
            {
                act.GetCategory();
                act.GetProperty(true, act2 => act2.GetCategory());
            })).ToList();
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