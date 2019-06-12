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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IPropertyService
    {
        Task<MethodStatus<Property>> PropertyAddAsync(PropertyInputViewModel model, bool save);

        Task<MethodStatus<Property>> PropertyComplexAddOrUpdateAsync(PropertyComplexInputViewModel model, bool save);

        Task<StatusEnum> PropertyRemoveAsync(string id);

        Task<List<PropertyJsonViewModel>> PropertyListAsync(string searchTerm);

        Task CleanDuplicatesAsync();

        Task<MethodStatus<Property>> PropertyComplexAddAsync(PropertyComplexInputViewModel model, bool save);

        Task<PropertyComplexInputViewModel> PropertyComplexInputAsync(string id);

        PropertyJsonViewModel MapJson(Property property);

        Task<MethodStatus<Property>> PropertyAddOrUpdateAsync(PropertyInputViewModel model, bool save);

        Task<PropertyJsonViewModel> PropertyJsonAsync(string id);

        Task<bool> PropertyValidate(string id);

        Task<PropertyInputViewModel> PropertyInputAsync(string id);

        Task<Property> PropertyEntityAsync(string id);

        Task<PaginationViewModel<PropertyViewModel>> PropertyListAsync(PropertySearchViewModel searchModel);

        Task<MethodStatus<PropertyOwnership>> PropertyOwnershipAddAsync(string propertyId, bool save);
    }

    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly ICustomerService _customerService;
        private readonly IFeatureService _featureService;
        private readonly ILocationService _locationService;
        private readonly DbSet<Property> _properties;
        private readonly DbSet<PropertyFeature> _propertyFeatures;
        private readonly DbSet<PropertyFacility> _propertyFacilities;
        private readonly DbSet<Item> _items;
        private readonly DbSet<ItemFeature> _itemFeatures;
        private readonly DbSet<Picture> _pictures;
        private readonly DbSet<Ownership> _ownerships;
        private readonly DbSet<PropertyOwnership> _propertyOwnerships;

        public PropertyService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IFeatureService featureService,
            ILocationService locationService,
            ICustomerService customerService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _locationService = locationService;
            _featureService = featureService;
            _customerService = customerService;
            _properties = _unitOfWork.Set<Property>();
            _items = _unitOfWork.Set<Item>();
            _propertyFacilities = _unitOfWork.Set<PropertyFacility>();
            _propertyFeatures = _unitOfWork.Set<PropertyFeature>();
            _pictures = _unitOfWork.Set<Picture>();
            _ownerships = _unitOfWork.Set<Ownership>();
            _itemFeatures = _unitOfWork.Set<ItemFeature>();
            _propertyOwnerships = _unitOfWork.Set<PropertyOwnership>();
        }

        public async Task<StatusEnum> PropertyRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _properties.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                .ConfigureAwait(false);

            return result;
        }

        //public IIncludableQueryable<Property, TEntity> IncludeRequirements<TEntity>(IIncludableQueryable<Property, TEntity> exp)
        //{
        //    var inc = exp.Include(x => x.PropertyOwnerships)
        //        .ThenInclude(x => x.Ownerships)
        //        .ThenInclude(x => x.Contact);
        //    return inc;
        //}

        public async Task<PropertyJsonViewModel> PropertyJsonAsync(string id)
        {
            var query = _properties.AsQueryable();
            var entity = await query.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (entity == null)
                return default;

            var result = MapJson(entity);
            return result;
        }

        public async Task CleanDuplicatesAsync()
        {
            var groups = await _properties.IgnoreQueryFilters()
               .OrderDescendingByCreationDateTime()
               .GroupBy(x => new
               {
                   x.Street,
                   x.CategoryId,
                   x.DistrictId,
                   x.Flat,
                   x.Alley,
                   x.Floor,
                   x.Number,
                   x.BuildingName
               }).Where(x => x.Count() > 1).ToListAsync();
            if (groups?.Any() == true)
            {
                foreach (var groupedProperty in groups)
                {
                    var cnt = groupedProperty.Count();
                    var propertyBest = groupedProperty.Any(x => x.PropertyFacilities.Any() || x.PropertyFeatures.Any())
                        ? groupedProperty.FirstOrDefault(x => x.PropertyFacilities.Any() || x.PropertyFeatures.Any())
                        : groupedProperty.FirstOrDefault();
                    if (propertyBest == null)
                        continue;

                    var validProperties = groupedProperty.Except(new[]
                    {
                    propertyBest
                }).ToList();
                    foreach (var property in validProperties)
                    {
                        var currentPropertyId = property.Id;

                        var propertyInDb = await _properties.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == currentPropertyId);
                        if (propertyInDb == null)
                            continue;

                        var items = await _items.IgnoreQueryFilters().Where(x => x.PropertyId == currentPropertyId).ToListAsync();
                        if (items?.Any() == true)
                        {
                            foreach (var item in items)
                            {
                                var itemInDb = await _items.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == item.Id);
                                if (itemInDb == null)
                                    continue;

                                var itemFeatures = await _itemFeatures.IgnoreQueryFilters().Where(x => x.ItemId == itemInDb.Id).ToListAsync();
                                if (itemFeatures?.Any() == true)
                                {
                                    foreach (var itemFeature in itemFeatures)
                                    {
                                        var itemFeatureInDb = await _itemFeatures.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == itemFeature.Id);
                                        await _baseService.RemoveAsync(itemFeatureInDb, null, DeleteEnum.Delete, false);
                                    }
                                }

                                await _baseService.RemoveAsync(itemInDb, null, DeleteEnum.Delete, false);
                            }
                        }

                        var propertyOwnerships = await _propertyOwnerships.IgnoreQueryFilters().Where(x => x.PropertyId == propertyInDb.Id).ToListAsync();
                        if (propertyOwnerships?.Any() == true)
                        {
                            foreach (var propertyOwnership in propertyOwnerships)
                            {
                                var ownerships = await _ownerships.IgnoreQueryFilters().Where(x => x.PropertyOwnershipId == propertyOwnership.Id).ToListAsync();
                                if (ownerships?.Any() == true)
                                    foreach (var ownership in ownerships)
                                    {
                                        var ownershipInDb = await _ownerships.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == ownership.Id);
                                        await _baseService.RemoveAsync(ownershipInDb, null, DeleteEnum.Delete, false);
                                    }

                                var propertyOwnershipInDb = await _propertyOwnerships.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == propertyOwnership.Id);
                                await _baseService.RemoveAsync(propertyOwnershipInDb, null, DeleteEnum.Delete, false);
                            }
                        }

                        var propertyFeatures = await _propertyFeatures.IgnoreQueryFilters().Where(x => x.PropertyId == propertyInDb.Id).ToListAsync();
                        if (propertyFeatures?.Any() == true)
                            foreach (var propertyFeature in propertyFeatures)
                            {
                                var propertyFeatureInDb = await _propertyFeatures.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == propertyFeature.Id);
                                await _baseService.RemoveAsync(propertyFeatureInDb, null, DeleteEnum.Delete, false);
                            }

                        var propertyFacilities = await _propertyFacilities.IgnoreQueryFilters().Where(x => x.PropertyId == propertyInDb.Id).ToListAsync();
                        if (propertyFacilities?.Any() == true)
                            foreach (var propertyFacility in propertyFacilities)
                            {
                                var propertyFacilityInDb = await _propertyFacilities.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == propertyFacility.Id);
                                await _baseService.RemoveAsync(propertyFacilityInDb, null, DeleteEnum.Delete, false);
                            }

                        var pictures = await _pictures.IgnoreQueryFilters().Where(x => x.PropertyId == propertyInDb.Id).ToListAsync();
                        if (pictures?.Any() == true)
                            foreach (var picture in pictures)
                            {
                                var pictureInDb = await _pictures.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == picture.Id);
                                await _baseService.RemoveAsync(pictureInDb, null, DeleteEnum.Delete, false);
                            }

                        await _baseService.RemoveAsync(propertyInDb, null, DeleteEnum.Delete, false);
                    }
                }
            }

            await _baseService.SaveChangesAsync();
        }

        public async Task<bool> PropertyValidate(string id)
        {
            var property = await _properties.AnyAsync(x => x.Id == id).ConfigureAwait(false);
            return property;
        }

        public async Task<List<PropertyJsonViewModel>> PropertyListAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return default;

            var query = _properties as IQueryable<Property>;
            query = query.Where(x => EF.Functions.Like(x.Id, searchTerm.Like())
                                     || EF.Functions.Like(x.Street, searchTerm.Like())
                                     || EF.Functions.Like(x.Alley, searchTerm.Like())
                                     || EF.Functions.Like(x.BuildingName, searchTerm.Like())
                                     || EF.Functions.Like(x.Category.Name, searchTerm.Like())
                                     || EF.Functions.Like(x.District.Name, searchTerm.Like()));

            var models = await query.ToListAsync().ConfigureAwait(false);
            if (models?.Any() != true)
                return default;

            var result = new List<PropertyJsonViewModel>();
            foreach (var property in models)
            {
                var mapped = MapJson(property);
                if (mapped == null)
                    continue;

                result.Add(mapped);
            }

            return result;
        }

        public async Task<PaginationViewModel<PropertyViewModel>> PropertyListAsync(PropertySearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_properties, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<PropertyViewModel>();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Id))
                    query = query.Where(x => x.Id == searchModel.Id);

                if (!string.IsNullOrEmpty(searchModel.Category))
                    query = query.Where(x => x.Category.Name == searchModel.Category);

                if (!string.IsNullOrEmpty(searchModel.District))
                    query = query.Where(x => x.District.Name == searchModel.District);

                if (!string.IsNullOrEmpty(searchModel.Street))
                    query = query.Where(x => EF.Functions.Like(x.Street, searchModel.Street.Like()));

                if (!string.IsNullOrEmpty(searchModel.Owner))
                    query = query.Where(x =>
                        x.PropertyOwnerships.Any(c => c.Ownerships.Any(v => EF.Functions.Like(v.Customer.Name, searchModel.Owner.Like()))));

                if (!string.IsNullOrEmpty(searchModel.OwnerMobile))
                    query = query.Where(x =>
                        x.PropertyOwnerships.Any(c => c.Ownerships.Any(v => EF.Functions.Like(v.Customer.MobileNumber, searchModel.OwnerMobile.Like()))));

                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<PropertyViewModel>(), Task.FromResult(false));

            return result;
        }

        public PropertyJsonViewModel MapJson(Property property)
        {
            var lastPropOwnership = property.PropertyOwnerships.OrderDescendingByCreationDateTime().FirstOrDefault();
            if (lastPropOwnership == null)
                return default;

            var prop = new PropertyJsonViewModel
            {
                Id = property.Id,
                District = property.District.Name,
                Description = property.Description,
                Address = property.Address,
                Category = property.Category.Name,
                Features = property.PropertyFeatures.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.FeatureId,
                    Name = x.Feature.Name,
                    Value = x.Value
                }).ToList(),
                Facilities = property.PropertyFacilities.Select(x => new FacilityJsonViewModel
                {
                    Id = x.FacilityId,
                    Name = x.Facility.Name,
                }).ToList(),
                Ownerships = lastPropOwnership.Ownerships.Select(x => new OwnershipJsonViewModel
                {
                    CustomerId = x.Customer?.Id,
                    Name = x.Customer?.Name,
                    Mobile = x.Customer?.MobileNumber,
                    Dong = x.Dong
                }).ToList()
            };
            return prop;
        }

        public async Task<Property> PropertyEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _properties.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<PropertyInputViewModel> PropertyInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await _properties.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var viewModel = entity?.Map<PropertyViewModel>();
            if (viewModel == null)
                return default;

            var result = new PropertyInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                Ownerships = viewModel.CurrentPropertyOwnership?.Ownerships?.Select(x => new OwnershipJsonViewModel
                {
                    CustomerId = x.Customer?.Id,
                    Name = x.Customer?.Name,
                    Mobile = x.Customer?.Mobile,
                    Dong = x.Dong
                }).ToList(),
                PropertyFacilities = viewModel.PropertyFacilities?.Select(x => new FacilityJsonViewModel
                {
                    Id = x.Facility?.Id,
                    Name = x.Facility?.Name
                }).ToList(),
                CategoryId = viewModel.Category?.Id,
                //                Latitude = viewModel.Geolocation?.Latitude ?? 0,
                //                Longitude = viewModel.Geolocation?.Longitude ?? 0,
                PropertyFeatures = viewModel.PropertyFeatures?.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature?.Id,
                    Name = x.Feature?.Name,
                    Value = x.Value
                }).ToList(),
                Number = viewModel.Number,
                Street = viewModel.Street,
                Flat = viewModel.Flat,
                DistrictId = viewModel.District?.Id,
                Alley = viewModel.Alley,
                BuildingName = viewModel.BuildingName,
                Floor = viewModel.Floor
            };
            return result;
        }

        public async Task<PropertyComplexInputViewModel> PropertyComplexInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await _properties.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var viewModel = entity?.Map<PropertyViewModel>(ent =>
            {
                ent.IncludeAs<PropertyFeature, PropertyFeatureViewModel>(entity.PropertyFeatures, ent2 => ent2.IncludeAs<Feature, FeatureViewModel>(ent2.Entity.Feature));
                ent.IncludeAs<PropertyFacility, PropertyFacilityViewModel>(entity.PropertyFacilities,
                    ent2 => ent2.IncludeAs<Facility, FacilityViewModel>(ent2.Entity.Facility));
                ent.IncludeAs<Category, CategoryViewModel>(entity.Category);
                ent.IncludeAs<District, DistrictViewModel>(entity.District);
            });
            if (viewModel == null)
                return default;

            var ownershipInput = await _customerService.OwnershipInputAsync(entity.CurrentOwnership.Ownerships.First().CustomerId).ConfigureAwait(false);
            if (ownershipInput == null)
                return default;

            var result = new PropertyComplexInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                PropertyFacilities = viewModel.PropertyFacilities?.Select(x => new FacilityJsonViewModel
                {
                    Id = x.Facility?.Id,
                    Name = x.Facility?.Name
                }).ToList(),
                CategoryId = viewModel.Category?.Id,
                //                Latitude = viewModel.Geolocation?.Latitude ?? 0,
                //                Longitude = viewModel.Geolocation?.Longitude ?? 0,
                PropertyFeatures = viewModel.PropertyFeatures?.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature?.Id,
                    Name = x.Feature?.Name,
                    Value = x.Value
                }).ToList(),
                Number = viewModel.Number,
                Street = viewModel.Street,
                Flat = viewModel.Flat,
                DistrictId = viewModel.District?.Id,
                Alley = viewModel.Alley,
                BuildingName = viewModel.BuildingName,
                Floor = viewModel.Floor,
                Ownership = ownershipInput
            };
            return result;
        }

        private async Task<MethodStatus<Property>> HasDuplicateAsync(PropertyComplexInputViewModel model)
        {
            var property = await _properties.FirstOrDefaultAsync(x =>
                x.Street == model.Street
                && x.CategoryId == model.CategoryId
                && x.DistrictId == model.DistrictId
                && x.Flat == model.Flat
                && x.Alley == model.Alley
                && x.Floor == model.Floor
                && x.Number == model.Number
                && x.BuildingName == model.BuildingName).ConfigureAwait(false);
            if (property == null)
                return new MethodStatus<Property>(StatusEnum.PropertyIsNull, null);

            var sameOwnershipAsModel = property.CurrentOwnership?.Ownerships?.Any(x => x.Customer.MobileNumber == model.Ownership.Mobile);
            if (sameOwnershipAsModel == null)
                throw new NullReferenceException(nameof(sameOwnershipAsModel) + " must be filled.");

            if (sameOwnershipAsModel == true)
                return new MethodStatus<Property>(StatusEnum.Success, property);

            return new MethodStatus<Property>(StatusEnum.PropertyIsAlreadyExistsWithDifferentOwner, property);
        }

        private async Task<bool> HasDuplicateAsync(PropertyInputViewModel model)
        {
            var duplicate = await _properties.FirstOrDefaultAsync(x =>
                x.Street == model.Street
                && x.CategoryId == model.CategoryId
                && x.DistrictId == model.DistrictId
                && x.Flat == model.Flat
                && x.Alley == model.Alley
                && x.Floor == model.Floor
                && x.Number == model.Number
                && x.BuildingName == model.BuildingName).ConfigureAwait(false);

            return duplicate != null;
        }

        public async Task<MethodStatus<Property>> PropertyComplexAddAsync(PropertyComplexInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Property>(StatusEnum.ModelIsNull, null);

            var (hasDuplicate, similarProperty) = await HasDuplicateAsync(model).ConfigureAwait(false);
            if (hasDuplicate == StatusEnum.Success)
                return new MethodStatus<Property>(hasDuplicate, similarProperty);

            var (customerAddStatus, newOwner) = await _customerService.OwnershipAddOrUpdateAsync(new OwnershipInputViewModel
            {
                Name = model.Ownership.Name,
                Address = model.Ownership.Address,
                Dong = 6,
                Mobile = model.Ownership.Mobile,
                Phone = model.Ownership.Phone
            }, !model.Ownership.IsNew, true).ConfigureAwait(false);
            if (customerAddStatus != StatusEnum.Success)
                return new MethodStatus<Property>(customerAddStatus, null);

            var (propertyAddStatus, newProperty) = await _baseService.AddAsync(new Property
            {
                Description = model.Description,
                DistrictId = model.DistrictId,
                CategoryId = model.CategoryId,
                Alley = model.Alley,
                BuildingName = model.BuildingName,
                Flat = model.Flat,
                Floor = model.Floor,
                Number = model.Number,
                Street = model.Street,
                //                Geolocation = model.Latitude > 0 && model.Longitude > 0 ? new Point(model.Longitude, model.Latitude) : default,
            }, null, false).ConfigureAwait(false);

            if (propertyAddStatus != StatusEnum.Success)
                return new MethodStatus<Property>(StatusEnum.PropertyIsNull, null);

            var (propertyOwnershipAddStatus, newPropertyOwnership) = await PropertyOwnershipAddAsync(newProperty.Id, false).ConfigureAwait(false);
            if (propertyOwnershipAddStatus != StatusEnum.Success)
                return new MethodStatus<Property>(StatusEnum.PropertyOwnershipIsNull, null);

            await PropertyComplexSyncAsync(newProperty, model, newPropertyOwnership, newOwner.Customer, false).ConfigureAwait(false);

            return await _baseService.SaveChangesAsync(newProperty, save).ConfigureAwait(false);
        }

        public async Task<MethodStatus<Property>> PropertyAddAsync(PropertyInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Property>(StatusEnum.ModelIsNull, null);

            if (model.Ownerships?.Any() != true)
                return new MethodStatus<Property>(StatusEnum.OwnershipIsNull, null);

            var duplicate = await HasDuplicateAsync(model).ConfigureAwait(false);
            if (duplicate)
                return new MethodStatus<Property>(StatusEnum.PropertyIsAlreadyExists, null);

            var (propertyAddStatus, newProperty) = await _baseService.AddAsync(new Property
            {
                Description = model.Description,
                DistrictId = model.DistrictId,
                CategoryId = model.CategoryId,
                Alley = model.Alley,
                BuildingName = model.BuildingName,
                Flat = model.Flat,
                Floor = model.Floor,
                Number = model.Number,
                Street = model.Street,
                //                Geolocation = model.Latitude > 0 && model.Longitude > 0 ? new Point(model.Longitude, model.Latitude) : default,
            }, null, false).ConfigureAwait(false);

            if (propertyAddStatus != StatusEnum.Success)
                return new MethodStatus<Property>(StatusEnum.PropertyIsNull, null);

            var (propertyOwnershipAddStatus, newPropertyOwnership) = await PropertyOwnershipAddAsync(newProperty.Id, false).ConfigureAwait(false);
            if (propertyOwnershipAddStatus != StatusEnum.Success)
                return new MethodStatus<Property>(StatusEnum.PropertyOwnershipIsNull, null);

            await PropertySyncAsync(newProperty, newPropertyOwnership, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newProperty, save).ConfigureAwait(false);
        }

        private async Task<StatusEnum> PropertyComplexSyncAsync(Property property, PropertyComplexInputViewModel model, PropertyOwnership newPropertyOwnership, Customer newCustomer, bool save)
        {
            var syncOwnership = new List<OwnershipJsonViewModel>
            {
                new OwnershipJsonViewModel
                {
                    Dong = 6,
                    Mobile = newCustomer.MobileNumber,
                    Name = newCustomer.Name,
                    CustomerId = newCustomer.Id
                }
            };

            await _baseService.SyncAsync(
                newPropertyOwnership.Ownerships.ToList(),
                syncOwnership,
                ownership => new Ownership
                {
                    CustomerId = ownership.CustomerId,
                    Dong = ownership.Dong,
                    PropertyOwnershipId = newPropertyOwnership.Id,
                },
                (inDb, inModel) => inDb.CustomerId == inModel.CustomerId,
                (inDb, inModel) => inDb.PropertyOwnershipId == newPropertyOwnership.Id && inDb.Dong == inModel.Dong,
                (inDb, inModel) =>
                {
                    inDb.PropertyOwnershipId = newPropertyOwnership.Id;
                    inDb.Dong = inModel.Dong;
                },
                null,
                false).ConfigureAwait(false);

            await _baseService.SyncAsync(
                property.PropertyFeatures.ToList(),
                model.PropertyFeatures,
                (feature, currentUser) => new PropertyFeature
                {
                    PropertyId = property.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                },
                (inDb, inModel) => inDb.FeatureId == inModel.Id,
                null, false).ConfigureAwait(false);

            await _baseService.SyncAsync(
                property.PropertyFacilities.ToList(),
                model.PropertyFacilities,
                (facility, currentUser) => new PropertyFacility
                {
                    PropertyId = property.Id,
                    FacilityId = facility.Id
                },
                (inDb, inModel) => inDb.FacilityId == inModel.Id,
                null,
                false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task<StatusEnum> PropertySyncAsync(Property property, PropertyOwnership propertyOwnership, PropertyInputViewModel model, bool save)
        {
            await _baseService.SyncAsync(
                propertyOwnership.Ownerships.ToList(),
                model.Ownerships,
                ownership => new Ownership
                {
                    CustomerId = ownership.CustomerId,
                    Dong = ownership.Dong,
                    PropertyOwnershipId = propertyOwnership.Id,
                },
                (inDb, inModel) => inDb.CustomerId == inModel.CustomerId,
                (inDb, inModel) => inDb.PropertyOwnershipId == propertyOwnership.Id && inDb.Dong == inModel.Dong,
                (inDb, inModel) =>
                {
                    inDb.PropertyOwnershipId = propertyOwnership.Id;
                    inDb.Dong = inModel.Dong;
                },
                null,
                false).ConfigureAwait(false);

            await _baseService.SyncAsync(
                property.PropertyFeatures.ToList(),
                model.PropertyFeatures,
                (feature, currentUser) => new PropertyFeature
                {
                    PropertyId = property.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                },
                (inDb, inModel) => inDb.FeatureId == inModel.Id,
                null, false).ConfigureAwait(false);

            await _baseService.SyncAsync(
                property.PropertyFacilities.ToList(),
                model.PropertyFacilities,
                (facility, currentUser) => new PropertyFacility
                {
                    PropertyId = property.Id,
                    FacilityId = facility.Id
                },
                (inDb, inModel) => inDb.FacilityId == inModel.Id,
                null,
                false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<MethodStatus<Property>> PropertyComplexAddOrUpdateAsync(PropertyComplexInputViewModel model, bool save)
        {
            return model?.IsNew != true
                ? PropertyComplexUpdateAsync(model, save)
                : PropertyComplexAddAsync(model, save);
        }

        public Task<MethodStatus<Property>> PropertyAddOrUpdateAsync(PropertyInputViewModel model, bool save)
        {
            return model?.IsNew != true
                ? PropertyUpdateAsync(model, save)
                : PropertyAddAsync(model, save);
        }

        private async Task<MethodStatus<Property>> PropertyComplexUpdateAsync(PropertyComplexInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Property>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Property>(StatusEnum.IdIsNull, null);

            var entity = await _properties.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            var owner = entity.CurrentOwnership.Ownerships.FirstOrDefault();
            if (owner == null)
                return new MethodStatus<Property>(StatusEnum.OwnershipIsNull, null);

            if (model.Ownership.Mobile == owner.Customer.MobileNumber)
            {
                if (model.Ownership.Name != owner.Customer.Name)
                {
                    var customer = await _customerService.CustomerEntityAsync(owner.CustomerId, null).ConfigureAwait(false);
                    if (customer == null)
                        return new MethodStatus<Property>(StatusEnum.CustomerIsNull, null);

                    var (updateCustStatus, updatedcustomer) = await _baseService.UpdateAsync(customer,
                            _ => customer.Name = model.Ownership.Name,
                            null,
                            true, StatusEnum.CustomerIsNull)
                        .ConfigureAwait(false);
                    if (updateCustStatus != StatusEnum.Success && updateCustStatus != StatusEnum.NoNeedToSave)
                        return new MethodStatus<Property>(updateCustStatus, null);
                }
            }
            else
            {
                var (newOwnerStatus, newOwner) = await _customerService.OwnershipAddAsync(new OwnershipInputViewModel
                {
                    Name = model.Ownership.Name,
                    Address = model.Ownership.Address,
                    Dong = 6,
                    Mobile = model.Ownership.Mobile,
                    Phone = model.Ownership.Phone
                }, true).ConfigureAwait(false);
                if (newOwnerStatus != StatusEnum.Success)
                    return new MethodStatus<Property>(newOwnerStatus, null);

                owner = newOwner;
            }

            var (updateStatus, updatedProperty) = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.Alley = model.Alley;
                    entity.BuildingName = model.BuildingName;
                    entity.CategoryId = model.CategoryId;
                    entity.Description = model.Description;
                    entity.DistrictId = model.DistrictId;
                    entity.Flat = model.Flat;
                    entity.Floor = model.Floor;
                    entity.Number = model.Number;
                    entity.Street = model.Street;
                }, null, false, StatusEnum.PropertyIsNull).ConfigureAwait(false);

            if (updatedProperty == null)
                return new MethodStatus<Property>(StatusEnum.PropertyIsNull, null);

            if (updatedProperty.CurrentOwnership == null)
                return new MethodStatus<Property>(StatusEnum.OwnershipIsNull, null);

            await PropertyComplexSyncAsync(updatedProperty, model, updatedProperty.CurrentOwnership, owner.Customer, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedProperty, save).ConfigureAwait(false);
        }

        private async Task<MethodStatus<Property>> PropertyUpdateAsync(PropertyInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Property>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Property>(StatusEnum.IdIsNull, null);

            if (model.Ownerships?.Any() != true)
                return new MethodStatus<Property>(StatusEnum.OwnershipIsNull, null);

            var duplicate = await HasDuplicateAsync(model).ConfigureAwait(false);
            if (duplicate)
                return new MethodStatus<Property>(StatusEnum.PropertyIsAlreadyExists, null);

            var entity = await _properties.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            var (updateStatus, updatedProperty) = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.Alley = model.Alley;
                    entity.BuildingName = model.BuildingName;
                    entity.CategoryId = model.CategoryId;
                    entity.Description = model.Description;
                    entity.DistrictId = model.DistrictId;
                    entity.Flat = model.Flat;
                    entity.Floor = model.Floor;
                    entity.Number = model.Number;
                    entity.Street = model.Street;
                }, null, false, StatusEnum.PropertyIsNull).ConfigureAwait(false);

            if (updatedProperty == null)
                return new MethodStatus<Property>(StatusEnum.PropertyIsNull, null);

            if (updatedProperty.CurrentOwnership == null)
                return new MethodStatus<Property>(StatusEnum.OwnershipIsNull, null);

            await PropertySyncAsync(updatedProperty, updatedProperty.CurrentOwnership, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedProperty, save).ConfigureAwait(false);
        }

        public async Task<MethodStatus<PropertyOwnership>> PropertyOwnershipAddAsync(string propertyId, bool save)
        {
            var newPropertyOwnership = await _baseService.AddAsync(new PropertyOwnership
            {
                PropertyId = propertyId
            }, null, save).ConfigureAwait(false);
            return newPropertyOwnership;
        }
    }
}