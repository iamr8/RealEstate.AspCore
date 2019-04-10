using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels
{
    public class ContactViewModel : BaseLogViewModel<Contact>
    {
        public ContactViewModel()
        {
        }

        public ContactViewModel(Contact entity) : base(entity)
        {
            if (entity == null)
                return;

            Id = Entity.Id;
            Mobile = Entity.MobileNumber;

            var usage = new ConcurrentDictionary<string, int>();
            if (Applicants?.Any() == true)
                foreach (var applicant in Applicants)
                    usage.AddOrUpdate(applicant.Name, 0, (key, oldValue) => oldValue + 1);

            if (Ownerships?.Any() == true)
                foreach (var ownership in Ownerships)
                    usage.AddOrUpdate(ownership.Name, 0, (key, oldValue) => oldValue + 1);

            if (usage.Count == 0)
                return;

            var max = usage.Max(c => c.Value);
            var name = usage.FirstOrDefault(x => x.Value == max);
            Name = name.Key;
        }

        public string Mobile { get; set; }
        public string Name { get; set; }
        public List<SmsViewModel> Smses { get; set; }
        public List<ApplicantViewModel> Applicants { get; set; }
        public List<OwnershipViewModel> Ownerships { get; set; }
    }
}