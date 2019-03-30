using System.Collections.Generic;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    /// <summary>
    /// Not to be tracked, because it's kind of TRACKER
    /// </summary>
    public class Deal : BaseEntity
    {
        public Deal()
        {
            Pictures = new HashSet<Picture>();
            DealPayments = new HashSet<DealPayment>();
            Beneficiaries = new HashSet<Beneficiary>();
        }

        public string Description { get; set; }

        public virtual ItemRequest ItemRequest { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<DealPayment> DealPayments { get; set; }
    }
}