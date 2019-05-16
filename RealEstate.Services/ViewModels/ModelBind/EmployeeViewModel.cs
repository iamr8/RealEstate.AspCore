using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class EmployeeViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Employee Entity { get; }

        public EmployeeViewModel(Employee entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string FirstName => Entity?.FirstName;

        public string LastName => Entity?.LastName;

        public string Mobile => Entity?.Mobile;
        public string Address => Entity?.Address;
        public string Phone => Entity?.Phone;

        public Lazy<List<UserViewModel>> Users => LazyLoadExtension.LazyLoad(() => Entity?.Users.Map<User, UserViewModel>());

        public Lazy<List<PictureViewModel>> Pictures => LazyLoadExtension.LazyLoad(() => Entity?.Pictures.Map<Picture, PictureViewModel>());
        public Lazy<List<FixedSalaryViewModel>> FixedSalaries => LazyLoadExtension.LazyLoad(() => Entity?.FixedSalaries.Map<FixedSalary, FixedSalaryViewModel>());

        public Lazy<List<PaymentViewModel>> Payments => LazyLoadExtension.LazyLoad(() => Entity?.Payments.Map<Payment, PaymentViewModel>());

        public Lazy<List<ManagementPercentViewModel>> ManagementPercents => LazyLoadExtension.LazyLoad(() => Entity?.ManagementPercents.Map<ManagementPercent, ManagementPercentViewModel>());

        public Lazy<List<InsuranceViewModel>> Insurances => LazyLoadExtension.LazyLoad(() => Entity?.Insurances.Map<Insurance, InsuranceViewModel>());

        public Lazy<List<LeaveViewModel>> Leaves => LazyLoadExtension.LazyLoad(() => Entity?.Leaves.Map<Leave, LeaveViewModel>());
        public Lazy<List<PresenceViewModel>> Presences => LazyLoadExtension.LazyLoad(() => Entity?.Presences.Map<Presence, PresenceViewModel>());
        public Lazy<List<EmployeeDivisionViewModel>> EmployeeDivisions => LazyLoadExtension.LazyLoad(() => Entity?.EmployeeDivisions.Map<EmployeeDivision, EmployeeDivisionViewModel>());

        public FixedSalaryViewModel CurrentFixedSalary() => FixedSalaries.LazyLoadLast();

        public InsuranceViewModel CurrentInsurance() => Insurances.LazyLoadLast();

        public UserViewModel CurrentUser() => Users.LazyLoadLast();
    }
}