using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels
{
    public class EmployeeViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly Employee _entity;

        public EmployeeViewModel(Employee entity, bool includeDeleted, Action<EmployeeViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string FirstName => _entity.FirstName;

        public string LastName => _entity.LastName;

        public string Mobile => _entity.Mobile;
        public string Address => _entity.Address;
        public string Phone => _entity.Phone;

        public void GetEmployeeStatuses(bool includeDeleted = false, Action<EmployeeStatusViewModel> action = null)
        {
            EmployeeStatuses = _entity?.EmployeeStatuses.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = _entity?.Pictures.Into(includeDeleted, action);
        }

        public void GetSmses(bool includeDeleted = false, Action<SmsViewModel> action = null)
        {
            Smses = _entity?.Smses.Into(includeDeleted, action);
        }

        public void GetPayments(bool includeDeleted = false, Action<PaymentViewModel> action = null)
        {
            Payments = _entity?.Payments.Into(includeDeleted, action);
        }

        public void GetFixedSalaries(bool includeDeleted = false, Action<FixedSalaryViewModel> action = null)
        {
            FixedSalaries = _entity?.FixedSalaries.Into(includeDeleted, action);
        }

        public void GetUsers(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            Users = _entity?.Users.Into(includeDeleted, action);
        }

        public void GetManagementPercents(bool includeDeleted = false, Action<ManagementPercentViewModel> action = null)
        {
            ManagementPercents = _entity?.ManagementPercents.Into(includeDeleted, action);
        }

        public void GetLeaves(bool includeDeleted = false, Action<LeaveViewModel> action = null)
        {
            Leaves = _entity?.Leaves.Into(includeDeleted, action);
        }

        public void GetPresences(bool includeDeleted = false, Action<PresenceViewModel> action = null)
        {
            Presences = _entity?.Presences.Into(includeDeleted, action);
        }

        public void GetEmployeeDivisions(bool includeDeleted = false, Action<EmployeeDivisionViewModel> action = null)
        {
            EmployeeDivisions = _entity?.EmployeeDivisions.Into(includeDeleted, action);
        }

        public void GetInsurances(bool includeDeleted = false, Action<InsuranceViewModel> action = null)
        {
            Insurances = _entity?.Insurances.Into(includeDeleted, action);
        }

        public List<UserViewModel> Users { get; private set; }
        public List<EmployeeStatusViewModel> EmployeeStatuses { get; private set; }
        public EmployeeStatusViewModel CurrentEmployeeStatus => EmployeeStatuses?.OrderDescendingByCreationDateTime().FirstOrDefault();
        public List<PictureViewModel> Pictures { get; private set; }
        public List<FixedSalaryViewModel> FixedSalaries { get; private set; }
        public FixedSalaryViewModel CurrentFixedSalary => FixedSalaries?.OrderDescendingByCreationDateTime().FirstOrDefault();
        public List<PaymentViewModel> Payments { get; private set; }
        public List<ManagementPercentViewModel> ManagementPercents { get; private set; }
        public List<InsuranceViewModel> Insurances { get; private set; }
        public InsuranceViewModel CurrentInsurance => Insurances?.OrderDescendingByCreationDateTime().FirstOrDefault();

        public List<LeaveViewModel> Leaves { get; private set; }
        public List<PresenceViewModel> Presences { get; private set; }
        public List<EmployeeDivisionViewModel> EmployeeDivisions { get; private set; }
        public List<SmsViewModel> Smses { get; private set; }
        public UserViewModel CurrentUser => Users?.OrderDescendingByCreationDateTime().FirstOrDefault();
    }
}