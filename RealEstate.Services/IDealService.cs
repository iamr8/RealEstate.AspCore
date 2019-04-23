using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IDealService
    {
        Task<(StatusEnum, Deal)> AddOrUpdateAsync(DealInputViewModel model, bool update, bool save);

        Task<(StatusEnum, Deal)> AddAsync(DealInputViewModel model, bool save);

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
                    var viewModel = alreadyFinished?.Deal?.Into<Deal, DealViewModel>(false, act =>
                    {
                        act.GetBeneficiaries(false, act2 => act2.GetUser(false, act3 => act3.GetEmployee()));
                        act.GetDealRequest();
                        act.GetDealPayments();
                    });
                    if (viewModel == null)
                        return default;

                    var result = new DealInputViewModel
                    {
                        Id = viewModel.Id,
                        Description = viewModel.Description,
                        Beneficiaries = viewModel.Beneficiaries?.Select(x => new BeneficiaryJsonViewModel
                        {
                            Id = x.Id,
                            UserId = x.User.Id,
                            UserFullName = $"{x.User.Employee.FirstName} • {x.User.Employee.LastName}",
                            TipPercent = x.TipPercent,
                            CommissionPercent = x.CommissionPercent
                        }).ToList(),
                        ItemId = entity.Id,
                        DealPayments = viewModel.DealPayments.Select(x => new DealPaymentJsonViewModel
                        {
                            Id = x.Id,
                            Text = x.Text,
                            PayDate = x.PayDate,
                            Commission = x.Commission,
                            Tip = x.Tip
                        }).ToList()
                    };

                    return result;
            }
        }

        public async Task<(StatusEnum, Deal)> AddAsync(DealInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.ItemId))
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ItemIsNull, null);

            var item = await _items.FirstOrDefaultAsync(x => x.Id == model.ItemId).ConfigureAwait(false);
            if (item == null)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ItemIsNull, null);

            var (addStatus, newDeal) = await _baseService.AddAsync(new Deal
            {
                Description = model.Description,
            },
                null,
                false).ConfigureAwait(false);

            await SyncAsync(newDeal, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newDeal, save).ConfigureAwait(false);
        }

        public async Task<StatusEnum> SyncAsync(Deal deal, DealInputViewModel model, bool save)
        {
            await _baseService.SyncAsync(
                deal.DealPayments,
                model.DealPayments,
                dealPayment => new DealPayment
                {
                    DealId = deal.Id,
                    CommissionPrice = dealPayment.Commission,
                    PayDate = dealPayment.PayDate,
                    Text = dealPayment.Text,
                    TipPrice = dealPayment.Tip,
                },
                (inDb, inModel) => inDb.DealId == inModel.Id,
                null, false).ConfigureAwait(false);

            await _baseService.SyncAsync(
                deal.Beneficiaries,
                model.Beneficiaries,
                beneficiary => new Beneficiary
                {
                    CommissionPercent = beneficiary.CommissionPercent,
                    TipPercent = beneficiary.TipPercent,
                    UserId = beneficiary.UserId,
                    DealId = deal.Id
                },
                (inDb, inModel) => inDb.UserId == inModel.UserId,
                null, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);
        }

        private async Task<(StatusEnum, Deal)> UpdateAsync(DealInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.IdIsNull, null);

            var entity = await _deals.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            var (updateStatus, updatedDeal) = await _baseService.UpdateAsync(entity,
                _ => entity.Description = model.Description,
                null,
                false, StatusEnum.PropertyIsNull).ConfigureAwait(false);

            if (updatedDeal == null)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.DealIsNull, null);

            await SyncAsync(updatedDeal, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedDeal, save).ConfigureAwait(false);
        }

        public Task<(StatusEnum, Deal)> AddOrUpdateAsync(DealInputViewModel model, bool update, bool save)
        {
            return update
                ? UpdateAsync(model, save)
                : AddAsync(model, save);
        }
    }
}