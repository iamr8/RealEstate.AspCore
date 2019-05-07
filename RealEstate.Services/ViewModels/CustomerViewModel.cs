using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class CustomerViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Customer Entity { get; }

        public CustomerViewModel(Customer entity, bool includeDeleted, Action<CustomerViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Name => Entity?.Name;
        public string Phone => Entity?.PhoneNumber;
        public string Address => Entity?.Address;
        public string Mobile => Entity?.MobileNumber;
        public bool IsPublic => Entity?.IsPublic == true;

        public void GetOwnerships(bool includeDeleted = false, Action<OwnershipViewModel> action = null)
        {
            Ownerships = Entity?.Ownerships.Into(includeDeleted, action).ShowBasedOn(x => x.Customer);
        }

        public void GetApplicants(bool includeDeleted = false, Action<ApplicantViewModel> action = null)
        {
            Applicants = Entity?.Applicants.Into(includeDeleted, action).ShowBasedOn(x => x.Customer);
        }

        public List<OwnershipViewModel> Ownerships { get; private set; }
        public List<ApplicantViewModel> Applicants { get; private set; }
    }
}