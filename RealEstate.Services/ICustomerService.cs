using Microsoft.EntityFrameworkCore;
using MoreLinq;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface ICustomerService
    {
        Task<(StatusEnum, Customer)> CustomerAddAsync(CustomerInputViewModel model, bool isForApplicantUse, bool save);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantAddOrUpdateAsync(ApplicantInputViewModel model, bool update, bool save);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantUpdateAsync(ApplicantInputViewModel model, bool save);

        Task<OwnershipJsonViewModel> OwnershipJsonAsync(string id);

        Task<(StatusEnum, Database.Tables.Customer)> CustomerAddOrUpdateAsync(CustomerInputViewModel model, bool update, bool save);

        Task<(StatusEnum, Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, string itemId, bool save);

        Task<(StatusEnum, Ownership)> OwnershipAddAsync(Ownership model, bool save);

        Task<PaginationViewModel<CustomerViewModel>> CustomerListAsync(CustomerSearchViewModel searchModel);

        Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save);

        Task<(StatusEnum, Database.Tables.Customer)> CustomerUpdateAsync(CustomerInputViewModel model, bool save);

        Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(ApplicantSearchViewModel searchModel);

        Task<ApplicantInputViewModel> ApplicantInputAsync(string id);

        Task<List<ItemCustomerJsonViewModel>> ListJsonAsync(string itemId);

        Task<List<CustomerViewModel>> CustomerListAsync();

        Task<Applicant> ApplicantEntityAsync(string id);

        Task<StatusEnum> ApplicantRemoveAsync(string id);

        Task<(StatusEnum, Ownership)> OwnershipPlugPropertyAsync(string ownerId, string propertyOwnershipId, bool save);

        Task<Customer> CustomerEntityAsync(string id, string mobile, bool includeApplicants = true, bool includeOwnerships = true, bool includeSmses = true);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantPlugItemRequestAsync(string applicantId, string dealId, bool save);

        Task<StatusEnum> CustomerRemoveAsync(string id);

        Task<Ownership> OwnershipEntityAsync(string id);
    }

    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;

        private readonly DbSet<Database.Tables.Customer> _customers;
        private readonly DbSet<Database.Tables.Applicant> _applicants;
        private readonly DbSet<Ownership> _ownerships;
        private readonly DbSet<Item> _items;

        public CustomerService(
            IUnitOfWork unitOfWork,
            IBaseService baseService

            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;

            _items = _unitOfWork.Set<Item>();
            _customers = _unitOfWork.Set<Database.Tables.Customer>();
            _applicants = _unitOfWork.Set<Database.Tables.Applicant>();
            _ownerships = _unitOfWork.Set<Ownership>();
        }

        public async Task<List<ItemCustomerJsonViewModel>> ListJsonAsync(string itemId)
        {
            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return default;

            //var applicantsQuery = await _applicants
            //    .Include(x => x.Item)
            //    .Where(x => x.UserId == currentUser.Id
            //                && (x.Item.Requestss.Count == 0 || x.Item.Requestss.OrderByDescending(c => c.DateTime).FirstOrDefault().Status != DealStatusEnum.Finished))
            //    .ToListAsync().ConfigureAwait(false);
            var customers = await _customers
                .Where(x => x.Applicants.Count == 0
                            || x.Ownerships.Count >= 0
                            || x.Applicants.Any(c => c.Item.DealRequests.Any(v => v.DealId != null))
                            || x.Applicants.Any(c => c.UserId == currentUser.Id))
                .ToListAsync()
                .ConfigureAwait(false);

            var result = new List<ItemCustomerJsonViewModel>();
            //if (applicantsQuery?.Any() == true)
            //    foreach (var applicant in applicantsQuery)
            //    {
            //        if (applicant.IsDeleted)
            //            continue;

            //        var itemCustomer = new ItemCustomerJsonViewModel
            //        {
            //            Name = applicant.Customer.Name,
            //            Mobile = applicant.Customer.MobileNumber,
            //            ApplicantId = applicant.Id,
            //            CustomerId = applicant.CustomerId
            //        };
            //        result.Add(itemCustomer);
            //    }

            if (customers?.Any() == true)
                foreach (var customer in customers)
                {
                    if (customer.IsDeleted)
                        continue;

                    var duplicate = result.Find(x => x.CustomerId == customer.Id);
                    if (duplicate != null)
                        continue;

                    var applicants = customer.Applicants.Where(x => x.UserId == currentUser.Id).ToList();
                    if (applicants?.Any() == true)
                    {
                        foreach (var applicant in applicants)
                        {
                            var itemCustomer = new ItemCustomerJsonViewModel
                            {
                                Name = customer.Name,
                                Mobile = customer.MobileNumber,
                                CustomerId = customer.Id,
                                ApplicantId = applicant.Id
                            };
                            result.Add(itemCustomer);
                        }
                    }
                    else
                    {
                        var itemCustomer = new ItemCustomerJsonViewModel
                        {
                            Name = customer.Name,
                            Mobile = customer.MobileNumber,
                            CustomerId = customer.Id,
                        };
                        result.Add(itemCustomer);
                    }
                }

            if (string.IsNullOrEmpty(itemId))
                return result;

            var item = await _items.FirstOrDefaultAsync(x => x.Id == itemId).ConfigureAwait(false);
            if (item == null)
                return result;

            if (item.IsDeleted)
                return result;

            var owners = item.Property?.CurrentOwnership?.Ownerships;
            if (owners?.Any() != true)
                return result;

            foreach (var ownership in owners)
            {
                var exist = result.Find(x => x.CustomerId == ownership.CustomerId);
                if (exist == null)
                    continue;

                if (string.IsNullOrEmpty(exist.ApplicantId))
                    result.Remove(exist);
            }

            return result;
        }

        public async Task<Database.Tables.Customer> CustomerEntityAsync(string id, string mobile, bool includeApplicants = true, bool includeOwnerships = true, bool includeSmses = true)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var query = _customers.AsQueryable();

            if (!string.IsNullOrEmpty(id))
                query = query.Where(x => x.Id == id);

            if (!string.IsNullOrEmpty(mobile))
                query = query.Where(x => x.MobileNumber == mobile);

            if (includeApplicants)
                query = query.Include(x => x.Applicants);

            if (includeOwnerships)
                query = query.Include(x => x.Ownerships);

            if (includeSmses)
                query = query.Include(x => x.Smses);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<List<CustomerViewModel>> CustomerListAsync()
        {
            var query = _customers
                .WhereNotDeleted()
                .WhereItIsPublic();

            var customers = await query.ToListAsync().ConfigureAwait(false);
            return customers.Into<Customer, CustomerViewModel>();
        }

        public async Task<Applicant> ApplicantEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var query = _applicants.AsQueryable();
            var entity = await query.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<Ownership> OwnershipEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _ownerships.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<OwnershipJsonViewModel> OwnershipJsonAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var entity = await OwnershipEntityAsync(id).ConfigureAwait(false);
            if (entity == null) return null;

            var viewModel = entity.Into<Ownership, OwnershipViewModel>(false, act => act.GetCustomer());
            if (viewModel == null)
                return default;

            return new OwnershipJsonViewModel
            {
                CustomerId = viewModel.Customer.Id,
                Name = viewModel.Customer.Name,
                Dong = viewModel.Dong,
                Mobile = viewModel.Customer.Mobile
            };
        }

        public async Task<StatusEnum> CustomerRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await CustomerEntityAsync(id, null).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<StatusEnum> ApplicantRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await ApplicantEntityAsync(id).ConfigureAwait(false);
            var mobile = user.Customer.MobileNumber;
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);
            if (result != StatusEnum.Success)
                return result;

            return result;
        }

        public async Task<(StatusEnum, Database.Tables.Applicant)> ApplicantPlugItemRequestAsync(string applicantId, string itemId, bool save)
        {
            if (string.IsNullOrEmpty(applicantId) || string.IsNullOrEmpty(itemId))
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ParamIsNull, null);

            var applicant = await ApplicantEntityAsync(applicantId).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(applicant,
                currentUser => applicant.ItemId = itemId,
                null, save, StatusEnum.ApplicantIsNull
            ).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<ApplicantInputViewModel> ApplicantInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await _applicants.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var viewModel = entity?.Into<Applicant, ApplicantViewModel>(false, act =>
            {
                act.GetCustomer();
                act.GetApplicantFeatures(false, act2 => act2.GetFeature());
            });
            if (viewModel == null)
                return default;

            var result = new ApplicantInputViewModel
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Name = viewModel.Customer?.Name,
                Description = viewModel.Description,
                Address = viewModel.Customer?.Address,
                Mobile = viewModel.Customer?.Mobile,
                Phone = viewModel.Customer?.Phone,
                ApplicantFeatures = viewModel.ApplicantFeatures?.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature?.Id,
                    Name = x.Feature?.Name,
                    Value = x.Value
                }).ToList(),
            };
            return result;
        }

        public async Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(ApplicantSearchViewModel searchModel)
        {
            var models = _applicants.AsQueryable();

            if (searchModel != null)
            {
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Applicant, ApplicantViewModel>(_baseService.IsAllowed(Role.SuperAdmin), act =>
                {
                    act.GetCustomer(_baseService.IsAllowed(Role.SuperAdmin));
                    act.GetApplicantFeatures(false, act2 => act2.GetFeature());
                }).ShowBasedOn(x => x.Customer, _baseService.IsAllowed(Role.SuperAdmin))
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<PaginationViewModel<CustomerViewModel>> CustomerListAsync(CustomerSearchViewModel searchModel)
        {
            var models = _customers.AsQueryable();
            models = models.WhereItIsPublic();

            if (searchModel != null)
            {
                models = models.SearchBy(searchModel.Id, x => x.Id);
                models = models.SearchBy(searchModel.Address, x => x.Address);
                models = models.SearchBy(searchModel.Mobile, x => x.MobileNumber);
                models = models.SearchBy(searchModel.Name, x => x.Name);
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Customer, CustomerViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetApplicants(false, act2 => act2.GetCustomer());
                    act.GetOwnerships(false, act2 => act2.GetCustomer());
                })
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<(StatusEnum, Ownership)> OwnershipPlugPropertyAsync(string ownerId, string propertyOwnershipId, bool save)
        {
            if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(propertyOwnershipId))
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.ParamIsNull, null);

            var ownership = await OwnershipEntityAsync(ownerId).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(ownership,
                currentUser => ownership.PropertyOwnershipId = propertyOwnershipId,
                null, save, StatusEnum.OwnershipIsNull
            ).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<(StatusEnum, Applicant)> ApplicantUpdateAsync(ApplicantInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.IdIsNull, null);

            var entity = await ApplicantEntityAsync(model.Id).ConfigureAwait(false);
            if (entity?.Customer?.IsPublic == true || entity?.Item?.DealRequests.OrderDescendingByCreationDateTime().FirstOrDefault()?.Status == DealStatusEnum.Finished)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ApplicantIsNull, null);

            var (updateStatus, updatedApplicant) = await _baseService.UpdateAsync(entity,
                currentUser =>
                {
                    entity.Description = model.Description;
                    entity.Type = model.Type;
                }, null, false, StatusEnum.UserIsNull).ConfigureAwait(false);

            var customer = updatedApplicant.Customer;
            var (updateCustomerStatus, updatedCustomer) = await _baseService.UpdateAsync(customer,
                currentUser =>
                {
                    customer.Address = model.Address;
                    customer.MobileNumber = model.Mobile;
                    customer.Name = model.Name;
                    customer.PhoneNumber = model.Phone;
                }, null, false, StatusEnum.CustomerIsNull).ConfigureAwait(false);

            var syncFeaturesStatus = await _baseService.SyncAsync(
                            updatedApplicant.ApplicantFeatures,
                            model.ApplicantFeatures,
                            feature => new ApplicantFeature
                            {
                                ApplicantId = updatedApplicant.Id,
                                FeatureId = feature.Id,
                                Value = feature.Value,
                            }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                            null,
                            false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedApplicant, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, string itemId, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ModelIsNull, null);

            var customer = await CustomerEntityAsync(null, model.Mobile).ConfigureAwait(false);
            if (customer == null)
            {
                var (customerAddStatus, newCustomer) = await CustomerAddAsync(new CustomerInputViewModel
                {
                    Mobile = model.Mobile,
                    Address = model.Address,
                    Name = model.Name,
                    Phone = model.Phone,
                    Id = model.Id
                }, true, true).ConfigureAwait(false);
                if (customerAddStatus == StatusEnum.Success)
                    customer = newCustomer;
                else
                    return new ValueTuple<StatusEnum, Applicant>(customerAddStatus, null);
            }

            var (addStatus, newApplicant) = await _baseService.AddAsync(currentUser => new Applicant
            {
                CustomerId = customer.Id,
                UserId = currentUser.Id,
                Type = model.Type,
                Description = model.Description,
                ItemId = !string.IsNullOrEmpty(itemId) ? itemId : null
            },
                null,
                false).ConfigureAwait(false);

            var syncFeaturesStatus = await _baseService.SyncAsync(
                newApplicant.ApplicantFeatures,
                model.ApplicantFeatures,
                feature => new ApplicantFeature
                {
                    ApplicantId = newApplicant.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                null,
                false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newApplicant, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Ownership)> OwnershipAddAsync(Ownership model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(model, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.ModelIsNull, null);

            var existingCustomer = await CustomerEntityAsync(null, model.Mobile).ConfigureAwait(false);
            if (existingCustomer == null)
            {
                var (customerAddStatus, newCustomer) = await CustomerAddAsync(new CustomerInputViewModel
                {
                    Mobile = model.Mobile,
                    Address = model.Address,
                    Name = model.Name,
                    Phone = model.Phone,
                }, false, true).ConfigureAwait(false);
                if (customerAddStatus != StatusEnum.Success)
                    return new ValueTuple<StatusEnum, Ownership>(customerAddStatus, null);

                existingCustomer = newCustomer;
            }

            var addStatus = await _baseService.AddAsync(new Ownership
            {
                CustomerId = existingCustomer.Id,
                Dong = model.Dong,
                Description = model.Description,
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, Customer)> CustomerAddAsync(CustomerInputViewModel model, bool isForApplicantUse, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Customer>(StatusEnum.ModelIsNull, null);

            var existing = await CustomerEntityAsync(null, model.Mobile).ConfigureAwait(false);
            if (existing != null)
            {
                if (existing.IsPublic)
                    return new ValueTuple<StatusEnum, Customer>(StatusEnum.AlreadyExists, existing);

                if (isForApplicantUse)
                    return new ValueTuple<StatusEnum, Customer>(StatusEnum.Success, existing);
            }

            var addStatus = await _baseService.AddAsync(new Customer
            {
                MobileNumber = model.Mobile,
                Name = model.Name,
                Address = model.Address,
                PhoneNumber = model.Phone,
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public Task<(StatusEnum, Applicant)> ApplicantAddOrUpdateAsync(ApplicantInputViewModel model, bool update, bool save)
        {
            return update
                ? ApplicantUpdateAsync(model, save)
                : ApplicantAddAsync(model, null, save);
        }

        public Task<(StatusEnum, Customer)> CustomerAddOrUpdateAsync(CustomerInputViewModel model, bool update, bool save)
        {
            return update
                ? CustomerUpdateAsync(model, save)
                : CustomerAddAsync(model, false, save);
        }

        public async Task<(StatusEnum, Customer)> CustomerUpdateAsync(CustomerInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Customer>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Customer>(StatusEnum.IdIsNull, null);

            var entity = await CustomerEntityAsync(model.Id, null).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                currentUser => entity.MobileNumber = model.Mobile,
                null, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }
    }
}