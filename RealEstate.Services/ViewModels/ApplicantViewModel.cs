using RealEstate.Base.Enums;
using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class ApplicantViewModel : BaseLogViewModel<Applicant>
    {
        public ApplicantViewModel(Applicant entity, bool showDeleted) : base(entity)
        {
            Type = Entity.Type;
            Description = Entity.Description;
            Address = Entity.Address;
            Name = Entity.Name;
            Phone = Entity.PhoneNumber;
            Id = Entity.Id;
        }

        public ApplicantViewModel()
        {
        }

        public string Name { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }

        public string Description { get; set; }
        public ApplicantTypeEnum Type { get; set; }
        public ContactViewModel Contact { get; set; }
        public UserViewModel User { get; set; }
        public List<FeatureValueViewModel> ApplicantFeatures { get; set; }
    }
}