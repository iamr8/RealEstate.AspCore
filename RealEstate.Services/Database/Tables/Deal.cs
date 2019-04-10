using RealEstate.Services.Database.Base;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
{
    public class Deal : BaseEntity
    {
        public Deal()
        {
            Pictures = new HashSet<Picture>();
            DealPayments = new HashSet<DealPayment>();
            Beneficiaries = new HashSet<Beneficiary>();
        }

        public string Description { get; set; }
        public string ItemRequestId { get; set; }
        public virtual ItemRequest ItemRequest { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<DealPayment> DealPayments { get; set; }
    }
}