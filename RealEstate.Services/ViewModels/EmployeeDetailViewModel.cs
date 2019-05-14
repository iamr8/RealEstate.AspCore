using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.ViewModels.ModelBind;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class EmployeeDetailViewModel : BaseViewModel
    {
        public EmployeeDetailPaymentViewModel Payments { get; set; }
    }

    public class EmployeeDetailPaymentViewModel
    {
        public double Current { get; set; }
        public ObligStatusEnum Status { get; set; }
        public List<PaymentViewModel> List { get; set; }
    }
}