using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ApplicantViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Applicant Entity { get; }

        public ApplicantViewModel(Applicant entity, Action<ApplicantViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Description => Entity?.Description;
        public ApplicantTypeEnum Type => Entity?.Type ?? ApplicantTypeEnum.Applicant;

        public CustomerViewModel Customer { get; set; }
        public UserViewModel User { get; set; }
        public ItemViewModel Item { get; set; }
        public List<ApplicantFeatureViewModel> ApplicantFeatures { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}