using RealEstate.Base;
using RealEstate.Base.Enums;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class ApplicantViewModel : BaseLogViewModel
    {
        public ApplicantTypeEnum Type { get; set; }
        public ContactViewModel Contact { get; set; }
        public UserViewModel User { get; set; }
        public List<FeatureValueViewModel> Features { get; set; }
    }
}