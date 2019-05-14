using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class DistrictViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public District Entity { get; }

        public DistrictViewModel(District entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Name => Entity?.Name;

        public Lazy<List<PropertyViewModel>> Properties => LazyLoadExtension.LazyLoad(() => Entity?.Properties.Into<Property, PropertyViewModel>());
    }
}