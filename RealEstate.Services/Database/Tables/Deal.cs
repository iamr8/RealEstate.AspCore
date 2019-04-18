using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
{
    public class Deal : BaseEntity
    {
        public Deal()
        {
            Applicants = new HashSet<Applicant>();
            Pictures = new HashSet<Picture>();
            Beneficiaries = new HashSet<Beneficiary>();
            DealPayments = new HashSet<DealPayment>();
        }

        public string Description { get; set; }
        public DealStatusEnum Status { get; set; }
        public string Barcode { get; set; }
        public string ItemId { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<Applicant> Applicants { get; set; }
        public virtual ICollection<Check> Checks { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<DealPayment> DealPayments { get; set; }
    }
}