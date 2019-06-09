using LinqKit;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IItemService
    {
        Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel, bool cleanDuplicates = false);

        Task<StatusEnum> RequestRejectAsync(string itemId, bool save);

        Task<StatusEnum> ItemRemoveAsync(string id);

        Task<bool> ItemCheckAsync(PropertyCheckViewModel model);

        Task<ItemComplexInputViewModel> ItemComplexInputAsync(string id);

        Task<MethodStatus<Item>> ItemComplexAddOrUpdateAsync(ItemComplexInputViewModel model, bool update, bool save);

        Task<MethodStatus<Item>> ItemComplexAddAsync(ItemComplexInputViewModel model, bool save);

        Task<MethodStatus<Item>> ItemComplexUpdateAsync(ItemComplexInputViewModel model, bool save);

        Task<SyncJsonViewModel> ItemListAsync(string user, string pass, string itmCategory, string propCategory);

        Task<List<PropertyJsonViewModel>> ItemListAsync(string district, string category, string street);

        Task<PaginationViewModel<ItemViewModel>> RequestListAsync(DealRequestSearchViewModel searchModel);

        Task<MethodStatus<Item>> ItemAddOrUpdateAsync(ItemInputViewModel model, bool update, bool save);

        Task<List<ZoonkanViewModel>> ZoonkansAsync();

        Task<MethodStatus<Item>> RequestAsync(DealRequestInputViewModel model, bool save);

        Task<Item> ItemEntityAsync(string id);

        Task<ItemInputViewModel> ItemInputAsync(string id);

        Task<ItemViewModel> ItemAsync(string id, DealStatusEnum? specificStatus);

        Task<MethodStatus<Item>> ItemAddAsync(ItemInputViewModel model, bool save);
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        //        private readonly IMapper _mapper;
        private readonly IBaseService _baseService;

        private readonly ICustomerService _customerService;
        private readonly IFeatureService _featureService;
        private readonly IPropertyService _propertyService;
        private readonly DbSet<Deal> _itemRequests;
        private readonly DbSet<Item> _items;
        private readonly DbSet<Ownership> _ownerships;
        private readonly DbSet<Customer> _customers;
        private readonly DbSet<DealRequest> _dealRequests;
        private readonly DbSet<Applicant> _applicants;
        private readonly DbSet<Feature> _features;
        private readonly DbSet<User> _users;
        private readonly DbSet<Category> _categories;
        private readonly DbSet<Picture> _pictures;
        private readonly DbSet<Property> _properties;
        private readonly DbSet<ItemFeature> _itemFeatures;
        private readonly DbSet<PropertyFeature> _propertyFeatures;
        private readonly DbSet<PropertyFacility> _propertyFacilities;
        private readonly DbSet<Facility> _facilities;
        private readonly DbSet<PropertyOwnership> _propertyOwnerships;

        public ItemService(
            IBaseService baseService,
            IUnitOfWork unitOfWork,
            IFeatureService featureService,
            IPropertyService propertyService,
            ICustomerService customerService
            //            IMapper mapper
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
            _featureService = featureService;
            _customerService = customerService;
            _propertyService = propertyService;
            //            _mapper = mapper;
            _itemRequests = _unitOfWork.Set<Deal>();
            _propertyOwnerships = _unitOfWork.Set<PropertyOwnership>();
            _applicants = _unitOfWork.Set<Applicant>();
            _properties = _unitOfWork.Set<Property>();
            _items = _unitOfWork.Set<Item>();
            _ownerships = _unitOfWork.Set<Ownership>();
            _customers = _unitOfWork.Set<Customer>();
            _dealRequests = _unitOfWork.Set<DealRequest>();
            _features = _unitOfWork.Set<Feature>();
            _facilities = _unitOfWork.Set<Facility>();
            _itemFeatures = _unitOfWork.Set<ItemFeature>();
            _propertyFeatures = _unitOfWork.Set<PropertyFeature>();
            _users = _unitOfWork.Set<User>();
            _propertyFacilities = _unitOfWork.Set<PropertyFacility>();
            _categories = _unitOfWork.Set<Category>();
            _pictures = _unitOfWork.Set<Picture>();
        }

        public async Task<List<ZoonkanViewModel>> ZoonkansAsync()
        {
            var query = _items
                .AsNoTracking()
                .Include(x => x.Property)
                .ThenInclude(x => x.Pictures)
                .Include(x => x.Property)
                .ThenInclude(x => x.Category)
                .Include(x => x.Category)
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
                    Pictures = x.SelectMany(item => item.Property.Pictures)
                });

            var categories = await query.ToListAsync();
            if (categories?.Any() != true)
                return default;

            var result = categories.Select(x => new ZoonkanViewModel
            {
                ItemCategory = x.ItemCategory,
                PropertyCategory = x.PropertyCategory,
                Count = x.Count,
                Picture = x.Pictures.Select(c => c.File).SelectRandom()
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

        public async Task<SyncJsonViewModel> ItemListAsync(string user, string pass, string itmCategory, string propCategory)
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                return new SyncJsonViewModel
                {
                    StatusCode = (int)StatusEnum.CredentialError,
                    Message = StatusEnum.CredentialError.GetDisplayName()
                };
            }

            var encryptedPass = pass.Cipher(CryptologyExtension.CypherMode.Encryption);
            var userDb = await _users
                .WhereNotDeleted()
                .Where(x => x.Username.Equals(user, StringComparison.CurrentCultureIgnoreCase) && x.Password == encryptedPass)
                .FirstOrDefaultAsync()
                ;
            if (userDb == null)
            {
                return new SyncJsonViewModel
                {
                    StatusCode = (int)StatusEnum.UserNotFound,
                    Message = StatusEnum.UserNotFound.GetDisplayName()
                };
            }

            var allowedItemCategories = userDb.UserItemCategories.ToList().Select(x => x.Category.Name).ToList();
            var allowedPropertyCategories = userDb.UserPropertyCategories.ToList().Select(x => x.Category.Name).ToList();

            var query = from item in _items
                        let requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
                        let lastRequest = requests.FirstOrDefault()
                        where !requests.Any() || lastRequest.Status == DealStatusEnum.Rejected
                        let category = item.Category
                        let property = item.Property
                        let propertyCategory = property.Category
                        where category.UserItemCategories.Any(userItemCategory => userItemCategory.UserId == userDb.Id && userItemCategory.CategoryId == category.Id)
                        where propertyCategory.UserPropertyCategories.Any(userPropertyCategory =>
                            userPropertyCategory.UserId == userDb.Id && userPropertyCategory.CategoryId == propertyCategory.Id)
                        select item;

            if (!string.IsNullOrEmpty(itmCategory))
                query = query.Where(x => x.Category.Name == itmCategory);

            if (!string.IsNullOrEmpty(propCategory))
                query = query.Where(x => x.Property.Category.Name == propCategory);

            var items = await query.ToListAsync();
            if (items?.Any() != true)
            {
                return new SyncJsonViewModel
                {
                    StatusCode = (int)StatusEnum.Success,
                    Message = StatusEnum.Success.GetDisplayName(),
                    ItemCategories = allowedItemCategories,
                    PropertyCategories = allowedPropertyCategories
                };
            }

            var result = new List<ItemOutJsonViewModel>();
            foreach (var model in items)
            {
                var converted = model.Map<ItemViewModel>();
                if (converted == null)
                    continue;

                var item = new ItemOutJsonViewModel
                {
                    Property = new PropertyOutJsonViewModel
                    {
                        Address = converted.Property?.Address,
                        Category = converted.Property?.Category?.Name,
                        District = converted.Property?.District?.Name,
                        Description = converted.Description,
                        Facilities = converted.Property?.PropertyFacilities?.Select(x => x.Facility?.Name).ToList(),
                        Ownerships = converted.Property?.PropertyOwnerships?.SelectMany(x => x.Ownerships?.Select(c => c.Customer?.Name)).ToList(),
                        Features = converted.Property?.PropertyFeatures?.Select(x => new ValueTuple<string, string>(x.Feature?.Name, x.Value)).ToList(),
                    },
                    Category = converted.Category?.Name,
                    ItemFeatures = converted.ItemFeatures?.Select(x => new ValueTuple<string, string>(x.Feature?.Name, x.Value)).ToList()
                };
                result.Add(item);
            }

            return new SyncJsonViewModel
            {
                StatusCode = (int)StatusEnum.Success,
                Message = StatusEnum.Success.GetDisplayName(),
                Items = result,
                ItemCategories = allowedItemCategories,
                PropertyCategories = allowedPropertyCategories
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
                        where (EF.Functions.Like(item.Property.Street, street.Like())
                               || EF.Functions.Like(item.Property.Alley, street.Like()))
                              && item.Property.District.Name == district
                              && item.Property.Category.Name == category
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

            var query = from item in _items.WhereNotDeleted()
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
                        where (EF.Functions.Like(item.Property.Street, model.Street.Like())
                               || EF.Functions.Like(item.Property.Alley, model.Street.Like()))
                              && item.Property.District.Name == model.District
                              && item.Property.Category.Name == model.Category
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

        private async Task CleanDuplicatesAsync()
        {
            var groups = await _items.IgnoreQueryFilters()
                //                .AsNoTracking()
                .GroupBy(x => new
                {
                    x.CategoryId,
                    x.PropertyId
                }).Where(x => x.Count() > 1).ToListAsync();
            if (groups?.Any() != true)
                return;

            foreach (var groupedItem in groups)
            {
                foreach (var item in groupedItem.Skip(1))
                {
                    var itemInDb = await _items.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == item.Id);
                    if (itemInDb == null)
                        continue;

                    var itemFeatures = itemInDb.ItemFeatures;
                    if (itemFeatures?.Any() == true)
                    {
                        foreach (var itemFeature in itemFeatures)
                        {
                            var itemFeatureInDb = await _itemFeatures.FirstOrDefaultAsync(x => x.Id == itemFeature.Id);
                            await _baseService.RemoveAsync(itemFeatureInDb, null, DeleteEnum.Delete, false);
                        }
                    }

                    var propertyInDb = itemInDb.Property;
                    if (propertyInDb == null)
                        continue;

                    var propertyOwnerships = propertyInDb.PropertyOwnerships;
                    if (propertyOwnerships?.Any() == true)
                    {
                        foreach (var propertyOwnership in propertyOwnerships)
                        {
                            var ownerships = propertyOwnership.Ownerships;
                            if (ownerships?.Any() == true)
                                foreach (var ownership in ownerships)
                                {
                                    var ownershipInDb = await _ownerships.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == ownership.Id);
                                    await _baseService.RemoveAsync(ownershipInDb, null, DeleteEnum.Delete, false);
                                }

                            var propertyOwnershipInDb = await _propertyOwnerships.FirstOrDefaultAsync(x => x.Id == propertyOwnership.Id);
                            await _baseService.RemoveAsync(propertyOwnershipInDb, null, DeleteEnum.Delete, false);
                        }
                    }

                    var propertyFeatures = propertyInDb.PropertyFeatures;
                    if (propertyFeatures?.Any() == true)
                        foreach (var propertyFeature in propertyFeatures)
                        {
                            var propertyFeatureInDb = await _propertyFeatures.FirstOrDefaultAsync(x => x.Id == propertyFeature.Id);
                            await _baseService.RemoveAsync(propertyFeatureInDb, null, DeleteEnum.Delete, false);
                        }

                    var propertyFacilities = propertyInDb.PropertyFacilities;
                    if (propertyFacilities?.Any() == true)
                        foreach (var propertyFacility in propertyFacilities)
                        {
                            var propertyFacilityInDb = await _propertyFacilities.FirstOrDefaultAsync(x => x.Id == propertyFacility.Id);
                            await _baseService.RemoveAsync(propertyFacilityInDb, null, DeleteEnum.Delete, false);
                        }

                    var pictures = propertyInDb.Pictures;
                    if (pictures?.Any() == true)
                        foreach (var picture in pictures)
                        {
                            var pictureInDb = await _pictures.FirstOrDefaultAsync(x => x.Id == picture.Id);
                            await _baseService.RemoveAsync(pictureInDb, null, DeleteEnum.Delete, false);
                        }

                    await _baseService.RemoveAsync(itemInDb, null, DeleteEnum.Delete, false);
                    await _baseService.RemoveAsync(propertyInDb, null, DeleteEnum.Delete, false);
                }
            }

            await _baseService.SaveChangesAsync();
        }

        public async Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel, bool cleanDuplicates = false)
        {
            if (cleanDuplicates && _baseService.IsAllowed(Role.SuperAdmin))
                await CleanDuplicatesAsync();

            var query = _baseService.CheckDeletedItemsPrevillege(_items, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<ItemViewModel>();

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

            var cacheKey = new StringBuilder();

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
                    //                    query = query.Where(x =>
                    //                        x.Property.PropertyOwnerships.Any(c => c.Ownerships.Any(v => EF.Functions.Like(v.Customer.Name, searchModel.Owner.Like()))));

                    var customer = await _customers.Where(x => x.Name == searchModel.Owner).Select(x => x.Id).FirstOrDefaultAsync();
                    query = from item in query
                            join propertyOwnership in _propertyOwnerships on item.Property.Id equals propertyOwnership.PropertyId into propertyOwnerships
                            let lastPropOwnership = propertyOwnerships.OrderByDescending(x => x.Audits.FirstOrDefault(v => v.Type == LogTypeEnum.Create).DateTime)
                                .FirstOrDefault()
                            join ownership in _ownerships on lastPropOwnership.Id equals ownership.PropertyOwnershipId into ownerships
                            where ownerships.Any(x => x.CustomerId == customer)
                            select item;
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
                    query = query.Where(x => x.Applicants.Any(c => c.CustomerId == searchModel.CustomerId)
                                             || x.Property.PropertyOwnerships.Any(v => v.Ownerships.Any(b => b.CustomerId == searchModel.CustomerId)));
                }

                if (!string.IsNullOrEmpty(searchModel.District))
                {
                    query = query.Where(x => x.Property.District.Name == searchModel.District);
                }

                if (searchModel.Facilities?.Any(x => !string.IsNullOrEmpty(x.Name)) == true)
                {
                    var validFacilities = searchModel.Facilities.Where(x => !string.IsNullOrEmpty(x.Name)).Select(x => x.Name).ToList();
                    var predicateFacilities = PredicateBuilder.New<Facility>(true);
                    foreach (var facilityName in validFacilities)
                        predicateFacilities = predicateFacilities.Or(facility => facility.Name == facilityName);
                    var facilities = await _facilities.Where(predicateFacilities).Select(x => x.Id).ToListAsync();

                    var tempQuery = query
                        .GroupJoin(_propertyFacilities, item => item.PropertyId, propertyFacility => propertyFacility.PropertyId,
                            (item, propertyFacilities) =>
                                new
                                {
                                    Item = item,
                                    PropertyFacilities = propertyFacilities
                                });

                    foreach (var facility in facilities)
                        tempQuery = tempQuery.Where(x => x.PropertyFacilities.Any(c => c.FacilityId == facility));

                    query = tempQuery.Select(x => x.Item);
                }

                if (searchModel.Features?.Any(x => !string.IsNullOrEmpty(x.Id)) == true)
                {
                    foreach (var feature in searchModel.Features.Where(x => !string.IsNullOrEmpty(x.Id)))
                    {
                        var id = feature.Id;
                        var type = await _features.Where(x => x.Id == id).Select(x => x.Type).FirstOrDefaultAsync();
                        var from = feature.From;
                        var to = feature.To;

                        if (!string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to))
                        {
                            if (int.TryParse(from, out var numFrom))
                            {
                                if (type == FeatureTypeEnum.Item)
                                {
                                    query = from que in query
                                            join itemFeature in _itemFeatures on que.Id equals itemFeature.ItemId into itemFeatures
                                            where itemFeatures.Any(x => x.Feature.Id == id && x.Value.IsNumeric() >= numFrom)
                                            select que;
                                }
                                else if (type == FeatureTypeEnum.Property)
                                {
                                    query = from que in query
                                            join propertyFeature in _propertyFeatures on que.PropertyId equals propertyFeature.PropertyId into propertyFeatures
                                            where propertyFeatures.Any(x => x.Feature.Id == id && x.Value.IsNumeric() >= numFrom)
                                            select que;
                                }
                            }
                            else
                            {
                                if (type == FeatureTypeEnum.Item)
                                {
                                    query = from que in query
                                            join itemFeature in _itemFeatures on que.Id equals itemFeature.ItemId into itemFeatures
                                            where itemFeatures.Any(x => x.Feature.Id == id && x.Value == @from)
                                            select que;
                                }
                                else if (type == FeatureTypeEnum.Property)
                                {
                                    query = from que in query
                                            join propertyFeature in _propertyFeatures on que.PropertyId equals propertyFeature.PropertyId into propertyFeatures
                                            where propertyFeatures.Any(x => x.Feature.Id == id && x.Value == @from)
                                            select que;
                                }
                            }
                        }
                        else if (string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                        {
                            if (!int.TryParse(to, out var numTo))
                                continue;

                            if (type == FeatureTypeEnum.Item)
                            {
                                query = from que in query
                                        join itemFeature in _itemFeatures on que.Id equals itemFeature.ItemId into itemFeatures
                                        where itemFeatures.Any(x => x.Feature.Id == id && x.Value.IsNumeric() <= numTo)
                                        select que;
                            }
                            else if (type == FeatureTypeEnum.Property)
                            {
                                query = from que in query
                                        join propertyFeature in _propertyFeatures on que.PropertyId equals propertyFeature.PropertyId into propertyFeatures
                                        where propertyFeatures.Any(x => x.Feature.Id == id && x.Value.IsNumeric() <= numTo)
                                        select que;
                            }
                        }
                        else if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                        {
                            if (!int.TryParse(@from, out var numFrom) || !int.TryParse(to, out var numTo) || numFrom >= numTo)
                                continue;

                            if (type == FeatureTypeEnum.Item)
                            {
                                query = from que in query
                                        join itemFeature in _itemFeatures on que.Id equals itemFeature.ItemId into itemFeatures
                                        where itemFeatures.Any(x => x.Feature.Id == id && x.Value.IsNumeric() <= numTo && x.Value.IsNumeric() >= numFrom)
                                        select que;
                            }
                            else if (type == FeatureTypeEnum.Property)
                            {
                                query = from que in query
                                        join propertyFeature in _propertyFeatures on que.PropertyId equals propertyFeature.PropertyId into propertyFeatures
                                        where propertyFeatures.Any(x => x.Feature.Id == id && x.Value.IsNumeric() <= numTo && x.Value.IsNumeric() >= numFrom)
                                        select que;
                            }
                        }
                    }
                }
                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            var userItemCategories = currentUser.UserItemCategories.Select(x => x.CategoryId).ToList();
            var userPropertyCategories = currentUser.UserPropertyCategories.Select(x => x.CategoryId).ToList();
            query = query.Where(x => userItemCategories.Any(c => c == x.CategoryId));
            query = query.Where(x => userPropertyCategories.Any(c => c == x.Property.CategoryId));

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<ItemViewModel>(act =>
                {
                    act.IncludeAs<Property, PropertyViewModel>(item.Property, ent =>
                    {
                        ent.IncludeAs<Category, CategoryViewModel>(ent.Entity?.Category);
                        ent.IncludeAs<District, DistrictViewModel>(ent.Entity?.District);
                        ent.IncludeAs<Picture, PictureViewModel>(ent.Entity?.Pictures);
                        ent.IncludeAs<PropertyFacility, PropertyFacilityViewModel>(ent.Entity?.PropertyFacilities,
                            ent2 => ent2.IncludeAs<Facility, FacilityViewModel>(ent2.Entity?.Facility));
                        ent.IncludeAs<PropertyFeature, PropertyFeatureViewModel>(ent.Entity?.PropertyFeatures,
                            ent2 => ent2.IncludeAs<Feature, FeatureViewModel>(ent2.Entity?.Feature));
                        ent.IncludeAs<PropertyOwnership, PropertyOwnershipViewModel>(ent.Entity?.PropertyOwnerships,
                            ent2 => ent2.IncludeAs<Ownership, OwnershipViewModel>(ent2.Entity?.Ownerships,
                                ent3 => ent3.IncludeAs<Customer, CustomerViewModel>(ent3.Entity?.Customer)));
                    });
                    act.IncludeAs<Category, CategoryViewModel>(item.Category);
                    act.IncludeAs<Applicant, ApplicantViewModel>(item.Applicants, ent => ent.IncludeAs<Customer, CustomerViewModel>(ent.Entity?.Customer));
                    act.IncludeAs<DealRequest, DealRequestViewModel>(item.DealRequests);
                    act.IncludeAs<ItemFeature, ItemFeatureViewModel>(item.ItemFeatures, ent => ent.IncludeAs<Feature, FeatureViewModel>(ent.Entity?.Feature));
                }), CheckDuplicatesAsync(), currentUser);
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
                item => item.Map<ItemViewModel>(), Task.FromResult(false));
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

        public async Task<ItemComplexInputViewModel> ItemComplexInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await _items.FirstOrDefaultAsync(x => x.Id == id);
            var viewModel = entity?.Map<ItemViewModel>(ent =>
            {
                ent.IncludeAs<ItemFeature, ItemFeatureViewModel>(entity.ItemFeatures, ent2 => ent2.IncludeAs<Feature, FeatureViewModel>(ent2.Entity.Feature));
                ent.IncludeAs<Category, CategoryViewModel>(entity.Category);
            });

            if (viewModel == null)
                return default;

            var propertyInput = await _propertyService.PropertyComplexInputAsync(entity.PropertyId);
            if (propertyInput == null)
                return default;

            var result = new ItemComplexInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                CategoryId = viewModel.Category?.Id,
                ItemFeatures = viewModel.ItemFeatures?.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature?.Id,
                    Name = x.Feature?.Name,
                    Value = x.Value
                }).ToList(),
                Property = propertyInput
            };
            return result;
        }

        public async Task<ItemInputViewModel> ItemInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await _items.FirstOrDefaultAsync(x => x.Id == id);
            var viewModel = entity?.Map<ItemViewModel>();

            if (viewModel == null)
                return default;

            var result = new ItemInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                CategoryId = viewModel.Category?.Id,
                ItemFeatures = viewModel.ItemFeatures?.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature?.Id,
                    Name = x.Feature?.Name,
                    Value = x.Value
                }).ToList(),
                PropertyId = viewModel.Property?.Id
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
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public Task<MethodStatus<Item>> ItemAddOrUpdateAsync(ItemInputViewModel model, bool update, bool save)
        {
            return update
                ? ItemUpdateAsync(model, save)
                : ItemAddAsync(model, save);
        }

        public Task<MethodStatus<Item>> ItemComplexAddOrUpdateAsync(ItemComplexInputViewModel model, bool update, bool save)
        {
            return update
                ? ItemComplexUpdateAsync(model, save)
                : ItemComplexAddAsync(model, save);
        }

        public async Task<MethodStatus<Item>> ItemComplexUpdateAsync(ItemComplexInputViewModel model, bool save)
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

            var entity = await ItemEntityAsync(model.Id);
            var (updateStatus, updatedItem) = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.CategoryId = model.CategoryId;
                    entity.Description = model.Description;
                    entity.PropertyId = updatedProperty.Id;
                }, null, false, StatusEnum.PropertyIsNull);

            if (updatedItem == null)
                return new MethodStatus<Item>(StatusEnum.ItemIsNull, null);

            await ItemComplexSyncAsync(updatedItem, model, false);
            return await _baseService.SaveChangesAsync(updatedItem, save);
        }

        private async Task<MethodStatus<Item>> ItemUpdateAsync(ItemInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Item>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Item>(StatusEnum.IdIsNull, null);

            var entity = await ItemEntityAsync(model.Id);
            var (updateStatus, updatedItem) = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.CategoryId = model.CategoryId;
                    entity.Description = model.Description;
                    entity.PropertyId = model.PropertyId;
                }, null, false, StatusEnum.PropertyIsNull);

            if (updatedItem == null)
                return new MethodStatus<Item>(StatusEnum.ItemIsNull, null);

            await ItemSyncAsync(updatedItem, model, false);
            return await _baseService.SaveChangesAsync(updatedItem, save);
        }

        public async Task<MethodStatus<Item>> ItemAddAsync(ItemInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Item>(StatusEnum.ModelIsNull, null);

            var (itemAddStatus, newItem) = await _baseService.AddAsync(new Item
            {
                CategoryId = model.CategoryId,
                Description = model.Description,
                PropertyId = model.PropertyId,
            }, null, false);

            await ItemSyncAsync(newItem, model, false);
            return await _baseService.SaveChangesAsync(newItem, save);
        }

        public async Task<MethodStatus<Item>> ItemComplexAddAsync(ItemComplexInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Item>(StatusEnum.ModelIsNull, null);

            if (model.Property == null)
                return new MethodStatus<Item>(StatusEnum.PropertyIsNull, null);

            var (propertyAddStatus, newProperty) = await _propertyService.PropertyComplexAddOrUpdateAsync(model.Property, true);
            if (propertyAddStatus != StatusEnum.Success)
                return new MethodStatus<Item>(propertyAddStatus, null);

            var (itemAddStatus, newItem) = await _baseService.AddAsync(new Item
            {
                CategoryId = model.CategoryId,
                Description = model.Description,
                PropertyId = newProperty.Id,
            }, null, false);
            if (itemAddStatus != StatusEnum.Success)
                return new MethodStatus<Item>(itemAddStatus, null);

            await ItemComplexSyncAsync(newItem, model, false);
            return await _baseService.SaveChangesAsync(newItem, save);
        }

        private async Task<StatusEnum> ItemComplexSyncAsync(Item newItem, ItemComplexInputViewModel model, bool save)
        {
            var syncFeatures = await _baseService.SyncAsync(
                newItem.ItemFeatures,
                model.ItemFeatures,
                (feature, currentUser) => new ItemFeature
                {
                    FeatureId = feature.Id,
                    Value = feature.Value,
                    ItemId = newItem.Id
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                null,
                save);
            return syncFeatures;
        }

        private async Task<StatusEnum> ItemSyncAsync(Item newItem, ItemInputViewModel model, bool save)
        {
            var syncFeatures = await _baseService.SyncAsync(
                newItem.ItemFeatures,
                model.ItemFeatures,
                (feature, currentUser) => new ItemFeature
                {
                    FeatureId = feature.Id,
                    Value = feature.Value,
                    ItemId = newItem.Id
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                null,
                save);
            return syncFeatures;
        }
    }
}