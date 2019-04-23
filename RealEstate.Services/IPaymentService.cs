using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IPaymentService
    {
        //Task<(StatusEnum, Payment)> PaymentAddAsync(PaymentInputViewModel model, bool save);
        Task<(StatusEnum, ManagementPercent)> ManagementPercentAddOrUpdateAsync(ManagementPercentInputViewModel model, bool update, bool save);

        Task<StatusEnum> ManagementPercentRemoveAsync(string id);

        Task<PaginationViewModel<PaymentViewModel>> PaymentListAsync(PaymentSearchViewModel searchModel);

        Task<ManagementPercentInputViewModel> ManagementPercentInputAsync(string id);

        Task<(StatusEnum, Payment)> PaymentAddAsync(PaymentInputViewModel model, bool save, PaymentTypeEnum type = PaymentTypeEnum.Salary);

        Task<PaginationViewModel<ManagementPercentViewModel>> ManagementPercentListAsync(ManagementPercentSearchViewModel searchModel);

        Task<(StatusEnum, FixedSalary)> FixedSalarySyncAsync(double value, string employeeId, bool save);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<FixedSalary> _fixedSalaries;
        private readonly DbSet<Payment> _payments;
        private readonly DbSet<Employee> _employees;
        private readonly DbSet<ManagementPercent> _managementPercents;

        public PaymentService(
            IBaseService baseService,
            IUnitOfWork unitOfWork
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
            _fixedSalaries = _unitOfWork.Set<FixedSalary>();
            _employees = _unitOfWork.Set<Employee>();
            _payments = _unitOfWork.Set<Payment>();
            _managementPercents = _unitOfWork.Set<ManagementPercent>();
        }

        public async Task<(StatusEnum, Payment)> PaymentAddAsync(PaymentInputViewModel model, bool save, PaymentTypeEnum type = PaymentTypeEnum.Salary)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Payment>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(new Payment
            {
                Type = type,
                EmployeeId = model.EmployeeId,
                Value = model.Value,
                Text = model.Text,
            }, new[]
            {
                Role.SuperAdmin, Role.Admin
            }, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<StatusEnum> ManagementPercentRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await _managementPercents.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public Task<(StatusEnum, ManagementPercent)> ManagementPercentAddOrUpdateAsync(ManagementPercentInputViewModel model, bool update, bool save)
        {
            return update
                ? ManagementPercentUpdateAsync(model, save)
                : ManagementPercentAddAsync(model, save);
        }

        public async Task<(StatusEnum, ManagementPercent)> ManagementPercentUpdateAsync(ManagementPercentInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, ManagementPercent>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, ManagementPercent>(StatusEnum.IdIsNull, null);

            if (string.IsNullOrEmpty(model.EmployeeId))
                return new ValueTuple<StatusEnum, ManagementPercent>(StatusEnum.EmployeeIsNull, null);

            var employee = await _employees.FirstOrDefaultAsync(x => x.Id == model.EmployeeId).ConfigureAwait(false);
            if (employee == null)
                return new ValueTuple<StatusEnum, ManagementPercent>(StatusEnum.EmployeeIsNull, null);

            var entity = await _managementPercents.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            if (entity == null)
                return new ValueTuple<StatusEnum, ManagementPercent>(StatusEnum.ManagemenetPercentIsNull, null);

            var updateStatus = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.EmployeeId = model.EmployeeId;
                    entity.Percent = model.Percent;
                }, null, save, StatusEnum.ManagemenetPercentIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<ManagementPercentInputViewModel> ManagementPercentInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await _managementPercents.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var viewModel = entity.Into<ManagementPercent, ManagementPercentViewModel>(false, act =>
            {
                act.GetEmployee();
            });
            if (viewModel == null)
                return default;

            var result = new ManagementPercentInputViewModel
            {
                Id = viewModel.Id,
                EmployeeId = viewModel.Employee?.Id,
                Percent = viewModel.Percent
            };

            return result;
        }

        public async Task<PaginationViewModel<ManagementPercentViewModel>> ManagementPercentListAsync(ManagementPercentSearchViewModel searchModel)
        {
            var models = _managementPercents.AsQueryable();

            if (searchModel != null)
            {
                models = models.SearchBy(searchModel.Percent, x => x.Percent);
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<ManagementPercent, ManagementPercentViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetEmployee();
                })
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<PaginationViewModel<PaymentViewModel>> PaymentListAsync(PaymentSearchViewModel searchModel)
        {
            var models = _payments.AsQueryable();

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Payment, PaymentViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetPayments();
                    act.GetCheckout();
                    act.GetEmployee();
                })
            ).ConfigureAwait(false);
            return result;
        }

        public async Task<(StatusEnum, ManagementPercent)> ManagementPercentAddAsync(ManagementPercentInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, ManagementPercent>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.EmployeeId))
                return new ValueTuple<StatusEnum, ManagementPercent>(StatusEnum.EmployeeIsNull, null);

            var employee = await _employees.FirstOrDefaultAsync(x => x.Id == model.EmployeeId).ConfigureAwait(false);
            if (employee == null)
                return new ValueTuple<StatusEnum, ManagementPercent>(StatusEnum.EmployeeIsNull, null);

            var addStatus = await _baseService.AddAsync(new ManagementPercent
            {
                EmployeeId = model.EmployeeId,
                Percent = model.Percent
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, FixedSalary)> FixedSalarySyncAsync(double value, string employeeId, bool save)
        {
            if (value <= 0 || string.IsNullOrEmpty(employeeId))
                return new ValueTuple<StatusEnum, FixedSalary>(StatusEnum.ParamIsNull, null);

            var findSalary = await _fixedSalaries.OrderDescendingByCreationDateTime()
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Value.Equals(value))
                .ConfigureAwait(false);
            if (findSalary != null)
                return new ValueTuple<StatusEnum, FixedSalary>(StatusEnum.AlreadyExists, findSalary);

            var addStatus = await _baseService.AddAsync(new FixedSalary
            {
                Value = value,
                EmployeeId = employeeId
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        //public async Task<(StatusEnum, Payment)> PaymentAddAsync(PaymentInputViewModel model, bool save)
        //{
        //    if (model == null)
        //        return new ValueTuple<StatusEnum, Payment>(StatusEnum.ModelIsNull, null);

        //    var newPayment = await _baseService.AddAsync(new Payment
        //    {
        //        Text = model.Text,
        //        Type = model.Type,
        //        Value = model.Value,
        //        UserId = model.UserId
        //    }, null, save).ConfigureAwait(false);
        //    return newPayment;
        //}
    }
}