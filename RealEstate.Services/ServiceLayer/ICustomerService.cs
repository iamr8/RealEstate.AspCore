using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;

namespace RealEstate.Services.ServiceLayer
{
    public interface ICustomerService
    {
        Task<MethodStatus<Ownership>> OwnershipAsync(OwnershipInputViewModel model, string propertyOwnershipId);

        Task<StatusEnum> ShiftApplicantAsync(TransApplicantViewModel model);

        Task<List<CustomerJsonViewModel>> CustomerListAsync(string name, string mobile);

        Task<PaginationViewModel<CustomerViewModel>> CustomerListAsync(CustomerSearchViewModel searchModel, bool loadData = true);

        Task<MethodStatus<Customer>> CustomerAsync(CustomerInputViewModel model);

        Task<MethodStatus<Applicant>> ApplicantAsync(ApplicantInputViewModel model);

        Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(ApplicantSearchViewModel searchModel, bool loadData = true);

        Task<ApplicantInputViewModel> ApplicantAsync(string id);

        Task<List<ItemCustomerJsonViewModel>> ListJsonAsync(string itemId);

        Task<StatusEnum> ApplicantRemoveAsync(string id);

        Task<CustomerInputViewModel> CustomerAsync(string customerId);

        Task<StatusEnum> CustomerRemoveAsync(string id);
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
            _customers = _unitOfWork.Set<Customer>();
            _applicants = _unitOfWork.Set<Applicant>();
            _ownerships = _unitOfWork.Set<Ownership>();
        }

        public async Task<CustomerInputViewModel> CustomerAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return default;

            var customer = await _customers.FirstOrDefaultAsync(x => x.Id == customerId);
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

        public async Task<StatusEnum> ShiftApplicantAsync(TransApplicantViewModel model)
        {
            if (model == null)
                return StatusEnum.ModelIsNull;

            var query = await _applicants.FirstOrDefaultAsync(x => x.Id == model.ApplicantId);
            if (query == null)
                return StatusEnum.ApplicantIsNull;

            var status = await _baseService.UpdateAsync(query,
                currentUser => query.UserId = model.NewUserId,
                null,
                true,
                StatusEnum.ApplicantIsNull);
            return StatusEnum.Success;
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
            //    .ToListAsync();
            var customers = await _customers
                .Where(x => x.Applicants.Count == 0
                            || x.Applicants.Any(c => c.Item.DealRequests.Any(v => v.DealId != null))
                            || x.Applicants.Any(c => c.UserId == currentUser.Id))
                .ToListAsync()
                ;

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

            var item = await _items.FirstOrDefaultAsync(x => x.Id == itemId);
            if (item == null)
                return result;

            if (item.IsDeleted)
                return result;

            var owners = item.Property?.CurrentPropertyOwnership?.Ownerships;
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

        public async Task<List<CustomerJsonViewModel>> CustomerListAsync(string name, string mobile)
        {
            var query = _customers
                .WhereItIsPublic()
                .Where(x => EF.Functions.Like(x.Name, name.Like())
                || EF.Functions.Like(x.MobileNumber, mobile.Like()));

            var customers = await query.ToListAsync();
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

        public async Task<StatusEnum> CustomerRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _customers.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<StatusEnum> ApplicantRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await _applicants.FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;
            if (result != StatusEnum.Success)
                return result;

            return result;
        }

        public async Task<ApplicantInputViewModel> ApplicantAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await _applicants
                .Include(x => x.ApplicantFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.Customer)
                .Where(x => x.Id == id)
                .Cacheable()
                .FirstOrDefaultAsync();
            var viewModel = entity?.Map<ApplicantViewModel>(act =>
            {
                act.IncludeAs<Applicant, ApplicantFeature, ApplicantFeatureViewModel>(_unitOfWork, x => x.ApplicantFeatures,
                    act2 => act2.IncludeAs<ApplicantFeature, Feature, FeatureViewModel>(_unitOfWork, c => c.Feature));
                act.IncludeAs<Applicant, Customer, CustomerViewModel>(_unitOfWork, x => x.Customer);
            });
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

        public async Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(ApplicantSearchViewModel searchModel, bool loadData = true)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_applicants, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<ApplicantViewModel>();

            query = query
                .Include(x => x.Customer)
                .Include(x => x.ApplicantFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.User)
                .ThenInclude(x => x.Employee);

            if (currentUser.Role != Role.SuperAdmin)
                query = query.Where(x => x.UserId == currentUser.Id);
            //                .Where(x => x.Item.DealRequests.All(c => c.DealId == null));

            var result = await _baseService.PaginateAsync(query, searchModel, item => item.Map<ApplicantViewModel>(act =>
            {
                act.IncludeAs<Applicant, Customer, CustomerViewModel>(_unitOfWork, x => x.Customer);
                act.IncludeAs<Applicant, User, UserViewModel>(_unitOfWork, x => x.User,
                    act2 => act2.IncludeAs<User, Employee, EmployeeViewModel>(_unitOfWork, x => x.Employee));
                act.IncludeAs<Applicant, ApplicantFeature, ApplicantFeatureViewModel>(_unitOfWork, x => x.ApplicantFeatures,
                    act2 => act2.IncludeAs<ApplicantFeature, Feature, FeatureViewModel>(_unitOfWork, c => c.Feature));
            }).ShowBasedOn(x => x.Customer), loadData: loadData);

            return result;
        }

