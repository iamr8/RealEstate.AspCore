using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class OwnershipViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Ownership Entity { get; }

        public OwnershipViewModel(Ownership entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Description => Entity?.Description;
        public int Dong => Entity?.Dong ?? 0;

        public Lazy<CustomerViewModel> Customer =>
            LazyLoadExtension.LazyLoad(() => Entity?.Customer.Map<Customer, CustomerViewModel>());

        public Lazy<PropertyOwnershipViewModel> PropertyOwnership =>
            LazyLoadExtension.LazyLoad(() => Entity?.PropertyOwnership.Map<PropertyOwnership, PropertyOwnershipViewModel>());
    }
}