using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFSecondLevelCache.Core;
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

namespace RealEstate.Services.ServiceLayer
{
    public interface IPropertyService
    {
        Task<StatusEnum> PropertyRemoveAsync(string id);

        Task<MethodStatus<Property>> PropertyAsync(PropertyInputViewModel model);

        Task<PropertyJsonViewModel> PropertyJsonAsync(string id);

        Task<bool> PropertyValidate(string id);

        Task<PaginationViewModel<PropertyViewModel>> PropertyListAsync(PropertySearchViewModel searchModel);
    }

    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly ICustomerService _customerService;
        private readonly DbSet<Property> _properties;

        public PropertyService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            ICustomerService customerService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _customerService = customerService;
            _properties = _unitOfWork.Set<Property>();
        }

        public async Task<StatusEnum> PropertyRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _properties.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<PropertyJsonViewModel> PropertyJsonAsync(string id)
        {
            var entity = await _properties
                .Include(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Customer)
                .Include(x => x.Category)
                .Include(x => x.District)
                .Include(x => x.PropertyFacilities)
                .ThenInclude(x => x.Facility)
                .Include(x => x.PropertyFeatures)
                .ThenInclude(x => x.Feature)
                .Where(x => x.Id == id).Cacheable().FirstOrDefaultAsync();

            var propertyOwnership = entity?.CurrentPropertyOwnership;
            if (propertyOwnership == null)
                return default;

            var result = new PropertyJsonViewModel
            {
                Id = entity.Id,
                District = entity.District.Name,
                Description = entity.Description,
                Address = entity.Address,
                Category = entity.Category.Name,
                Features = entity.PropertyFeatures.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.FeatureId,
                    Name = x.Feature.Name,
                    Value = x.Value
                }).ToList(),
                Facilities = entity.PropertyFacilities.Select(x => new FacilityJsonViewModel
                {
                    Id = x.FacilityId,
                    Name = x.Facility.Name,
                }).ToList(),
                Ownerships = propertyOwnership.Ownerships.Select(x => new OwnershipJsonViewModel
                {
                    CustomerId = x.Customer?.Id,
                    Name = x.Customer?.Name,
                    Mobile = x.Customer?.MobileNumber,
                    Dong = x.Dong
                }).ToList()
            };
            return result;
        }

        public async Task<bool> PropertyValidate(string id)
        {
            var property = await _properties.AnyAsync(x => x.Id == id);
            return property;
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
                item => item.Map<PropertyViewModel>());

            return result;
        }

        private async Task PropertySyncAsync(Property property, PropertyInputViewModel model, PropertyOwnership propertyOwnership, Customer newCustomer)
        {
            if (model == null)
                throw new ArgumentNullException($"{nameof(model)} cannot be null");

            if (propertyOwnership != null)
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
                propertyOwnership.Ownerships?.ToList(),
                syncOwnership,
                (ownership, currentUser) => new Ownership
                {
                    CustomerId = ownership.CustomerId,
                    Dong = ownership.Dong,
                    PropertyOwnershipId = propertyOwnership.Id,
                },
                inDb => new
                {
                    inDb.CustomerId,
                    inDb.Dong
                },
                (inDb, inModel) => inDb.CustomerId == inModel.CustomerId,
                (inDb, inModel) => inDb.PropertyOwnershipId == propertyOwnership.Id && inDb.Dong == inModel.Dong,
                (inDb, inModel) =>
                {
                    inDb.PropertyOwnershipId = propertyOwnership.Id;
                    inDb.Dong = inModel.Dong;
                },
                null);
            }

            if (property != null)
            {
                await _baseService.SyncAsync(
                    property.PropertyFeatures?.ToList(),
                    model.PropertyFeatures,
                    (feature, currentUser) => new PropertyFeature
                    {
                        PropertyId = property.Id,
                        FeatureId = feature.Id,
                        Value = feature.Value,
                    },
                    inDb => new
                    {
                        inDb.FeatureId,
                        inDb.Value
                    },
                    (inDb, inModel) => inDb.FeatureId == inModel.Id,
                    (inDb, inModel) => inDb.Value == inModel.Value,
                    (inDb, inModel) => inDb.Value = inModel.Value,
                    null);

                await _baseService.SyncAsync(
                    property.PropertyFacilities?.ToList(),
                    model.PropertyFacilities,
                    (facility, currentUser) => new PropertyFacility
                    {
                        PropertyId = property.Id,
                        FacilityId = facility.Id
                    },
                    inDb => inDb.FacilityId,
                    (inDb, inModel) => inDb.FacilityId == inModel.Id,
                    null,
                    null,
                    null);
            }
        }

        public async Task<MethodStatus<Property>> PropertyAsync(PropertyInputViewModel model)
        {
            if (model == null)
                return new MethodStatus<Property>(StatusEnum.ModelIsNull);

            if (model.Ownership == null)
                return new MethodStatus<Property>(StatusEnum.OwnershipIsNull);

            Property property;
            bool isPropertySuccess;
            StatusEnum propertyStatus;

            var existingProperty = await _properties
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.BuildingName == model.BuildingName
                                          && x.CategoryId == model.CategoryId
                                          && x.DistrictId == model.DistrictId
                                          && x.Flat == model.Flat
                                          && x.Floor == model.Floor
                                          && x.Number == model.Number
                                          && x.Street == model.Address);
            if (existingProperty == null)
            {
                (propertyStatus, property, isPropertySuccess) = await _baseService.AddAsync(new Property
                {
                    Street = model.Address,
                    BuildingName = model.BuildingName,
                    CategoryId = model.CategoryId,
                    DistrictId = model.DistrictId,
                    Flat = model.Flat,
                    Floor = model.Floor,
                    Number = model.Number,
                }, null, true);
            }
            else
            {
                (propertyStatus, property, isPropertySuccess) = await _baseService.UpdateAsync(existingProperty,
                    _ =>
                    {
                        if (string.IsNullOrEmpty(existingProperty.Street))
                            existingProperty.Street = model.Address;

                        if (string.IsNullOrEmpty(existingProperty.CategoryId))
                            existingProperty.CategoryId = model.CategoryId;

                        if (string.IsNullOrEmpty(existingProperty.DistrictId))
                            existingProperty.DistrictId = model.DistrictId;

                        existingProperty.Flat = model.Flat;
                        existingProperty.Floor = model.Floor;
                        existingProperty.BuildingName = model.BuildingName;
                        existingProperty.Number = model.Number;
                    }, null, true, StatusEnum.PropertyIsNull);
            }
            if (!isPropertySuccess)
                return new MethodStatus<Property>(propertyStatus);

            var propertyOwnership = property.CurrentPropertyOwnership;
            if (propertyOwnership == null)
            {
                StatusEnum propertyOwnershipStatus;
                bool isPropertyOwnershipSuccess;
                (propertyOwnershipStatus, propertyOwnership, isPropertyOwnershipSuccess) = await _baseService.AddAsync(new PropertyOwnership
                {
                    PropertyId = property.Id,
                }, null, true);
                if (!isPropertyOwnershipSuccess)
                    return new MethodStatus<Property>(propertyOwnershipStatus);
            }

            // GetOrAdd Ownership-Customer
            var (ownershipStatus, ownership, isOwnershipSuccess) = await _customerService.OwnershipAsync(model.Ownership, propertyOwnership.Id);
            if (!isOwnershipSuccess)
                return new MethodStatus<Property>(ownershipStatus);

            await PropertySyncAsync(property, model, propertyOwnership, ownership.Customer);
            return new MethodStatus<Property>(propertyStatus, existingProperty);
        }
    }
}