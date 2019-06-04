using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class OwnershipViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Ownership Entity { get; }

        public OwnershipViewModel(Ownership entity, Action<OwnershipViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Description => Entity?.Description;
        public int Dong => Entity?.Dong ?? 0;

        public CustomerViewModel Customer { get; set; }

        public PropertyOwnershipViewModel PropertyOwnership { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}