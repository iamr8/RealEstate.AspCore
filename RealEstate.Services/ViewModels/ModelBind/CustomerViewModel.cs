using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class CustomerViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Customer Entity { get; }

        public CustomerViewModel(Customer entity, Action<CustomerViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Name => Entity?.Name;
        public string Phone => Entity?.PhoneNumber;
        public string Address => Entity?.Address;
        public string Mobile => Entity?.MobileNumber;
        public bool IsPublic => Entity?.IsPublic == true;

        public List<OwnershipViewModel> Ownerships { get; set; }

        public List<ApplicantViewModel> Applicants { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}