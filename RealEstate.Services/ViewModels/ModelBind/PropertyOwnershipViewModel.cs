using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PropertyOwnershipViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public PropertyOwnership Entity { get; }

        public PropertyOwnershipViewModel(PropertyOwnership entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public Lazy<PropertyViewModel> Property =>
            LazyLoadExtension.LazyLoad(() => Entity?.Property.Map<Property, PropertyViewModel>());

        public Lazy<List<OwnershipViewModel>> Ownerships =>
            LazyLoadExtension.LazyLoad(() => Entity?.Ownerships.Map<Ownership, OwnershipViewModel>());
    }
}