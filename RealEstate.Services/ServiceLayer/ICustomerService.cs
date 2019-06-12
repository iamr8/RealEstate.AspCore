using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface ICustomerService
    {
        Task<MethodStatus<Customer>> CustomerAddAsync(CustomerInputViewModel model, bool isForApplicantUse, bool save);

        Task<MethodStatus<Applicant>> ApplicantAddOrUpdateAsync(ApplicantInputViewModel model, bool update, bool save);

        Task<CustomerJsonViewModel> CustomerJsonAsync(string id);

        Task<MethodStatus<Applicant>> ApplicantUpdateAsync(ApplicantInputViewModel model, bool save);

        Task<OwnershipInputViewModel> OwnershipInputAsync(string customerId);

        Task<MethodStatus<Ownership>> OwnershipAddOrUpdateAsync(OwnershipInputViewModel model, bool update, bool save);

        Task<List<CustomerJsonViewModel>> CustomerListAsync(string name, string mobile);

        Task CleanDuplicatesAsync();

        Task<OwnershipJsonViewModel> OwnershipJsonAsync(string id);

        Task<MethodStatus<Customer>> CustomerAddOrUpdateAsync(CustomerInputViewModel model, bool update, bool save);

        Task<MethodStatus<Applicant>> ApplicantAddAsync(ApplicantInputViewModel model, string itemId, bool save);

        Task<MethodStatus<Ownership>> OwnershipAddAsync(Ownership model, bool save);

        Task<PaginationViewModel<CustomerViewModel>> CustomerListAsync(CustomerSearchViewModel searchModel, bool cleanDuplicates = false);

        Task<MethodStatus<Ownership>> OwnershipAddAsync(OwnershipInputViewModel model, bool save);

        Task<MethodStatus<Customer>> CustomerUpdateAsync(CustomerInputViewModel model, bool save);

        Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(ApplicantSearchViewModel searchModel);

        Task<ApplicantInputViewModel> ApplicantInputAsync(string id);

        Task<List<ItemCustomerJsonViewModel>> ListJsonAsync(string itemId);

        Task<List<CustomerViewModel>> CustomerListAsync();

        Task<Applicant> ApplicantEntityAsync(string id);

        Task<StatusEnum> ApplicantRemoveAsync(string id);

        Task<MethodStatus<Ownership>> OwnershipPlugPropertyAsync(string ownerId, string propertyOwnershipId, bool save);

        Task<CustomerInputViewModel> CustomerInputAsync(string customerId);

        Task<Customer> CustomerEntityAsync(string id, string mobile, bool includeApplicants = true, bool includeOwnerships = true, bool includeSmses = true);

        Task<MethodStatus<Applicant>> ApplicantPlugItemRequestAsync(string applicantId, string dealId, bool save);

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

        public async Task<CustomerInputViewModel> CustomerInputAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return default;

            var customer = await _customers.FirstOrDefaultAsync(x => x.Id == customerId).ConfigureAwait(false);
            if (customer == null)
                return default;

            var result = new CustomerInputViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Address = customer.Address,
                Mobile = customer.MobileNumber,
                Phone = customer.PhoneNumber,
            };
            return result;
        }

        public async Task<OwnershipInputViewModel> OwnershipInputAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return default;

            var ownership = await _ownerships.FirstOrDefaultAsync(x => x.CustomerId == customerId).ConfigureAwait(false);
            if (ownership == null)
                return default;

            var result = new OwnershipInputViewModel
            {
                Id = ownership.CustomerId,
                Name = ownership.Customer.Name,
                Address = ownership.Customer.Address,
                Mobile = ownership.Customer.MobileNumber,
                Phone = ownership.Customer.PhoneNumber,
                Description = ownership.Description,
                Dong = ownership.Dong
            };
            return result;
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

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<List<CustomerJsonViewModel>> CustomerListAsync(string name, string mobile)
        {
            var query = _customers
                .WhereNotDeleted()
                .WhereItIsPublic()
                .Where(x => EF.Functions.Like(x.Name, name.Like())
                || EF.Functions.Like(x.MobileNumber, mobile.Like()));

            var customers = await query.ToListAsync().ConfigureAwait(false);
            var cust = customers.Map<Customer, CustomerViewModel>();
            return cust?.Select(x => new CustomerJsonViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Mobile = x.Mobile,
                Phone = x.Phone
            }).ToList();
        }

        public async Task<List<CustomerViewModel>> CustomerListAsync()
        {
            var query = _customers
                .WhereNotDeleted()
                .WhereItIsPublic();

            var customers = await query.ToListAsync().ConfigureAwait(false);
            return customers.Map<Customer, CustomerViewModel>();
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

        public async Task<CustomerJsonViewModel> CustomerJsonAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var entity = await _customers.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (entity == null) return null;

            var viewModel = entity.Map<CustomerViewModel>();
            if (viewModel == null)
                return default;

            return new CustomerJsonViewModel
            {
                Name = viewModel.Name,
                Mobile = viewModel.Mobile,
                Id = viewModel.Id,
                Address = viewModel.Address,
                Phone = viewModel.Phone
            };
        }

        public async Task<OwnershipJsonViewModel> OwnershipJsonAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var entity = await OwnershipEntityAsync(id).ConfigureAwait(false);
            if (entity == null) return null;

            var viewModel = entity.Map<OwnershipViewModel>();
            if (viewModel == null)
                return default;

            return new OwnershipJsonViewModel
            {
                CustomerId = viewModel.Customer?.Id,
                Name = viewModel.Customer?.Name,
                Dong = viewModel.Dong,
                Mobile = viewModel.Customer?.Mobile
            };
        }

        public async Task<StatusEnum> CustomerRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _customers.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                .ConfigureAwait(false);

            return result;
        }

        public async Task<StatusEnum> ApplicantRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await ApplicantEntityAsync(id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                .ConfigureAwait(false);
            if (result != StatusEnum.Success)
                return result;

            return result;
        }

        public async Task<MethodStatus<Applicant>> ApplicantPlugItemRequestAsync(string applicantId, string itemId, bool save)
        {
            if (string.IsNullOrEmpty(applicantId) || string.IsNullOrEmpty(itemId))
                return new MethodStatus<Applicant>(StatusEnum.ParamIsNull, null);

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
            var viewModel = entity?.Map<ApplicantViewModel>();
            if (viewModel == null)
                return default;

            var customer = viewModel.Customer;
            var result = new ApplicantInputViewModel
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Name = customer?.Name,
                Description = viewModel.Description,
                Address = customer?.Address,
                Mobile = customer?.Mobile,
                Phone = customer?.Phone,
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
            var query = _baseService.CheckDeletedItemsPrevillege(_applicants, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<ApplicantViewModel>();

            query = query
                .Where(x => x.UserId == currentUser.Id)
                .Where(x => x.Item.DealRequests.All(c => c.DealId == null));

            var result = await _baseService.PaginateAsync(query, searchModel, item => item.Map<ApplicantViewModel>().ShowBasedOn(x => x.Customer),
                Task.FromResult(false));

            return result;
        }

        private async Task<bool> CheckDuplicatesAsync()
        {
            var groups = await _customers.IgnoreQueryFilters()
                .GroupBy(x => new
                {
                    x.MobileNumber
                }).AnyAsync(x => x.Count() > 1);
            return groups;
        }

        public async Task CleanDuplicatesAsync()
        {
            var groups = await _customers.IgnoreQueryFilters()
                .GroupBy(x => new
                {
                    x.MobileNumber
                }).Where(x => x.Count() > 1).ToListAsync();
            if (groups?.Any() != true)
                return;

            foreach (var groupedCustomer in groups)
            {
                var cnt = groupedCustomer.Count();
                var customerBest = groupedCustomer.Any(x => x.Name != "مالک")
                    ? groupedCustomer.FirstOrDefault(x => x.Name != "مالک")
                    : groupedCustomer.FirstOrDefault();
                if (customerBest == null)
                    continue;

                var customers = groupedCustomer.Except(new[]
                {
                    customerBest
                }).ToList();
                var customerId = customerBest.Id;
                foreach (var customer in customers)
                {
                    var ownerships = customer.Ownerships;
                    if (ownerships?.Any() == true)
                    {
                        foreach (var ownership in ownerships)
                        {
                            var ownershipInDb = await _ownerships.FirstOrDefaultAsync(x => x.Id == ownership.Id);
                            await _baseService.UpdateAsync(ownershipInDb,
                                currentUser =>
                                {
                                    ownershipInDb.CustomerId = customerId;
                                }, null, false, StatusEnum.OwnershipIsNull);
                        }
                    }

                    var customerInDb = await _customers.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == customer.Id);
                    if (customerInDb == null)
                        continue;

                    await _baseService.RemoveAsync(customerInDb, null, DeleteEnum.Delete, false);
                }
            }

            await _baseService.SaveChangesAsync();
        }

        public async Task<PaginationViewModel<CustomerViewModel>> CustomerListAsync(CustomerSearchViewModel searchModel, bool cleanDuplicates = false)
        {
            if (cleanDuplicates && _baseService.IsAllowed(Role.SuperAdmin))
                await CleanDuplicatesAsync();

            var query = _baseService.CheckDeletedItemsPrevillege(_customers, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<CustomerViewModel>();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Id))
                    query = query.Where(x => x.Id == searchModel.Id);

                if (!string.IsNullOrEmpty(searchModel.Address))
                    query = query.Where(x => EF.Functions.Like(x.Address, searchModel.Address.Like()));

                if (!string.IsNullOrEmpty(searchModel.Mobile))
                    query = query.Where(x => EF.Functions.Like(x.MobileNumber, searchModel.Mobile.Like()));

                if (!string.IsNullOrEmpty(searchModel.Name))
                    query = query.Where(x => EF.Functions.Like(x.Name, searchModel.Name.Like()));

                if (!string.IsNullOrEmpty(searchModel.Phone))
                    query = query.Where(x => EF.Functions.Like(x.PhoneNumber, searchModel.Phone.Like()));

                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<CustomerViewModel>(), CheckDuplicatesAsync(), currentUser);

            return result;
        }

        public async Task<MethodStatus<Ownership>> OwnershipPlugPropertyAsync(string ownerId, string propertyOwnershipId, bool save)
        {
            if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(propertyOwnershipId))
                return new MethodStatus<Ownership>(StatusEnum.ParamIsNull, null);

            var ownership = await OwnershipEntityAsync(ownerId).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(ownership,
                currentUser => ownership.PropertyOwnershipId = propertyOwnershipId,
                null, save, StatusEnum.OwnershipIsNull
            ).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<MethodStatus<Applicant>> ApplicantUpdateAsync(ApplicantInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Applicant>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Applicant>(StatusEnum.IdIsNull, null);

            var entity = await ApplicantEntityAsync(model.Id).ConfigureAwait(false);
            if (entity?.Customer?.IsPublic == true || entity?.Item?.DealRequests.OrderDescendingByCreationDateTime().FirstOrDefault()?.Status == DealStatusEnum.Finished)
                return new MethodStatus<Applicant>(StatusEnum.ApplicantIsNull, null);

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
                            (feature, currentUser) => new ApplicantFeature
                            {
                                ApplicantId = updatedApplicant.Id,
                                FeatureId = feature.Id,
                                Value = feature.Value,
                            }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                            null,
                            false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedApplicant, save).ConfigureAwait(false);
        }

        public async Task<MethodStatus<Applicant>> ApplicantAddAsync(ApplicantInputViewModel model, string itemId, bool save)
        {
            if (model == null)
                return new MethodStatus<Applicant>(StatusEnum.ModelIsNull, null);

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
                    return new MethodStatus<Applicant>(customerAddStatus, null);
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
                (feature, currentUser) => new ApplicantFeature
                {
                    ApplicantId = newApplicant.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                null,
                false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newApplicant, save).ConfigureAwait(false);
        }

        public async Task<MethodStatus<Ownership>> OwnershipAddAsync(Ownership model, bool save)
        {
            if (model == null)
                return new MethodStatus<Ownership>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(model, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<MethodStatus<Ownership>> OwnershipAddAsync(OwnershipInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Ownership>(StatusEnum.ModelIsNull, null);

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
                    return new MethodStatus<Ownership>(customerAddStatus, null);

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

        public async Task<MethodStatus<Customer>> CustomerAddAsync(CustomerInputViewModel model, bool isForApplicantUse, bool save)
        {
            if (model == null)
                return new MethodStatus<Customer>(StatusEnum.ModelIsNull, null);

            var existing = await CustomerEntityAsync(null, model.Mobile).ConfigureAwait(false);
            if (existing != null)
            {
                if (existing.IsPublic)
                    return new MethodStatus<Customer>(StatusEnum.AlreadyExists, existing);

                if (isForApplicantUse)
                    return new MethodStatus<Customer>(StatusEnum.Success, existing);
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

        public Task<MethodStatus<Applicant>> ApplicantAddOrUpdateAsync(ApplicantInputViewModel model, bool update, bool save)
        {
            return update
                ? ApplicantUpdateAsync(model, save)
                : ApplicantAddAsync(model, null, save);
        }

        public Task<MethodStatus<Ownership>> OwnershipAddOrUpdateAsync(OwnershipInputViewModel model, bool update, bool save)
        {
            return update
                ? OwnershipUpdateAsync(model, save)
                : OwnershipAddAsync(model, save);
        }

        public Task<MethodStatus<Customer>> CustomerAddOrUpdateAsync(CustomerInputViewModel model, bool update, bool save)
        {
            return update
                ? CustomerUpdateAsync(model, save)
                : CustomerAddAsync(model, false, save);
        }

        public async Task<MethodStatus<Ownership>> OwnershipUpdateAsync(OwnershipInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Ownership>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Ownership>(StatusEnum.IdIsNull, null);

            var entity = await _ownerships.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            if (entity == null)
                return new MethodStatus<Ownership>(StatusEnum.OwnershipIsNull, null);

            var customer = await CustomerEntityAsync(entity.CustomerId, null).ConfigureAwait(false);
            if (customer == null)
                return new MethodStatus<Ownership>(StatusEnum.CustomerIsNull, null);

            var (customerStatus, customer1) = await CustomerAddOrUpdateAsync(new CustomerInputViewModel
            {
                Address = model.Address,
                Description = model.Description,
                Mobile = model.Mobile,
                Name = model.Name,
                Phone = model.Phone
            }, true, true).ConfigureAwait(false);
            if (customerStatus != StatusEnum.Success)
                return new MethodStatus<Ownership>(StatusEnum.CustomerIsNull, null);

            return new MethodStatus<Ownership>(StatusEnum.Success, entity);
        }

        public async Task<MethodStatus<Customer>> CustomerUpdateAsync(CustomerInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Customer>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Customer>(StatusEnum.IdIsNull, null);

            var entity = await CustomerEntityAsync(model.Id, null).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                currentUser => entity.MobileNumber = model.Mobile,
                null, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }
    }
}