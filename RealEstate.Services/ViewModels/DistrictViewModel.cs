using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class DistrictViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly District _entity;

        public DistrictViewModel(District entity, bool includeDeleted, Action<DistrictViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Name => _entity.Name;

        public void GetProperties(bool includeDeleted, Action<PropertyViewModel> action = null)
        {
            Properties = _entity?.Properties.Into(includeDeleted, action);
        }

        public List<PropertyViewModel> Properties { get; private set; }
    }
}