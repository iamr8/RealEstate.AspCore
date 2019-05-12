using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
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
    public interface IPropertyService
    {
        Task<(StatusEnum, Property)> PropertyAddAsync(PropertyInputViewModel model, bool save);

        Task<StatusEnum> PropertyRemoveAsync(string id);

        Task<List<PropertyJsonViewModel>> PropertyListAsync(string searchTerm);

        PropertyJsonViewModel MapJson(Property property);

        Task<(StatusEnum, Property)> PropertyAddOrUpdateAsync(PropertyInputViewModel model, bool save);

        Task<PropertyJsonViewModel> PropertyJsonAsync(string id);

        Task<bool> PropertyValidate(string id);

        Task<PropertyInputViewModel> PropertyInputAsync(string id);

        Task<Property> PropertyEntityAsync(string id);

        Task<PaginationViewModel<PropertyViewModel>> PropertyListAsync(PropertySearchViewModel searchModel);

        Task<(StatusEnum, PropertyOwnership)> PropertyOwnershipAddAsync(string propertyId, bool save);
    }

    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly ICustomerService _customerService;
        private readonly IFeatureService _featureService;
        private readonly ILocationService _locationService;
        private readonly DbSet<Property> _properties;
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
                    },
                    true,
                    true)
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

            var result = await _baseService.PaginateAsync(query, searchModel?.PageNo ?? 1,
                    item =>
                    {
                        return new PropertyViewModel(item, _baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                        {
                            act.GetCategory();
                            act.GetDistrict();
                            act.GetPropertyOwnerships(false, act2 => act2.GetOwnerships(false, act3 => act3.GetCustomer()));
                            act.GetPropertyFacilities(false, act4 => act4.GetFacility());
                            act.GetPropertyFeatures(false, act5 => act5.GetFeature());
                        });
                    })
                .ConfigureAwait(false);

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
            var viewModel = entity?.Into<Property, PropertyViewModel>(false, act =>
            {
                act.GetPropertyOwnerships(false, act2 => act2.GetOwnerships(false, act3 => act3.GetCustomer()));
                act.GetPropertyFacilities(false, act4 => act4.GetFacility());
                act.GetPropertyFeatures(false, act5 => act5.GetFeature());
                act.GetCategory();
                act.GetDistrict();
            });
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

        public async Task<(StatusEnum, Property)> PropertyAddAsync(PropertyInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.ModelIsNull, null);

            if (model.Ownerships?.Any() != true)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.OwnershipIsNull, null);

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
                return new ValueTuple<StatusEnum, Property>(StatusEnum.PropertyIsNull, null);

            var (propertyOwnershipAddStatus, newPropertyOwnership) = await PropertyOwnershipAddAsync(newProperty.Id, false).ConfigureAwait(false);
            if (propertyOwnershipAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.PropertyOwnershipIsNull, null);

            await PropertySyncAsync(newProperty, newPropertyOwnership, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newProperty, save).ConfigureAwait(false);
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
            return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);
        }

        public Task<(StatusEnum, Property)> PropertyAddOrUpdateAsync(PropertyInputViewModel model, bool save)
        {
            return model?.IsNew != true
                ? PropertyUpdateAsync(model, save)
                : PropertyAddAsync(model, save);
        }

        private async Task<(StatusEnum, Property)> PropertyUpdateAsync(PropertyInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.IdIsNull, null);

            if (model.Ownerships?.Any() != true)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.OwnershipIsNull, null);

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
                return new ValueTuple<StatusEnum, Property>(StatusEnum.PropertyIsNull, null);

            if (updatedProperty.CurrentOwnership == null)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.OwnershipIsNull, null);

            await PropertySyncAsync(updatedProperty, updatedProperty.CurrentOwnership, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedProperty, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, PropertyOwnership)> PropertyOwnershipAddAsync(string propertyId, bool save)
        {
            var newPropertyOwnership = await _baseService.AddAsync(new PropertyOwnership
            {
                PropertyId = propertyId
            }, null, save).ConfigureAwait(false);
            return newPropertyOwnership;
        }
    }
}