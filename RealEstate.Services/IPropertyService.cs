using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.ViewModels.Input;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IPropertyService
    {
        Task<(StatusEnum, Property)> PropertyAddAsync(PropertyInputViewModel model, bool save);

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
                Geolocation = model.Latitude > 0 && model.Longitude > 0 ? new Point(model.Longitude, model.Latitude) : default
            }, null, false).ConfigureAwait(false);

            var (propertyOwnershipAddStatus, newPropertyOwnership) = await PropertyOwnershipAddAsync(newProperty.Id, false).ConfigureAwait(false);
            if (propertyOwnershipAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Property>(StatusEnum.PropertyOwnershipIsNull, null);

            foreach (var owner in model.Ownerships)
                await _contactService.OwnershipPlugPropertyAsync(owner.OwnershipId, newPropertyOwnership.Id, false).ConfigureAwait(false);

            await _baseService.SyncAsync(
                newProperty.PropertyFeatures,
                model.PropertyFeatures,
                (feature, currentUser) => new PropertyFeature
                {
                    PropertyId = newProperty.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                },
                (inDb, inModel) => inDb.FeatureId == inModel.Id,
                null, false).ConfigureAwait(false);

            await _baseService.SyncAsync(
                newProperty.PropertyFacilities,
                model.PropertyFacilities,
                (facility, currentUser) => new PropertyFacility
                {
                    PropertyId = newProperty.Id,
                    FacilityId = facility.Id
                },
                (inDb, inModel) => inDb.FacilityId == inModel.Id,
                null,
                false).ConfigureAwait(false);

            return await _baseService.SaveChangesAsync(newProperty, save).ConfigureAwait(false);
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