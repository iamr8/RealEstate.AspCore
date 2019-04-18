using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Services.Extensions;

namespace RealEstate.Services
{
    public interface IDealService
    {
        Task<PaginationViewModel<DealViewModel>> RequestListAsync(int pageNo);

        Task<(StatusEnum, Deal)> RequestAsync(ItemRequestInputViewModel model, bool save);
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
        }

        public async Task<PaginationViewModel<DealViewModel>> RequestListAsync(int pageNo)
        {
            var query = _deals.AsQueryable();
            query = query.Where(x => x.Status == DealStatusEnum.Requested);

            var result = await _baseService.PaginateAsync(query, pageNo,
                item => item.Into<Deal, DealViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetItem(false,
                        act2 => act2.GetProperty(false, act3 => act3.GetPropertyOwnerships(false, act4 => act4.GetOwnerships(false, act5 => act5.GetCustomer()))));
                    act.GetApplicants(false, act6 => act6.GetCustomer());
                })).ConfigureAwait(false);
            return result;
        }

        public async Task<(StatusEnum, Deal)> RequestAsync(ItemRequestInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ModelIsNull, null);

            if (model?.Customers?.Any() != true)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ApplicantsEmpty, null);

            var (requestAddStatus, newRequest) = await _baseService.AddAsync(new Deal
            {
                Description = model.Description,
                ItemId = model.ItemId,
                Status = DealStatusEnum.Requested
            }, null, false).ConfigureAwait(false);
            if (requestAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ItemRequestIsNull, null);

            await SyncApplicantsAsync(newRequest, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newRequest, save).ConfigureAwait(false);
        }

        public async Task<StatusEnum> SyncApplicantsAsync(Deal deal, ItemRequestInputViewModel model, bool save)
        {
            var allowedCustomers = await _customerService.ListJsonAsync().ConfigureAwait(false);
            if (allowedCustomers?.Any() != true)
                return StatusEnum.CustomerIsNull;

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null) return StatusEnum.UserIsNull;

            var mustBeLeft = deal.Applicants.Where(ent => model.Customers.Any(mdl => ent.CustomerId == mdl.CustomerId)).ToList();
            var mustBeRemoved = deal.Applicants.Where(x => !mustBeLeft.Contains(x)).ToList();
            if (mustBeRemoved.Count > 0)
            {
                foreach (var redundant in mustBeRemoved)
                {
                    await _baseService.UpdateAsync(redundant,
                        () => redundant.DealId = null, null, false, StatusEnum.ApplicantIsNull).ConfigureAwait(false);
                }
            }

            if (model.Customers?.Any() != true)
                return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);

            foreach (var customr in model.Customers)
            {
                var source = deal.Applicants.FirstOrDefault(ent => ent.CustomerId == customr.CustomerId);
                if (source == null)
                {
                    var cnt = await _customers.FirstOrDefaultAsync(x => x.Id == customr.CustomerId).ConfigureAwait(false);
                    if (cnt == null)
                        continue;

                    var (applicantAddStatus, newApplicant) = await _customerService.ApplicantAddAsync(new ApplicantInputViewModel
                    {
                        Address = cnt.Address,
                        Mobile = cnt.MobileNumber,
                        Name = cnt.Name,
                        Phone = cnt.PhoneNumber,
                        Type = ApplicantTypeEnum.Applicant
                    }, deal.Id, false).ConfigureAwait(false);
                }
                else
                {
                    var applicant = await _applicants.FirstOrDefaultAsync(x => x.Id == customr.ApplicantId).ConfigureAwait(false);
                    await _baseService.UpdateAsync(applicant,
                        () => applicant.DealId = deal.Id, null, false, StatusEnum.ApplicantIsNull).ConfigureAwait(false);
                }
            }

            return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);
        }
    }
}