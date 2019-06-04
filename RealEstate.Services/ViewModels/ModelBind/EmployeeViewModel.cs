using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class EmployeeViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Employee Entity { get; }

        public EmployeeViewModel(Employee entity, Action<EmployeeViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string FirstName => Entity?.FirstName;

        public string LastName => Entity?.LastName;

        public string Mobile => Entity?.Mobile;
        public string Address => Entity?.Address;
        public string Phone => Entity?.Phone;

        public List<UserViewModel> Users { get; set; }

        public List<PictureViewModel> Pictures { get; set; }
        public List<FixedSalaryViewModel> FixedSalaries { get; set; }

        public List<PaymentViewModel> Payments { get; set; }

        public List<ManagementPercentViewModel> ManagementPercents { get; set; }

        public List<InsuranceViewModel> Insurances { get; set; }

        public List<LeaveViewModel> Leaves { get; set; }
        public List<PresenceViewModel> Presences { get; set; }
        public List<EmployeeDivisionViewModel> EmployeeDivisions { get; set; }

        public FixedSalaryViewModel CurrentFixedSalary => FixedSalaries?.OrderDescendingByCreationDateTime().FirstOrDefault();

        public InsuranceViewModel CurrentInsurance => Insurances?.OrderDescendingByCreationDateTime().FirstOrDefault();

        public UserViewModel CurrentUser => Users?.OrderDescendingByCreationDateTime().FirstOrDefault();

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}