        public async Task<PaginationViewModel<CustomerViewModel>> CustomerListAsync(CustomerSearchViewModel searchModel, bool loadData = true)
        {
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
                item => item.Map<CustomerViewModel>(), loadData: loadData);

            return result;
        }

        public async Task<MethodStatus<Applicant>> ApplicantAsync(ApplicantInputViewModel model)
        {
            if (model == null)
                return new MethodStatus<Applicant>(StatusEnum.ModelIsNull);

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return new MethodStatus<Applicant>(StatusEnum.UserIsNull);

            var (customerStatus, customer, isCustomerSuccess) = await CustomerAsync(model);
            if (!isCustomerSuccess)
                return new MethodStatus<Applicant>(customerStatus);

            StatusEnum status;
            bool isSuccess;
            Applicant applicant;
            var needUpdate = false;

            if (!model.IsNew)
            {
                applicant = await _applicants.FirstOrDefaultAsync(x => x.Id == model.Id);
                needUpdate = true;
                status = StatusEnum.Success;
                isSuccess = true;
            }
            else
            {
                var existingApplicant = await _applicants
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.CustomerId == customer.Id && x.UserId == currentUser.Id && x.Type == model.Type);
                if (existingApplicant == null)
                {
                    (status, applicant, isSuccess) = await _baseService.AddAsync(new Applicant
                    {
                        CustomerId = customer.Id,
                        Description = model.Description,
                        Type = model.Type,
                        UserId = currentUser.Id,
                    }, null, true);
                }
                else
                {
                    applicant = existingApplicant;
                    needUpdate = true;
                    status = StatusEnum.Success;
                    isSuccess = true;
                }
            }

            if (needUpdate)
            {
                (status, applicant, isSuccess) = await _baseService.UpdateAsync(applicant,
                    _ => applicant.Description = model.Description,
                    null,
                    true, StatusEnum.ApplicantIsNull);
            }
            if (!isSuccess)
                return new MethodStatus<Applicant>(status);

            await ApplicantSyncAsync(applicant, model);
            return await _baseService.SaveChangesAsync(applicant);
        }

        private async Task ApplicantSyncAsync(Applicant applicant, ApplicantInputViewModel model)
        {
            await _baseService.SyncAsync(
                applicant.ApplicantFeatures,
                model.ApplicantFeatures,
                (feature, currentUser) => new ApplicantFeature
                {
                    ApplicantId = applicant.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                },
                inDb => new { inDb.FeatureId, inDb.Value },
                (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                (inDb, inModel) => inDb.Value == inModel.Value,
                (inDb, inModel) => inDb.Value = inModel.Value,
                null);
        }

        public async Task<MethodStatus<Ownership>> OwnershipAsync(OwnershipInputViewModel model, string propertyOwnershipId)
        {
            if (model == null)
                return new MethodStatus<Ownership>(StatusEnum.ModelIsNull);

            var (customerStatus, customer, isCustomerSuccess) = await CustomerAsync(model);
            if (!isCustomerSuccess)
                return new MethodStatus<Ownership>(customerStatus);

            var ownership = await _ownerships
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.CustomerId == customer.Id && x.PropertyOwnershipId == propertyOwnershipId);
            if (ownership != null)
                return new MethodStatus<Ownership>(StatusEnum.Success, ownership);

            var status = await _baseService.AddAsync(new Ownership
            {
                CustomerId = customer.Id,
                Dong = 6,
                PropertyOwnershipId = propertyOwnershipId,
            }, null, true);
            return status;
        }

        public async Task<MethodStatus<Customer>> CustomerAsync(CustomerInputViewModel model)
        {
            if (model == null)
                return new MethodStatus<Customer>(StatusEnum.ModelIsNull);

            StatusEnum status;
            Customer customer;
            var needUpdate = false;

            if (!model.IsNew)
            {
                customer = await _customers.FirstOrDefaultAsync(x => x.Id == model.Id);
                needUpdate = true;
                status = StatusEnum.Success;
            }
            else
            {
                var existingCustomer = await _customers
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.MobileNumber == model.Mobile);
                if (existingCustomer == null)
                {
                    (status, customer, _) = await _baseService.AddAsync(new Customer
                    {
                        Name = model.Name,
                        MobileNumber = model.Mobile,
                    }, null, true);
                }
                else
                {
                    customer = existingCustomer;
                    needUpdate = true;
                    status = StatusEnum.Success;
                }
            }

            if (needUpdate)
            {
                (status, customer, _) = await _baseService.UpdateAsync(customer,
                _ => customer.Name = model.Name,
                null,
                true, StatusEnum.CustomerIsNull);
            }
            return new MethodStatus<Customer>(status, customer);
        }
    }
}