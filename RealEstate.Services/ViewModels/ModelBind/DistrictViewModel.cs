using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class DistrictViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public District Entity { get; }

        public DistrictViewModel(District entity, Action<DistrictViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Name => Entity?.Name;

        public List<PropertyViewModel> Properties { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}