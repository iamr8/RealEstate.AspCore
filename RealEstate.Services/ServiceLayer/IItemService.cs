using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Api.Request;
using RealEstate.Services.ViewModels.Api.Response;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IItemService
    {
        Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel, bool cleanDuplicates = false);

        Task<ItemOutJsonViewModel> ItemJsonAsync(string id);

        Task<StatusEnum> RequestRejectAsync(string itemId, bool save);

        Task<StatusEnum> ItemRemoveAsync(string id);

        Task<bool> ItemCheckAsync(PropertyCheckViewModel model);

        Task<ItemComplexInputViewModel> ItemInputAsync(string id);

        Task<MethodStatus<Item>> ItemAddOrUpdateAsync(ItemComplexInputViewModel model, bool update, bool save);

        Task<Response<ItemOutJsonViewModel>> ItemListAsync(ItemRequest model);

        Task<List<PropertyJsonViewModel>> ItemListAsync(string district, string category, string street);

        Task<PaginationViewModel<ItemViewModel>> RequestListAsync(DealRequestSearchViewModel searchModel);

        Task<List<ZoonkanViewModel>> ZoonkansAsync();

        Task<MethodStatus<Item>> RequestAsync(DealRequestInputViewModel model, bool save);

        Task<Item> ItemEntityAsync(string id);

        Task<ItemViewModel> ItemAsync(string id, DealStatusEnum? specificStatus);
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IBaseService _baseService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ICustomerService _customerService;
        private readonly IPropertyService _propertyService;
        private readonly IUserService _userService;
        private readonly DbSet<Item> _items;
        private readonly DbSet<Customer> _customers;
        private readonly DbSet<DealRequest> _dealRequests;
        private readonly DbSet<Applicant> _applicants;
        private readonly DbSet<Feature> _features;
        private readonly DbSet<ItemFeature> _itemFeatures;

        public ItemService(
            IBaseService baseService,
            IUnitOfWork unitOfWork,
            IPropertyService propertyService,
            IUserService userService,
            ICustomerService customerService,
            IStringLocalizer<SharedResource> localizer
        )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
            _customerService = customerService;
            _propertyService = propertyService;
            _userService = userService;
            _localizer = localizer;
            _applicants = _unitOfWork.Set<Applicant>();
            _items = _unitOfWork.Set<Item>();
            _customers = _unitOfWork.Set<Customer>();
            _dealRequests = _unitOfWork.Set<DealRequest>();
            _features = _unitOfWork.Set<Feature>();
            _itemFeatures = _unitOfWork.Set<ItemFeature>();
        }

        public async Task<List<ZoonkanViewModel>> ZoonkansAsync()
        {
            var query = _items
                .Include(x => x.Property.Pictures)
                .Include(x => x.Property.Category)
                .Include(x => x.Category)
                .Where(x => x.Property.PropertyOwnerships.Any(c => c.Ownerships.Any()))
                //.GroupJoin(_pictures, x => x.PropertyId, x => x.PropertyId, (item, pictures) => new
                //{
                //    Item = item,
                //    Pictures = pictures,
                //})
                .GroupBy(x => new
                {
                    ItemCategory = x.Category.Name,
                    PropertyCategory = x.Property.Category.Name,
                })
                .Select(x => new
                {
                    x.Key.ItemCategory,
                    x.Key.PropertyCategory,
                    Count = x.Count(),
                    //                    Pictures = from item in x
                    //                               let property = item.Property
                    //                               select new
                    //                               {
                    //                                   property.Pictures
                    //                               }
                });

            var categories = await query
                .Cacheable()
                .ToListAsync();
            if (categories?.Any() != true)
                return default;

            var result = categories.Select(x => new ZoonkanViewModel
            {
                ItemCategory = x.ItemCategory,
                PropertyCategory = x.PropertyCategory,
                Count = x.Count,
                //                Picture = x.Pictures.ToList().Select(c => c.Pictures).SelectRandom().Select(c => c.File).FirstOrDefault()
            }).ToList();
            return result;
        }

        public async Task<StatusEnum> RequestRejectAsync(string itemId, bool save)
        {
            if (string.IsNullOrEmpty(itemId))
                return StatusEnum.IdIsNull;

            var item = await _items.FirstOrDefaultAsync(x => x.Id == itemId);
            if (item == null)
                return StatusEnum.ItemIsNull;

            var (requestStatus, newState) = await RequestAddStateAsync(item.Id, DealStatusEnum.Rejected);
            return requestStatus;
        }

        private async Task<MethodStatus<DealRequest>> RequestAddStateAsync(string itemId, DealStatusEnum newState)
        {
            if (string.IsNullOrEmpty(itemId))
                return new MethodStatus<DealRequest>(StatusEnum.ItemIsNull, null);

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return new MethodStatus<DealRequest>(StatusEnum.UserIsNull, null);

            var lastRequest = await _dealRequests.OrderDescendingByCreationDateTime().Where(x => x.ItemId == itemId).FirstOrDefaultAsync();
            if (lastRequest?.Status == newState)
                return new MethodStatus<DealRequest>(StatusEnum.AlreadyExists, null);

            var addState = await _baseService.AddAsync(new DealRequest
            {
                ItemId = itemId,
                Status = newState
            }, null, true);
            return addState;
        }

        public async Task<StatusEnum> SyncApplicantsAsync(Item item, DealRequestInputViewModel model, bool save)
        {
            var allowedCustomers = await _customerService.ListJsonAsync(item.Id);
            if (allowedCustomers?.Any() != true)
                return StatusEnum.CustomerIsNull;

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null) return StatusEnum.UserIsNull;

            var mustBeLeft = item.Applicants.Where(ent => model.Customers.Any(mdl => ent.CustomerId == mdl.CustomerId)).ToList();
            var mustBeRemoved = item.Applicants.Where(x => !mustBeLeft.Contains(x)).ToList();
            if (mustBeRemoved.Count > 0)
            {
                foreach (var redundant in mustBeRemoved)
                {
                    await _baseService.UpdateAsync(redundant,
                        _ => redundant.ItemId = null,
                        null,
                        false, StatusEnum.ApplicantIsNull);
                }
            }

            if (model.Customers?.Any() != true)
                return await _baseService.SaveChangesAsync();

            foreach (var customer in model.Customers)
            {
                var source = item.Applicants.FirstOrDefault(ent => ent.CustomerId == customer.CustomerId);
                if (source == null)
                {
                    var appli = await _applicants.FirstOrDefaultAsync(x => x.CustomerId == customer.CustomerId && x.UserId == currentUser.Id);
                    if (appli != null)
                    {
                        await _baseService.UpdateAsync(appli,
                            _ => appli.ItemId = item.Id,
                            null,
                            false, StatusEnum.ApplicantIsNull);
                    }
                    else
                    {
                        var cnt = await _customers.FirstOrDefaultAsync(x => x.Id == customer.CustomerId);
                        if (cnt == null)
                            continue;

                        var (addStatus, newApplicant) = await _baseService.AddAsync(_ => new Applicant
                        {
                            CustomerId = customer.CustomerId,
                            UserId = currentUser.Id,
                            Type = ApplicantTypeEnum.Applicant,
                            ItemId = item.Id
                        },
                            null,
                            false);
                    }
                }
                else
                {
                    var applicant = await _customerService.ApplicantEntityAsync(customer.ApplicantId);
                    await _baseService.UpdateAsync(applicant,
                        _ => applicant.ItemId = item.Id, null, false, StatusEnum.ApplicantIsNull);
                }
            }

            return await _baseService.SaveChangesAsync();
        }

        public async Task<ItemOutJsonViewModel> ItemJsonAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _items
                .AsNoTracking()
                .Include(x => x.ItemFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.Category)
                .Include(x => x.Property.Pictures)
                .Include(x => x.Property.Category)
                .Include(x => x.Property.District)
                .Include(x => x.Property.PropertyFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.Property.PropertyFacilities)
                .ThenInclude(x => x.Facility)
                .Include(x => x.Property.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Customer)
                .Where(x => x.Id == id)
                .Cacheable()
                .FirstOrDefaultAsync();

            var viewModel = entity?.Map<ItemViewModel>(ent =>
            {
                ent.IncludeAs<Item, ItemFeature, ItemFeatureViewModel>(_unitOfWork, x => x.ItemFeatures,
                    ent2 => ent2.IncludeAs<ItemFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                ent.IncludeAs<Item, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                ent.IncludeAs<Item, Property, PropertyViewModel>(_unitOfWork, x => x.Property, ent2 =>
                {
                    ent2.IncludeAs<Property, Picture, PictureViewModel>(_unitOfWork, x => x.Pictures);
                    ent2.IncludeAs<Property, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                    ent2.IncludeAs<Property, District, DistrictViewModel>(_unitOfWork, x => x.District);
                    ent2.IncludeAs<Property, PropertyFeature, PropertyFeatureViewModel>(_unitOfWork, x => x.PropertyFeatures,
                        ent3 => ent3.IncludeAs<PropertyFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                    ent2.IncludeAs<Property, PropertyFacility, PropertyFacilityViewModel>(_unitOfWork, x => x.PropertyFacilities,
                        ent3 => ent3.IncludeAs<PropertyFacility, Facility, FacilityViewModel>(_unitOfWork, x => x.Facility));
                    ent2.IncludeAs<Property, PropertyOwnership, PropertyOwnershipViewModel>(_unitOfWork, x => x.PropertyOwnerships,
                        ent3 => ent3.IncludeAs<PropertyOwnership, Ownership, OwnershipViewModel>(_unitOfWork, x => x.Ownerships,
                            ent4 => ent4.IncludeAs<Ownership, Customer, CustomerViewModel>(_unitOfWork, x => x.Customer)));
                });
            });
            if (viewModel == null)
                return default;

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return default;

            var result = new ItemOutJsonViewModel
            {
                Id = viewModel.Id,
                Features = viewModel.ItemFeatures?.ToDictionary(x => x.Feature?.Name, x => x.Value),
                IsNegotiable = viewModel.IsNegotiable,
                Category = viewModel.Category?.Name,
                Description = viewModel.Description,
                Property = viewModel.Property != null
                    ? new PropertyOutJsonViewModel
                    {
                        Id = viewModel.Property.Id,
                        Pictures = viewModel.Property.Pictures?.Select(x => x.File).ToList(),
                        Category = viewModel.Property.Category?.Name,
                        Address = viewModel.Property.Address,
                        District = viewModel.Property.District?.Name,
                        Facilities = viewModel.Property.PropertyFacilities?.Select(x => x.Facility?.Name).ToList(),
                        Features = viewModel.Property.PropertyFeatures?.ToDictionary(x => x.Feature?.Name, x => x.Value),
                        Ownership = viewModel.Property.CurrentPropertyOwnership?.Ownership != null
                            ? new OwnershipOutJsonViewModel
                            {
                                Id = viewModel.Property.CurrentPropertyOwnership?.Ownership.Customer?.Id,
                                Name = viewModel.Property.CurrentPropertyOwnership?.Ownership.Customer?.Name,
                                Mobile = viewModel.Property.CurrentPropertyOwnership?.Ownership.Customer?.Mobile
                            }
                            : default
                    }
                    : default,
                Log = currentUser.Role == Role.SuperAdmin || currentUser.Role == Role.Admin ? entity.Audits.Render() : default
            };
            return result;
        }

        public async Task<Response<ItemOutJsonViewModel>> ItemListAsync(ItemRequest model)
        {
            if (model == null)
                return new Response<ItemOutJsonViewModel>
                {
                    Success = false,
                    Message = StatusEnum.Failed.GetDisplayName(),
                    BaseUrl = _userService.BaseUrl
                };
            var tokenState = await _userService.ValidateToken(model?.Token);
            if (!tokenState.Success)
                return new Response<ItemOutJsonViewModel>
                {
                    Message = tokenState.Message,
                    Success = tokenState.Success,
                    BaseUrl = tokenState.BaseUrl
                };

            if (string.IsNullOrEmpty(model.ItemCategory))
                return new Response<ItemOutJsonViewModel>
                {
                    Success = false,
                    Message = "دسته بندی مورد را پر کنید",
                    BaseUrl = tokenState.BaseUrl
                };

            if (string.IsNullOrEmpty(model.PropertyCategory))
                return new Response<ItemOutJsonViewModel>
                {
                    Success = false,
                    Message = "دسته بندی ملک را پر کنید",
                    BaseUrl = tokenState.BaseUrl
                };

            var query = _items.AsQueryable();
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.Category);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.District);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.Pictures);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.PropertyFacilities)
                .ThenInclude(x => x.Facility);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.PropertyFeatures)
                .ThenInclude(x => x.Feature);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Customer);
            query = query.Include(x => x.Category);
            query = query.Include(x => x.DealRequests);
            query = query.Include(x => x.ItemFeatures)
                .ThenInclude(x => x.Feature);
            query = query.Include(x => x.Applicants)
                .ThenInclude(x => x.Customer);

            query = query.Where(x => x.Category.Name == model.ItemCategory);
            query = query.Where(x => x.Property.Category.Name == model.PropertyCategory);
            query = query.Where(x => x.Category.UserItemCategories.Any(c => c.UserId == tokenState.UserId));
            query = query.Where(x => x.Property.Category.UserPropertyCategories.Any(c => c.UserId == tokenState.UserId));

            var items = await query.Cacheable().ToListAsync();
            if (items?.Any() != true)
                return new Response<ItemOutJsonViewModel>
                {
                    Message = _localizer[SharedResource.NoItemToShow],
                    Success = true,
                    BaseUrl = tokenState.BaseUrl
                };

            var result = from item in items
                         let converted = item.Map<ItemViewModel>(ent =>
                         {
                             ent.IncludeAs<Item, Property, PropertyViewModel>(_unitOfWork, x => x.Property, ent2 =>
                             {
                                 ent2.IncludeAs<Property, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                                 ent2.IncludeAs<Property, District, DistrictViewModel>(_unitOfWork, x => x.District);
                                 ent2.IncludeAs<Property, PropertyFacility, PropertyFacilityViewModel>(_unitOfWork, x => x.PropertyFacilities,
                                     ent3 => ent3.IncludeAs<PropertyFacility, Facility, FacilityViewModel>(_unitOfWork, x => x.Facility));
                                 ent2.IncludeAs<Property, PropertyFeature, PropertyFeatureViewModel>(_unitOfWork, x => x.PropertyFeatures,
                                     ent3 => ent3.IncludeAs<PropertyFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                                 ent2.IncludeAs<Property, PropertyOwnership, PropertyOwnershipViewModel>(_unitOfWork, x => x.PropertyOwnerships,
                                     ent3 => ent3.IncludeAs<PropertyOwnership, Ownership, OwnershipViewModel>(_unitOfWork, x => x.Ownerships,
                                         ent4 => ent4.IncludeAs<Ownership, Customer, CustomerViewModel>(_unitOfWork, x => x.Customer)));
                             });
                             ent.IncludeAs<Item, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                             ent.IncludeAs<Item, ItemFeature, ItemFeatureViewModel>(_unitOfWork, x => x.ItemFeatures,
                                 ent2 => ent2.IncludeAs<ItemFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                         })
                         where converted != null
                         let property = converted.Property
                         where property != null
                         select new ItemOutJsonViewModel
                         {
                             Property = new PropertyOutJsonViewModel
                             {
                                 Address = property.Address,
                                 Category = property.Category?.Name,
                                 District = property.District?.Name,
                                 Facilities = property.PropertyFacilities?.Select(x => x.Facility?.Name).ToList(),
                                 Ownership = property.CurrentPropertyOwnership?.Ownership != null ?
                             new OwnershipOutJsonViewModel
                             {
                                 Name = property.CurrentPropertyOwnership?.Ownership.Customer?.Name,
                                 Id = property.CurrentPropertyOwnership?.Ownership.Customer?.Id,
                                 Mobile = property.CurrentPropertyOwnership?.Ownership.Customer?.Mobile
                             } :
                             default,
                                 Features = property.PropertyFeatures.ToDictionary(x => x.Feature?.Name, x => x.Value),
                             },
                             Category = converted.Category?.Name,
                             Features = converted.ItemFeatures.ToDictionary(x => x.Feature?.Name, x => x.Value.CurrencyToWords())
                         };

            return new Response<ItemOutJsonViewModel>
            {
                Message = StatusEnum.Success.GetDisplayName(),
                Success = true,
                Result = result.ToList(),
                BaseUrl = tokenState.BaseUrl
            };
        }

        public async Task<List<PropertyJsonViewModel>> ItemListAsync(string district, string category, string street)
        {
            if (string.IsNullOrEmpty(district) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(street))
                return default;

            //            var query1 = _items.AsNoTracking()
            //                .Include(x => x.Property)
            //                .ThenInclude(x => x.District)
            //                .Include(x => x.Property)
            //                .ThenInclude(x => x.Category);

            var query = from item in _items
                        where (EF.Functions.Like(item.Property.Street, street.Like()) ||
                                EF.Functions.Like(item.Property.Alley, street.Like())) &&
                            item.Property.District.Name == district &&
                            item.Property.Category.Name == category
                        select item;

            var models = await query.Select(x => x.Property).ToListAsync();
            if (models?.Any() != true)
                return default;

            var result = models.Select(property => new PropertyJsonViewModel
            {
                Id = property.Id,
                District = property.District?.Name,
                Address = property.Address,
                Category = property.Category?.Name
            }).ToList();
            return result;
        }

        public async Task<bool> ItemCheckAsync(PropertyCheckViewModel model)
        {
            if (model == null)
                return false;

            if (string.IsNullOrEmpty(model.District) || string.IsNullOrEmpty(model.Category) || string.IsNullOrEmpty(model.Street))
                return false;

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return default;

            var query = from item in _items
                        let requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
                        let lastRequest = requests.FirstOrDefault()
                        where !requests.Any() || lastRequest.Status == DealStatusEnum.Rejected
                        let itemCategory = item.Category
                        let property = item.Property
                        let propertyCategory = property.Category
                        where itemCategory.UserItemCategories.Any(userItemCategory =>
                           userItemCategory.UserId == currentUser.Id && userItemCategory.CategoryId == itemCategory.Id)
                        where propertyCategory.UserPropertyCategories.Any(userPropertyCategory =>
                           userPropertyCategory.UserId == currentUser.Id && userPropertyCategory.CategoryId == propertyCategory.Id)
                        where (EF.Functions.Like(item.Property.Street, model.Street.Like()) ||
                                EF.Functions.Like(item.Property.Alley, model.Street.Like())) &&
                            item.Property.District.Name == model.District &&
                            item.Property.Category.Name == model.Category
                        select item;

            var models = await query.Select(x => x.Property).FirstOrDefaultAsync();
            if (models != null)
                return true;

            return false;
        }

        private async Task<bool> CheckDuplicatesAsync()
        {
            var groups = await _items.IgnoreQueryFilters()
                .Include(x => x.Property)
                .GroupBy(x => new
                {
                    x.CategoryId,
                    x.Property
                }).Where(x => x.Count() > 1).AnyAsync();
            return groups;
        }

        private async Task CleanComplexDuplicatesAsync()
        {
            await _propertyService.CleanDuplicatesAsync();
            await CleanDuplicatesAsync();
            await _customerService.CleanDuplicatesAsync();
        }

        private async Task CleanDuplicatesAsync()
        {
            var groups = await _items.IgnoreQueryFilters()
                .OrderDescendingByCreationDateTime()
                .Include(x => x.Property)
                .GroupBy(x => new
                {
                    x.CategoryId,
                    x.Property
                }).Where(x => x.Count() > 1).ToListAsync();
            if (groups?.Any() == true)
            {
                foreach (var groupedItem in groups)
                {
                    foreach (var item in groupedItem.Skip(1))
                    {
                        var itemInDb = await _items.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == item.Id);
                        if (itemInDb == null)
                            continue;

                        var itemFeatures = await _itemFeatures.Where(x => x.ItemId == itemInDb.Id).ToListAsync();
                        if (itemFeatures?.Any() == true)
                        {
                            foreach (var itemFeature in itemFeatures)
                            {
                                var itemFeatureInDb = await _itemFeatures.FirstOrDefaultAsync(x => x.Id == itemFeature.Id);
                                await _baseService.RemoveAsync(itemFeatureInDb, null, DeleteEnum.Delete, false);
                            }
                        }

                        await _baseService.RemoveAsync(itemInDb, null, DeleteEnum.Delete, false);
                    }
                }
            }

            await _baseService.SaveChangesAsync();
        }

        public async Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel, bool cleanDuplicates = false)
        {
            if (cleanDuplicates && _baseService.IsAllowed(Role.SuperAdmin))
                await CleanComplexDuplicatesAsync();

            var query = _baseService.CheckDeletedItemsPrevillege(_items, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<ItemViewModel>();

            query = query.Include(x => x.Property.Category)
                .Include(x => x.Property.District)
                .Include(x => x.Property.Pictures)
                .Include(x => x.Property.PropertyFacilities)
                .ThenInclude(x => x.Facility)
                .Include(x => x.Property.PropertyFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.Property.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Customer)
                .Include(x => x.Category)
                .Include(x => x.DealRequests)
                .Include(x => x.ItemFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.Applicants)
                .ThenInclude(x => x.Customer);

            //query = from item in query
            //        let requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
            //        let lastRequest = requests.FirstOrDefault()
            //        where !requests.Any() || lastRequest.Status == DealStatusEnum.Rejected
            //        select item;

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.HasFeature))
                {
                    query = query.Where(x =>
                       x.Property.PropertyFeatures.Any(c => c.FeatureId == searchModel.HasFeature) || x.ItemFeatures.Any(c => c.FeatureId == searchModel.HasFeature));
                }

                if (!string.IsNullOrEmpty(searchModel.Owner))
                {
                    query = query.Where(x =>
                       x.Property.PropertyOwnerships.Any(c => c.Ownerships.Any(v => EF.Functions.Like(v.Customer.Name, searchModel.Owner.Like()))));
                }

                if (!string.IsNullOrEmpty(searchModel.OwnerMobile))
                {
                    query = query.Where(x =>
                       x.Property.PropertyOwnerships.Any(c => c.Ownerships.Any(v => EF.Functions.Like(v.Customer.MobileNumber, searchModel.OwnerMobile.Like()))));
                }

                if (!string.IsNullOrEmpty(searchModel.Street))
                {
                    query = query.Where(x => EF.Functions.Like(x.Property.Street, searchModel.Street.Like()));
                }

                if (!string.IsNullOrEmpty(searchModel.ItemId))
                {
                    query = query.Where(x => x.Id == searchModel.ItemId);
                }

                if (!string.IsNullOrEmpty(searchModel.ItemCategory))
                {
                    query = query.Where(x => x.Category.Name == searchModel.ItemCategory);
                }

                if (!string.IsNullOrEmpty(searchModel.PropertyCategory))
                {
                    query = query.Where(x => x.Property.Category.Name == searchModel.PropertyCategory);
                }

                if (!string.IsNullOrEmpty(searchModel.CustomerId))
                {
                    query = query.Where(x => x.Applicants.Any(c => c.CustomerId == searchModel.CustomerId) ||
                       x.Property.PropertyOwnerships.Any(v => v.Ownerships.Any(b => b.CustomerId == searchModel.CustomerId)));
                }

                if (!string.IsNullOrEmpty(searchModel.District))
                {
                    query = query.Where(x => x.Property.District.Name == searchModel.District);
                }

                if (searchModel.Facilities?.Any(x => !string.IsNullOrEmpty(x.Name)) == true)
                {
                    query = searchModel.Facilities
                        .Where(x => !string.IsNullOrEmpty(x.Name))
                        .Select(facility => facility.Name)
                        .Aggregate(query, (current, name) => current.Where(x => x.Property.PropertyFacilities.Any(c => c.Facility.Name == name)));
                }

                if (searchModel.Features?.Any(x => !string.IsNullOrEmpty(x.Id)) == true)
                {
                    foreach (var feature in searchModel.Features.Where(x => !string.IsNullOrEmpty(x.Id)).ToList())
                    {
                        var id = feature.Id;
                        var from = feature.From;
                        var to = feature.To;

                        var featureInDb = await _features.Where(x => x.Id == feature.Id).Select(x => new
                        {
                            x.Name,
                            x.Type
                        }).FirstOrDefaultAsync();
                        if (featureInDb == null)
                            continue;

                        var isFromFilled = !string.IsNullOrEmpty(from);
                        var isToFilled = !string.IsNullOrEmpty(to);

                        if (!isFromFilled && !isToFilled)
                            continue;

                        if (isFromFilled && !isToFilled)
                        {
                            if (long.TryParse(from, out var numFrom))
                            {
                                if (featureInDb.Type == FeatureTypeEnum.Item)
                                {
                                    query = query.Where(x => x.ItemFeatures.Any(ftr => ftr.FeatureId == id &&
                                      CustomDbFunctions.IsNumeric(ftr.Value) == 1 &&
                                      Convert.ToInt64(ftr.Value) >= numFrom));
                                }
                                else
                                {
                                    query = query.Where(x => x.Property.PropertyFeatures.Any(ftr => ftr.FeatureId == id &&
                                      CustomDbFunctions.IsNumeric(ftr.Value) == 1 &&
                                      Convert.ToInt64(ftr.Value) >= numFrom));
                                }
                            }
                            else
                            {
                                if (featureInDb.Type == FeatureTypeEnum.Item)
                                {
                                    query = query.Where(x => x.ItemFeatures.Any(ftr => ftr.FeatureId == id &&
                                      EF.Functions.Like(ftr.Value, from.Like())));
                                }
                                else
                                {
                                    query = query.Where(x => x.Property.PropertyFeatures.Any(ftr => ftr.FeatureId == id &&
                                      EF.Functions.Like(ftr.Value, from.Like())));
                                }
                            }
                        }

                        if (!isFromFilled && isToFilled)
                        {
                            if (long.TryParse(to, out var numTo))
                            {
                                if (featureInDb.Type == FeatureTypeEnum.Item)
                                {
                                    query = query.Where(x => x.ItemFeatures.Any(ftr => ftr.FeatureId == id &&
                                      CustomDbFunctions.IsNumeric(ftr.Value) == 1 &&
                                      Convert.ToInt64(ftr.Value) <= numTo));
                                }
                                else
                                {
                                    query = query.Where(x => x.Property.PropertyFeatures.Any(ftr => ftr.FeatureId == id &&
                                      CustomDbFunctions.IsNumeric(ftr.Value) == 1 &&
                                      Convert.ToInt64(ftr.Value) <= numTo));
                                }
                            }
                        }

                        if (isFromFilled && isToFilled)
                        {
                            if (long.TryParse(from, out var numFrom) && long.TryParse(to, out var numTo))
                            {
                                if (featureInDb.Type == FeatureTypeEnum.Item)
                                {
                                    query = query.Where(x => x.ItemFeatures.Any(ftr => ftr.FeatureId == id &&
                                      CustomDbFunctions.IsNumeric(ftr.Value) == 1 &&
                                      Convert.ToInt64(ftr.Value) >= numFrom &&
                                      Convert.ToInt64(ftr.Value) <= numTo));
                                }
                                else
                                {
                                    query = query.Where(x => x.Property.PropertyFeatures.Any(ftr => ftr.FeatureId == id &&
                                      CustomDbFunctions.IsNumeric(ftr.Value) == 1 &&
                                      Convert.ToInt64(ftr.Value) >= numFrom &&
                                      Convert.ToInt64(ftr.Value) <= numTo));
                                }
                            }
                        }
                    }
                }
                if (searchModel.IsNegotiable)
                    query = query.Where(x => x.Description.Contains("قابل مذاکره"));

                if (searchModel.HasPicture)
                    query = query.Where(x => x.Property.Pictures.Any());

                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            query = query.Where(x => x.Category.UserItemCategories.Any(c => c.UserId == currentUser.Id));
            query = query.Where(x => x.Property.Category.UserPropertyCategories.Any(c => c.UserId == currentUser.Id));

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<ItemViewModel>(act =>
                {
                    act.IncludeAs<Item, Property, PropertyViewModel>(_unitOfWork, x => x.Property, ent =>
                    {
                        ent.IncludeAs<Property, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                        ent.IncludeAs<Property, District, DistrictViewModel>(_unitOfWork, x => x.District);
                        ent.IncludeAs<Property, Picture, PictureViewModel>(_unitOfWork, x => x.Pictures);
                        ent.IncludeAs<Property, PropertyFacility, PropertyFacilityViewModel>(_unitOfWork, x => x.PropertyFacilities,
                            ent2 => ent2.IncludeAs<PropertyFacility, Facility, FacilityViewModel>(_unitOfWork, x => x.Facility));
                        ent.IncludeAs<Property, PropertyFeature, PropertyFeatureViewModel>(_unitOfWork, x => x.PropertyFeatures,
                            ent2 => ent2.IncludeAs<PropertyFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                        ent.IncludeAs<Property, PropertyOwnership, PropertyOwnershipViewModel>(_unitOfWork, x => x.PropertyOwnerships,
                            ent2 => ent2.IncludeAs<PropertyOwnership, Ownership, OwnershipViewModel>(_unitOfWork, x => x.Ownerships,
                                ent3 => ent3.IncludeAs<Ownership, Customer, CustomerViewModel>(_unitOfWork, x => x.Customer)));
                    });
                    act.IncludeAs<Item, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                    act.IncludeAs<Item, Applicant, ApplicantViewModel>(_unitOfWork, x => x.Applicants,
                        ent => ent.IncludeAs<Applicant, Customer, CustomerViewModel>(_unitOfWork, x => x.Customer));
                    act.IncludeAs<Item, DealRequest, DealRequestViewModel>(_unitOfWork, x => x.DealRequests);
                    act.IncludeAs<Item, ItemFeature, ItemFeatureViewModel>(_unitOfWork, x => x.ItemFeatures,
                        ent => ent.IncludeAs<ItemFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                }), currentUser, 12);

            return result;
        }

        public async Task<PaginationViewModel<ItemViewModel>> RequestListAsync(DealRequestSearchViewModel searchModel)
        {
            var query = from item in _items
                        let requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
                        let lastRequest = requests.FirstOrDefault()
                        where requests.Any() && lastRequest.Status == DealStatusEnum.Requested
                        select item;

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<ItemViewModel>());
            return result;
        }

        public async Task<MethodStatus<Item>> RequestAsync(DealRequestInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Item>(StatusEnum.ModelIsNull, null);

            if (model?.Customers?.Any() != true)
                return new MethodStatus<Item>(StatusEnum.ApplicantsEmpty, null);

            var item = await _items.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (item == null)
                return new MethodStatus<Item>(StatusEnum.ItemIsNull, null);

            var (requestAddStatus, newState) = await RequestAddStateAsync(item.Id, DealStatusEnum.Requested);
            if (requestAddStatus != StatusEnum.Success)
                return new MethodStatus<Item>(StatusEnum.DealRequestIsNull, null);

            await SyncApplicantsAsync(item, model, false);
            return await _baseService.SaveChangesAsync(item, save);
        }

        public async Task<ItemComplexInputViewModel> ItemInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var query = _items.AsQueryable().AsNoTracking();
            query = query.Include(x => x.ItemFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.Category)
                .Include(x => x.Property.PropertyFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.Property.PropertyFacilities)
                .ThenInclude(x => x.Facility)
                .Include(x => x.Property.Category)
                .Include(x => x.Property.District)
                .Include(x => x.Property.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Customer);
            var entity = await query.FirstOrDefaultAsync(x => x.Id == id);
            var viewModel = entity?.Map<ItemViewModel>(ent =>
            {
                ent.IncludeAs<Item, ItemFeature, ItemFeatureViewModel>(_unitOfWork, x => x.ItemFeatures,
                    ent2 => ent2.IncludeAs<ItemFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                ent.IncludeAs<Item, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                ent.IncludeAs<Item, Property, PropertyViewModel>(_unitOfWork, x => x.Property, ent2 =>
                {
                    ent2.IncludeAs<Property, PropertyFeature, PropertyFeatureViewModel>(_unitOfWork, x => x.PropertyFeatures,
                        ent3 => ent3.IncludeAs<PropertyFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                    ent2.IncludeAs<Property, PropertyFacility, PropertyFacilityViewModel>(_unitOfWork, x => x.PropertyFacilities,
                        ent3 => ent3.IncludeAs<PropertyFacility, Facility, FacilityViewModel>(_unitOfWork, x => x.Facility));
                    ent2.IncludeAs<Property, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                    ent2.IncludeAs<Property, District, DistrictViewModel>(_unitOfWork, x => x.District);
                    ent2.IncludeAs<Property, PropertyOwnership, PropertyOwnershipViewModel>(_unitOfWork, x => x.PropertyOwnerships, ent3 =>
                       ent3.IncludeAs<PropertyOwnership, Ownership, OwnershipViewModel>(_unitOfWork, x => x.Ownerships, ent4 =>
                          ent4.IncludeAs<Ownership, Customer, CustomerViewModel>(_unitOfWork, x => x.Customer)));
                });
            });

            #region Property

            var ownership = viewModel?.Property.CurrentPropertyOwnership?.Ownership;
            if (ownership == null)
                return default;

            #endregion Property

            var result = new ItemComplexInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                CategoryId = viewModel.Category?.Id,
                ItemFeatures = viewModel.ItemFeatures?.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature?.Id,
                    Name = x.Feature?.Name,
                    Value = x.OriginalValue
                }).ToList(),
                Property = new PropertyComplexInputViewModel
                {
                    Id = viewModel.Property?.Id,
                    Description = viewModel.Property?.Description,
                    PropertyFacilities = viewModel.Property?.PropertyFacilities?.Select(x => new FacilityJsonViewModel
                    {
                        Id = x.Facility?.Id,
                        Name = x.Facility?.Name
                    }).ToList(),
                    CategoryId = viewModel.Property?.Category?.Id,
                    PropertyFeatures = viewModel.Property?.PropertyFeatures?.Select(x => new FeatureJsonValueViewModel
                    {
                        Id = x.Feature?.Id,
                        Name = x.Feature?.Name,
                        Value = x.OriginalValue
                    }).ToList(),
                    Number = viewModel.Property?.Number,
                    Street = viewModel.Property?.Street,
                    Flat = viewModel.Property?.Flat ?? 0,
                    DistrictId = viewModel.Property?.District?.Id,
                    Alley = viewModel.Property?.Alley,
                    BuildingName = viewModel.Property?.BuildingName,
                    Floor = viewModel.Property?.Floor ?? 0,
                    Ownership = new OwnershipInputViewModel
                    {
                        Id = ownership.Customer.Id,
                        Name = ownership.Customer.Name,
                        Address = ownership.Customer.Address,
                        Mobile = ownership.Customer.Mobile,
                        Phone = ownership.Customer.Phone,
                        Description = ownership.Description,
                        Dong = ownership.Dong
                    }
                },
                IsNegotiable = viewModel.IsNegotiable
            };
            return result;
        }

        public async Task<ItemViewModel> ItemAsync(string id, DealStatusEnum? specificStatus)
        {
            var query = _items.Select(item => new
            {
                item,
                requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
            })
                .Select(processed => new
                {
                    processedItem = processed,
                    lastRequest = processed.requests.FirstOrDefault()
                });

            if (specificStatus != null)
            {
                switch (specificStatus)
                {
                    case DealStatusEnum.Rejected:
                        query = query.Where(editedItem => !editedItem.processedItem.requests.Any() || editedItem.lastRequest.Status == DealStatusEnum.Rejected);
                        break;

                    case DealStatusEnum.Requested:
                    case DealStatusEnum.Finished:
                    default:
                        query = query.Where(editedItem => editedItem.processedItem.requests.Any() && editedItem.lastRequest.Status == specificStatus);
                        break;
                }
            }

            var que = query.Select(editedItem => editedItem.processedItem.item);
            var entity = await que.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return default;

            var viewModel = entity?.Map<ItemViewModel>();
            return viewModel;
        }

        public async Task<Item> ItemEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _items.FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public async Task<StatusEnum> ItemRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _items.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(entity,
                new[] {
                    Role.SuperAdmin, Role.Admin
                });

            return result;
        }

        public Task<MethodStatus<Item>> ItemAddOrUpdateAsync(ItemComplexInputViewModel model, bool update, bool save)
        {
            return update ?
                ItemUpdateAsync(model, save) :
                ItemAddAsync(model, save);
        }

        private async Task<MethodStatus<Item>> ItemUpdateAsync(ItemComplexInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Item>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Item>(StatusEnum.IdIsNull, null);

            if (model.Property == null)
                return new MethodStatus<Item>(StatusEnum.PropertyIsNull, null);

            var (propertyUpdateStatus, updatedProperty) = await _propertyService.PropertyComplexAddOrUpdateAsync(model.Property, true);
            if (propertyUpdateStatus != StatusEnum.Success && propertyUpdateStatus != StatusEnum.PropertyIsAlreadyExists)
                return new MethodStatus<Item>(propertyUpdateStatus, null);

            var entity = await _items
                .IgnoreQueryFilters()
                .Include(x => x.ItemFeatures)
                .ThenInclude(x => x.Feature)
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (model.IsNegotiable)
            {
                if (string.IsNullOrEmpty(model.Description))
                {
                    model.Description = "قابل مذاکره";
                }
                else
                {
                    if (!model.Description.Contains("قابل مذاکره", StringComparison.CurrentCultureIgnoreCase))
                    {
                        model.Description += " قابل مذاکره";
                    }
                }
            }
            else
            {
                model.Description = model.Description?.Replace("قابل مذاکره", "", StringComparison.CurrentCultureIgnoreCase);
            }

            var (updateStatus, updatedItem) = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.CategoryId = model.CategoryId;
                    entity.Description = model.Description;
                    entity.PropertyId = updatedProperty.Id;
                }, null, false, StatusEnum.PropertyIsNull);

            if (updatedItem == null)
                return new MethodStatus<Item>(StatusEnum.ItemIsNull, null);

            await ItemSyncAsync(updatedItem, model, false);
            return await _baseService.SaveChangesAsync(updatedItem, save);
        }

        private async Task<MethodStatus<Item>> ItemAddAsync(ItemComplexInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Item>(StatusEnum.ModelIsNull, null);

            if (model.Property == null)
                return new MethodStatus<Item>(StatusEnum.PropertyIsNull, null);

            var (propertyAddStatus, newProperty) = await _propertyService.PropertyComplexAddOrUpdateAsync(model.Property, true);
            if (propertyAddStatus != StatusEnum.Success)
                return new MethodStatus<Item>(propertyAddStatus, null);

            if (model.IsNegotiable)
            {
                if (string.IsNullOrEmpty(model.Description))
                {
                    model.Description = "قابل مذاکره";
                }
                else
                {
                    if (!model.Description.Contains("قابل مذاکره", StringComparison.CurrentCultureIgnoreCase))
                    {
                        model.Description += " قابل مذاکره";
                    }
                }
            }
            else
            {
                model.Description = model.Description?.Replace("قابل مذاکره", "", StringComparison.CurrentCultureIgnoreCase);
            }

            var (itemAddStatus, newItem) = await _baseService.AddAsync(new Item
            {
                CategoryId = model.CategoryId,
                Description = model.Description,
                PropertyId = newProperty.Id,
            }, null, false);
            if (itemAddStatus != StatusEnum.Success)
                return new MethodStatus<Item>(itemAddStatus, null);

            await ItemSyncAsync(newItem, model, false);
            return await _baseService.SaveChangesAsync(newItem, save);
        }

        private async Task<StatusEnum> ItemSyncAsync(Item newItem, ItemComplexInputViewModel model, bool save)
        {
            var syncFeatures = await _baseService.SyncAsync(
                newItem.ItemFeatures,
                model.ItemFeatures,
                (feature, currentUser) => new ItemFeature
                {
                    FeatureId = feature.Id,
                    Value = feature.Value,
                    ItemId = newItem.Id
                },
                inDb => new { inDb.FeatureId, inDb.Value },
                (inDb, inModel) => inDb.FeatureId == inModel.Id,
                (inDb, inModel) => inDb.Value == inModel.Value,
                (inDb, inModel) => inDb.Value = inModel.Value,
                null);
            return syncFeatures;
        }
    }
}