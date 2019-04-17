using Microsoft.EntityFrameworkCore;
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
    public interface IPropertyService
    {
        Task<(StatusEnum, Property)> PropertyAddAsync(PropertyInputViewModel model, bool save);

        Task<StatusEnum> PropertyRemoveAsync(string id);

        Task<List<PropertyJsonViewModel>> PropertyListAsync(string searchTerm);

        Task<(StatusEnum, Property)> PropertyAddOrUpdateAsync(PropertyInputViewModel model, bool save);

        Task<PropertyJsonViewModel> PropertyJsonAsync(string id);

        Task<PropertyInputViewModel> PropertyInputAsync(string id);

        Task<Property> PropertyEntityAsync(string id);

        Task<PaginationViewModel<PropertyViewModel>> PropertyListAsync(PropertySearchViewModel searchModel);

        Task<(StatusEnum, PropertyOwnership)> PropertyOwnershipAddAsync(string propertyId, bool save);
    }

    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IContactService _contactService;
        private readonly IFeatureService _featureService;
        private readonly ILocationService _locationService;
        private readonly DbSet<Property> _properties;
        private readonly DbSet<PropertyOwnership> _propertyOwnerships;

        public PropertyService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IFeatureService featureService,
            ILocationService locationService,
            IContactService contactService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _locationService = locationService;
            _featureService = featureService;
            _contactService = contactService;
            _properties = _unitOfWork.Set<Property>();
            _propertyOwnerships = _unitOfWork.Set<PropertyOwnership>();
        }

        public async Task<StatusEnum> PropertyRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var property = await PropertyEntityAsync(id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(property,
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
            var query = _properties as IQueryable<Property>;
            query = query.Include(x => x.Items)
                .Include(x => x.PropertyFacilities)
                .ThenInclude(x => x.Facility)
                .Include(x => x.PropertyFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.District)
                .Include(x => x.Category)
                .Include(x => x.District)
                .Include(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Contact);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (entity == null)
                return default;

            var result = MapJson(entity);
            return result;
        }

        public async Task<List<PropertyJsonViewModel>> PropertyListAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return default;

            var query = _properties as IQueryable<Property>;
            query = query.Include(x => x.Items)
                .Include(x => x.PropertyFacilities)
                .ThenInclude(x => x.Facility)
                .Include(x => x.PropertyFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.District)
                .Include(x => x.Category)
                .Include(x => x.District)
                .Include(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Contact);

            query = query.Where(x => EF.Functions.Like(x.Id, searchTerm.LikeExpression())
                                     || EF.Functions.Like(x.Street, searchTerm.LikeExpression())
                                     || EF.Functions.Like(x.Alley, searchTerm.LikeExpression())
                                     || EF.Functions.Like(x.BuildingName, searchTerm.LikeExpression())
                                     || EF.Functions.Like(x.Category.Name, searchTerm.LikeExpression())
                                     || EF.Functions.Like(x.District.Name, searchTerm.LikeExpression()));

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
            var models = _properties as IQueryable<Property>;
            models = models.Include(x => x.Items)
                .Include(x => x.PropertyFacilities)
                .ThenInclude(x => x.Facility)
                .Include(x => x.PropertyFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.District)
                .Include(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Contact)
                .Include(x => x.Category);

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Id))
                    models = models.Where(x => EF.Functions.Like(x.Id, searchModel.Id.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.Address))
                    models = models.Where(x => EF.Functions.Like(x.Street, searchModel.Address.LikeExpression())
                    || EF.Functions.Like(x.Alley, searchModel.Address.LikeExpression())
                    || EF.Functions.Like(x.BuildingName, searchModel.Address.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.Category))
                    models = models.Where(x => EF.Functions.Like(x.Category.Name, searchModel.Category.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.District))
                    models = models.Where(x => EF.Functions.Like(x.District.Name, searchModel.District.LikeExpression()));

                //                if (!string.IsNullOrEmpty(searchModel.Owner))
                //                    models = models.Where(x => EF.Functions.Like(x.Category.Name, searchModel.Category.LikeExpression()));
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                    item =>
                    {
                        return new PropertyViewModel(item, _baseService.IsAllowed(Role.SuperAdmin, Role.Admin), property =>
                        {
                            property.GetCategory();
                            property.GetPropertyOwnerships(action: propertyOwnership =>
                            {
                                propertyOwnership.GetOwnerships(action: ownership =>
                                {
                                    ownership.GetContact();
                                });
                            });
                        });
                    })
                .ConfigureAwait(false);

            return result;
        }

        private PropertyJsonViewModel MapJson(Property property)
        {
            var lastPropOwnership = property.PropertyOwnerships.OrderByDescending(x => x.DateTime).FirstOrDefault();
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
                    ContactId = x.Contact?.Id,
                    Name = x.Contact?.Name,
                    Mobile = x.Contact?.MobileNumber,
                    Dong = x.Dong
                }).ToList()
            };
            return prop;
        }

        public async Task<Property> PropertyEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var query = _properties as IQueryable<Property>;
            var entity = await _properties.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<PropertyInputViewModel> PropertyInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var query = _properties as IQueryable<Property>;
            query = query.Include(x => x.PropertyFacilities)
                .ThenInclude(x => x.Facility)
                .Include(x => x.PropertyFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.District)
                .Include(x => x.Category)
                .Include(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Contact);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var viewModel = entity?.Into<Property, PropertyViewModel>();
            if (viewModel == null)
                return default;

            var result = new PropertyInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                Ownerships = viewModel.CurrentPropertyOwnership?.Ownerships?.Select(x => new OwnershipJsonViewModel
                {
                    ContactId = x.Contact?.Id,
                    Name = x.Contact?.Name,
                    Mobile = x.Contact?.Mobile,
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
            {
                _unitOfWork.Detach(newProperty);
                return new ValueTuple<StatusEnum, Property>(StatusEnum.PropertyIsNull, null);
            }

            var (propertyOwnershipAddStatus, newPropertyOwnership) = await PropertyOwnershipAddAsync(newProperty.Id, false).ConfigureAwait(false);
            if (propertyOwnershipAddStatus != StatusEnum.Success)
            {
                _unitOfWork.Detach(newPropertyOwnership);
                return new ValueTuple<StatusEnum, Property>(StatusEnum.PropertyOwnershipIsNull, null);
            }

            await PropertySyncAsync(newProperty, newPropertyOwnership, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newProperty, save).ConfigureAwait(false);
        }

        private async Task<StatusEnum> PropertySyncAsync(Property property, PropertyOwnership propertyOwnership, PropertyInputViewModel model, bool save)
        {
            await _baseService.SyncAsync(
                propertyOwnership.Ownerships,
                model.Ownerships,
                ownership => new Ownership
                {
                    ContactId = ownership.ContactId,
                    Dong = ownership.Dong,
                    PropertyOwnershipId = propertyOwnership.Id,
                },
                (inDb, inModel) => inDb.ContactId == inModel.ContactId,
                (inDb, inModel) => inDb.PropertyOwnershipId == propertyOwnership.Id && inDb.Dong == inModel.Dong,
                (inDb, inModel) =>
                {
                    inDb.PropertyOwnershipId = propertyOwnership.Id;
                    inDb.Dong = inModel.Dong;
                },
                null,
                false).ConfigureAwait(false);

            await _baseService.SyncAsync(
                property.PropertyFeatures,
                model.PropertyFeatures,
                feature => new PropertyFeature
                {
                    PropertyId = property.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                },
                (inDb, inModel) => inDb.FeatureId == inModel.Id,
                null, false).ConfigureAwait(false);

            await _baseService.SyncAsync(
                property.PropertyFacilities,
                model.PropertyFacilities,
                facility => new PropertyFacility
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

            var query = _properties.Include(x => x.PropertyFacilities)
                .ThenInclude(x => x.Facility)
                .Include(x => x.PropertyFeatures)
                .ThenInclude(x => x.Feature)
                .Include(x => x.District)
                .Include(x => x.Category)
                .Include(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Contact);
            var entity = await query.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            var (updateStatus, updatedProperty) = await _baseService.UpdateAsync(entity,
                () =>
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