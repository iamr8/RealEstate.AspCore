using RealEstate.Base;
using RealEstate.Base.Enums;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class ApplicantViewModel : BaseLogViewModel
    {
        public string Name { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }

        public string Description { get; set; }
        public ApplicantTypeEnum Type { get; set; }
        public ContactViewModel Contact { get; set; }
        public UserViewModel User { get; set; }
        public List<FeatureValueViewModel> Features { get; set; }
    }
}