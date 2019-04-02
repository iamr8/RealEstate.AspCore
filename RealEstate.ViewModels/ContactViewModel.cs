using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class ContactViewModel : BaseLogViewModel
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }
        public List<SmsViewModel> Smses { get; set; }
        public List<ApplicantViewModel> Applicants { get; set; }
        public List<OwnershipViewModel> Ownerships { get; set; }
    }
}