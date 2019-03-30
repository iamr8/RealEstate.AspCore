using RealEstate.Base;
using System.Collections.Generic;
using RealEstate.Base.Enums;

namespace RealEstate.ViewModels
{
    public class ApplicantViewModel : BaseTrackViewModel
    {
        public ContactViewModel Contact { get; set; }
        public ApplicantTypeEnum Type { get; set; }
        public List<FeatureValueViewModel> Features { get; set; }
    }
}