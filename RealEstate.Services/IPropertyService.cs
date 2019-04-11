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

        Task<(StatusEnum, Property)> PropertyAddOrUpdateAsync(PropertyInputViewModel model, bool update, bool save);

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
        private readonly DbSet<Property> _properties;
        private readonly DbSet<PropertyOwnership> _propertyOwnerships;
        private readonly DbSet<Log> _logs;

        public PropertyService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IContactService contactService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _contactService = contactService;
            _logs = _unitOfWork.Set<Log>();
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

            query = query.Where(x => EF.Functions.Like(x.Id, searchTerm.LikeExpression()));
            query = query.Where(x => EF.Functions.Like(x.Street, searchTerm.LikeExpression())
            || EF.Functions.Like(x.Alley, searchTerm.LikeExpression())
            || EF.Functions.Like(x.BuildingName, searchTerm.LikeExpression()));
            query = query.Where(x => EF.Functions.Like(x.Category.Name, searchTerm.LikeExpression()));
            query = query.Where(x => EF.Functions.Like(x.District.Name, searchTerm.LikeExpression()));

            var models = await query.ToListAsync().ConfigureAwait(false);
            if (models?.Any() != true)
                return default;

            var result = new List<PropertyJsonViewModel>();
            foreach (var property in models)
            {
                var lastPropOwnership = property.PropertyOwnerships.OrderByDescending(x => x.DateTime).FirstOrDefault();
                if (lastPropOwnership == null)
                    continue;

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
                    Owners = lastPropOwnership.Ownerships.Select(x => new OwnershipJsonViewModel
                    {
                        Name = x.Name,
                        Id = x.Id,
                        Mobile = x.Contact.MobileNumber,
                        Dong = x.Dong
                    }).ToList()
                };
                result.Add(prop);
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
                .Include(x => x.District);

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
                item => new PropertyViewModel(item)
                    .R8Include(model =>
                    {
                        model.Facilities = model.Entity.PropertyFacilities.Select(propEntity =>
                            new PropertyFacilityViewModel(propEntity).R8Include(x => x.Facility = new FacilityViewModel(x.Entity.Facility))).ToList();
                        model.Features = model.Entity.PropertyFeatures.Select(propEntity =>
                            new PropertyFeatureViewModel(propEntity).R8Include(x => x.Feature = new FeatureViewModel(x.Entity.Feature))).ToList();
                        model.District = new DistrictViewModel(model.Entity.District);
                    })).ConfigureAwait(false);

            return result;
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

            var entity = await PropertyEntityAsync(id).ConfigureAwait(false);
            if (entity == null)
                return default;

            var viewModel = new PropertyViewModel(entity)
                .R8Include(model =>
                {
                    model.Facilities = model.Entity.PropertyFacilities.Select(propEntity =>
                        new PropertyFacilityViewModel(propEntity).R8Include(x => x.Facility = new FacilityViewModel(x.Entity.Facility))).ToList();
                    model.Features = model.Entity.PropertyFeatures.Select(propEntity =>
                        new PropertyFeatureViewModel(propEntity).R8Include(x => x.Feature = new FeatureViewModel(x.Entity.Feature))).ToList();
                    model.District = new DistrictViewModel(model.Entity.District);
                });
            if (viewModel == null)
                return default;

            var lastOwnership = viewModel.Ownerships.OrderByDescending(x => x.DateTime).FirstOrDefault();
            var result = new PropertyInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                Ownerships = lastOwnership?.Owners.Select(x => new OwnershipJsonViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Mobile = x.Contact.Mobile,
                    Dong = x.Dong
                }).ToList(),
                PropertyFacilities = viewModel.Facilities.Select(x => new FacilityJsonViewModel
                {
                    Id = x.Id,
                    Name = x.Facility.Name
                }).ToList(),
                CategoryId = viewModel.Category.Id,
                //                Latitude = viewModel.Geolocation?.Latitude ?? 0,
                PropertyFeatures = viewModel.Features.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature.Id,
                    Name = x.Feature.Name,
                    Value = x.Value
                }).ToList(),
                //                Longitude = viewModel.Geolocation?.Longitude ?? 0,
                Number = viewModel.Number,
                Street = viewModel.Street,
                Flat = viewModel.Flat,
                DistrictId = viewModel.District.Id,
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

            var (propertyOwnershipAddStatus, newPropertyOwnership) = await PropertyOwnershipAddAsync(newProperty.Id, false).ConfigureAwait(false);
            if (propertyOwnershipAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.PropertyOwnershipIsNull, null);

            //            foreach (var owner in model.Ownerships)
            //                await _contactService.OwnershipPlugPropertyAsync(owner.OwnershipId, newPropertyOwnership.Id, false).ConfigureAwait(false);
            await SyncAsync(newProperty, newPropertyOwnership, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newProperty, save).ConfigureAwait(false);
        }

        private async Task<StatusEnum> SyncAsync(Property property, PropertyOwnership propertyOwnership, PropertyInputViewModel model, bool save)
        {
            await _baseService.SyncAsync(
                propertyOwnership.Ownerships,
                model.Ownerships,
                ownership => ownership.PropertyOwnershipId = propertyOwnership.Id,
                ownership => ownership.PropertyOwnershipId = null,
                (inDb, inModel) => inDb.Id == inModel.Id,
                null, false).ConfigureAwait(false);

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

        public Task<(StatusEnum, Property)> PropertyAddOrUpdateAsync(PropertyInputViewModel model, bool update, bool save)
        {
            return update
                ? PropertyUpdateAsync(model, save)
                : PropertyAddAsync(model, save);
        }

        private async Task<(StatusEnum, Property)> PropertyUpdateAsync(PropertyInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.IdIsNull, null);

            var entity = await PropertyEntityAsync(model.Id).ConfigureAwait(false);
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

            var lastOwnership = updatedProperty.PropertyOwnerships.OrderByDescending(x => x.DateTime).FirstOrDefault();
            if (lastOwnership == null)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.OwnershipIsNull, null);

            await SyncAsync(updatedProperty, lastOwnership, model, false).ConfigureAwait(false);
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