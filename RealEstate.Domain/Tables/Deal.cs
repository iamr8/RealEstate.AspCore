using RealEstate.Domain.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class Deal : BaseEntity
    {
        public Deal()
        {
            Pictures = new HashSet<Picture>();
            DealPayments = new HashSet<DealPayment>();
            Beneficiaries = new HashSet<Beneficiary>();
            Logs = new HashSet<Log>();
        }

        public string Description { get; set; }

        public virtual ItemRequest ItemRequest { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<DealPayment> DealPayments { get; set; }
    }
}