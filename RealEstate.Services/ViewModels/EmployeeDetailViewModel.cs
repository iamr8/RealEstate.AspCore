using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class EmployeeDetailViewModel : BaseViewModel
    {
        public EmployeeDetailPaymentViewModel Payments { get; set; }
    }

    public class EmployeeDetailPaymentViewModel
    {
        public double Obligee { get; set; }
        public double Obligor { get; set; }
        public List<PaymentViewModel> List { get; set; }
    }
}