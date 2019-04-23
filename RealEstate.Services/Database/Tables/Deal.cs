using RealEstate.Services.Database.Base;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
{
    public class Deal : BaseEntity
    {
        public string Description { get; set; }
        public string Barcode { get; set; }
        public virtual DealRequest DealRequest { get; set; }
        public virtual ICollection<Check> Checks { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<DealPayment> DealPayments { get; set; }
    }
}