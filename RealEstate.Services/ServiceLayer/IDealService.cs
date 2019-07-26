using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IDealService
    {
        Task<MethodStatus<Deal>> AddOrUpdateAsync(DealInputViewModel model, bool update, bool save);

        Task<PaginationViewModel<DealViewModel>> ListAsync(DealSearchViewModel searchModel);

        Task<MethodStatus<Deal>> AddAsync(DealInputViewModel model, bool save);

        Task<DealInputViewModel> DealInputAsync(string itemId);
    }

    public class DealService : IDealService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly ICustomerService _customerService;
        private readonly IItemService _itemService;
        private readonly DbSet<Applicant> _applicants;
        private readonly DbSet<Customer> _customers;
        private readonly DbSet<Deal> _deals;
        private readonly DbSet<Item> _items;
        private readonly DbSet<DealRequest> _dealRequests;

        public DealService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IItemService itemService,
            ICustomerService customerService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _customerService = customerService;
            _itemService = itemService;
            _applicants = _unitOfWork.Set<Applicant>();
            _customers = _unitOfWork.Set<Customer>();
            _deals = _unitOfWork.Set<Deal>();
            _items = _unitOfWork.Set<Item>();
            _dealRequests = _unitOfWork.Set<DealRequest>();
        }

        public async Task<DealInputViewModel> DealInputAsync(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return default;

            var query = from item in _items
                        let requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
                        let lastRequest = requests.FirstOrDefault()
                        where requests.Any() && (lastRequest.Status == DealStatusEnum.Requested || lastRequest.Status == DealStatusEnum.Finished)
                        where item.Id == itemId
                        select item;
            var entity = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            if (entity == null)
                return default;

            var alreadyFinished = entity.DealRequests.OrderDescendingByCreationDateTime().FirstOrDefault();
            switch (alreadyFinished?.Status)
            {
                case DealStatusEnum.Requested:
                    var result1 = new DealInputViewModel
                    {
                        ItemId = entity.Id
                    };
                    return result1;

                case DealStatusEnum.Rejected:
                case null:
                case DealStatusEnum.Finished:
                default:
                    var viewModel = alreadyFinished?.Deal?.Map<DealViewModel>();
                    if (viewModel == null)
                        return default;

                    var result = new DealInputViewModel
                    {
                        Id = viewModel.Id,
                        Description = viewModel.Description,
                        Beneficiaries = viewModel.Beneficiaries?.Select(x => new BeneficiaryJsonViewModel
                        {
                            UserId = x.User?.Id,
                            UserFullName = $"{x.User?.Employee?.FirstName} • {x.User?.Employee?.LastName}",
                            TipPercent = x.TipPercent,
                            CommissionPercent = x.CommissionPercent
                        }).ToList(),
                        ItemId = entity.Id,
                        Checks = viewModel.Reminders?.Select(x => new CheckJsonViewModel
                        {
                            Date = x.Date.GregorianToPersian(true),
                            Number = x.CheckNumber,
                            Price = (decimal)x.Price,
                            Bank = x.CheckBank
                        }).ToList(),
                        Tip = (decimal)viewModel.TipPrice,
                        Commission = (decimal)viewModel.CommissionPrice,
                        Barcode = viewModel.Barcode
                    };

                    return result;
            }
        }

        public async Task<PaginationViewModel<DealViewModel>> ListAsync(DealSearchViewModel searchModel)
        {
            var query = _deals.AsQueryable();

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<DealViewModel>());
            return result;
        }

        public async Task<MethodStatus<Deal>> AddAsync(DealInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Deal>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.ItemId))
                return new MethodStatus<Deal>(StatusEnum.ItemIsNull, null);

            var item = await _items.FirstOrDefaultAsync(x => x.Id == model.ItemId).ConfigureAwait(false);
            if (item == null)
                return new MethodStatus<Deal>(StatusEnum.ItemIsNull, null);

            var lastRequest = item.DealRequests.OrderDescendingByCreationDateTime().FirstOrDefault();
            if (lastRequest == null)
                return new MethodStatus<Deal>(StatusEnum.DealRequestIsNull, null);

            if (lastRequest.Status != DealStatusEnum.Requested)
                return new MethodStatus<Deal>(StatusEnum.DealRequestIsNull, null);

            var (addStatus, newDeal) = await _baseService.AddAsync(new Deal
            {
                Description = model.Description,
                TipPrice = model.Tip,
                CommissionPrice = model.Commission,
            },
                null,
                false).ConfigureAwait(false);

            var newRequest = await _baseService.AddAsync(_ => new DealRequest
            {
                DealId = newDeal.Id,
                ItemId = item.Id,
                Status = DealStatusEnum.Finished
            }, null, false).ConfigureAwait(false);
            await SyncAsync(newDeal, model, true).ConfigureAwait(false);

            return await _baseService.SaveChangesAsync(newDeal).ConfigureAwait(false);
        }

        public async Task<StatusEnum> SyncAsync(Deal deal, DealInputViewModel model, bool save)
        {
            //await _baseService.SyncAsync(
            //    deal.Reminders,
            //    model.Checks,
            //    (reminder, currentUser) => new Reminder
            //    {
            //        DealId = deal.Id,
            //        CheckBank = reminder.Bank,
            //        CheckNumber = reminder.Number,
            //        Date = reminder.Date.PersianToGregorian(),
            //        Price = reminder.Price,
            //        UserId = currentUser.Id,
            //    },
            //    (inDb, inModel) => inDb.DealId == deal.Id && inDb.Date == inModel.Date.PersianToGregorian(),
            //    null, false).ConfigureAwait(false);

            //await _baseService.SyncAsync(
            //    deal.Beneficiaries,
            //    model.Beneficiaries,
            //    (beneficiary, currentUser) => new Beneficiary
            //    {
            //        CommissionPercent = beneficiary.CommissionPercent,
            //        TipPercent = beneficiary.TipPercent,
            //        UserId = beneficiary.UserId,
            //        DealId = deal.Id
            //    },
            //    (inDb, inModel) => inDb.UserId == inModel.UserId,
            //    null, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task<MethodStatus<Deal>> UpdateAsync(DealInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Deal>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Deal>(StatusEnum.IdIsNull, null);

            var entity = await _deals.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            var (updateStatus, updatedDeal) = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.Description = model.Description;
                    entity.TipPrice = model.Tip;
                    entity.CommissionPrice = model.Commission;
                },
                null,
                false, StatusEnum.PropertyIsNull).ConfigureAwait(false);

            if (updatedDeal == null)
                return new MethodStatus<Deal>(StatusEnum.DealIsNull, null);

            await SyncAsync(updatedDeal, model, true).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedDeal).ConfigureAwait(false);
        }

        public Task<MethodStatus<Deal>> AddOrUpdateAsync(DealInputViewModel model, bool update, bool save)
        {
            return update
                ? UpdateAsync(model, save)
                : AddAsync(model, save);
        }
    }
}