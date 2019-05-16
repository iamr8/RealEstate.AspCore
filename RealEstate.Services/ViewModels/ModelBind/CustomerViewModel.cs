using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class CustomerViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Customer Entity { get; }

        public CustomerViewModel(Customer entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Name => Entity?.Name;
        public string Phone => Entity?.PhoneNumber;
        public string Address => Entity?.Address;
        public string Mobile => Entity?.MobileNumber;
        public bool IsPublic => Entity?.IsPublic == true;

        public Lazy<List<OwnershipViewModel>> Ownerships =>
            LazyLoadExtension.LazyLoad(() => Entity?.Ownerships.Map<Ownership, OwnershipViewModel>());

        public Lazy<List<ApplicantViewModel>> Applicants =>
            LazyLoadExtension.LazyLoad(() => Entity?.Applicants.Map<Applicant, ApplicantViewModel>());
    }
}