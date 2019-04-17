using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class OwnershipViewModel : BaseLogViewModel<Ownership>
    {
        [JsonIgnore]
        private readonly Ownership _entity;

        public OwnershipViewModel(Ownership entity, bool includeDeleted, Action<OwnershipViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public void GetContact(bool includeDeleted = false, Action<ContactViewModel> action = null)
        {
            Contact = _entity?.Contact.Into(includeDeleted, action);
        }

        public void GetPropertyOwnership(bool includeDeleted = false, Action<PropertyOwnershipViewModel> action = null)
        {
            PropertyOwnership = _entity?.PropertyOwnership.Into(includeDeleted, action);
        }

        public string Description => _entity.Description;
        public int Dong => _entity.Dong;
        public ContactViewModel Contact { get; private set; }
        public PropertyOwnershipViewModel PropertyOwnership { get; private set; }
    }
}