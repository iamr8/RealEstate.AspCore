using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class OwnershipViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Ownership Entity { get; }

        public OwnershipViewModel(Ownership entity, bool includeDeleted, Action<OwnershipViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public void GetCustomer(bool includeDeleted = false, Action<CustomerViewModel> action = null)
        {
            Customer = Entity?.Customer.Into(includeDeleted, action);
        }

        public void GetPropertyOwnership(bool includeDeleted = false, Action<PropertyOwnershipViewModel> action = null)
        {
            PropertyOwnership = Entity?.PropertyOwnership.Into(includeDeleted, action);
        }

        public string Description => Entity.Description;
        public int Dong => Entity.Dong;
        public CustomerViewModel Customer { get; private set; }
        public PropertyOwnershipViewModel PropertyOwnership { get; private set; }
    }
}