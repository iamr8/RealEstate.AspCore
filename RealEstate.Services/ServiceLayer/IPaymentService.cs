using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IPaymentService
    {
        //Task<(StatusEnum, Payment)> PaymentAddAsync(PaymentInputViewModel model, bool save);
        Task<MethodStatus<ManagementPercent>> ManagementPercentAddOrUpdateAsync(ManagementPercentInputViewModel model, bool update, bool save);

        Task<StatusEnum> PaymentRemoveAsync(string id, string employeeId);

        Task<StatusEnum> ManagementPercentRemoveAsync(string id);

        Task<(double, List<Payment>)> PaymentLastStateAsync(Employee employee);

        Task<MethodStatus<Payment>> PayAsync(string paymentId, bool save);

        Task<PaginationViewModel<PaymentViewModel>> PaymentListAsync(PaymentSearchViewModel searchModel);

        Task<ManagementPercentInputViewModel> ManagementPercentInputAsync(string id);

        Task<MethodStatus<Payment>> PaymentAddAsync(PaymentInputViewModel model, bool save);

        Task<PaginationViewModel<ManagementPercentViewModel>> ManagementPercentListAsync(ManagementPercentSearchViewModel searchModel);

        Task<(double, List<Payment>)> PaymentLastStateAsync(string employeeId);

        Task<MethodStatus<FixedSalary>> FixedSalarySyncAsync(double value, string employeeId, bool save);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IPictureService _pictureService;
        private readonly DbSet<FixedSalary> _fixedSalaries;
        private readonly DbSet<Payment> _payments;
        private readonly DbSet<Employee> _employees;
        private readonly DbSet<ManagementPercent> _managementPercents;

        public PaymentService(
            IBaseService baseService,
            IPictureService pictureService,
            IUnitOfWork unitOfWork
            )
        {
            _baseService = baseService;
            _pictureService = pictureService;
            _unitOfWork = unitOfWork;
            _fixedSalaries = _unitOfWork.Set<FixedSalary>();
            _employees = _unitOfWork.Set<Employee>();
            _payments = _unitOfWork.Set<Payment>();
            _managementPercents = _unitOfWork.Set<ManagementPercent>();
        }

        public async Task<(double, List<Payment>)> PaymentLastStateAsync(string employeeId)
        {
            var employee = await _employees.IgnoreQueryFilters().OrderDescendingByCreationDateTime().Where(x => x.Id == employeeId).FirstOrDefaultAsync()
                .ConfigureAwait(false);
            return await PaymentLastStateAsync(employee);
        }

        public async Task<(double, List<Payment>)> PaymentLastStateAsync(Employee employee)
        {
            if (employee == null)
                return default;

            var payments = employee.Payments.OrderDescendingByCreationDateTime().ToList();
            if (payments?.Any() != true)
                return default;

            var lastFixedSalary = employee.FixedSalaries.OrderDescendingByCreationDateTime().FirstOrDefault();
            if (lastFixedSalary == null)
            {
                var (addFixedSalaryStatus, fixedSalaryEntity) = await _baseService.AddAsync(new FixedSalary
                {
                    Value = 0,
                    EmployeeId = employee.Id,
                }, null, true).ConfigureAwait(false);
                if (addFixedSalaryStatus == StatusEnum.Success)
                {
                    lastFixedSalary = fixedSalaryEntity;
                }
                else
                    throw new NullReferenceException($"Unable to create new {nameof(FixedSalary)}");
            }

            var currentDt = new PersianDateTime(DateTime.Now);
            var startofMonth = new DateTime(currentDt.Year, currentDt.Month, 1, new PersianCalendar());
            var endOfMonth = new DateTime(currentDt.Year, currentDt.Month, currentDt.MonthDays, new PersianCalendar());

            var fixedSalaryForThisMonth = lastFixedSalary.Audits.Count > 0
                                          && lastFixedSalary.Audits.Find(x => x.Type == LogTypeEnum.Create).DateTime.Date < startofMonth.Date;
            if (fixedSalaryForThisMonth)
            {
                var fixedSalaryThisMonth = payments.FirstOrDefault(x => x.Type == PaymentTypeEnum.FixedSalary
                                                                        && x.Audits.Count > 0
                                                                        && x.Audits.Find(c => c.Type == LogTypeEnum.Create).DateTime.Date >= startofMonth.Date
                                                                        && x.Audits.Find(c => c.Type == LogTypeEnum.Create).DateTime.Date <= endOfMonth.Date);
                if (fixedSalaryThisMonth == null && lastFixedSalary.Value > 0)
                {
                    var paymentForFixed = await _baseService.AddAsync(new Payment
                    {
                        Value = lastFixedSalary.Value,
                        EmployeeId = employee.Id,
                        Type = PaymentTypeEnum.FixedSalary,
                    }, null, true).ConfigureAwait(false);
                }
            }

            payments = await _payments.IgnoreQueryFilters().Where(x => x.EmployeeId == employee.Id).OrderDescendingByCreationDateTime().ToListAsync().ConfigureAwait(false);
            if (payments?.Any() != true)
                return default;
            var populatedPayments = payments.GroupBy(x => x.CheckoutId);

            var list = payments;
            var currentMoney = list.Calculate();
            return new ValueTuple<double, List<Payment>>(currentMoney, list);
        }

        public async Task<MethodStatus<Payment>> PayAsync(string paymentId, bool save)
        {
            if (string.IsNullOrEmpty(paymentId))
                return new MethodStatus<Payment>(StatusEnum.ParamIsNull, null);

            var payment = await _payments.FirstOrDefaultAsync(x => x.Id == paymentId).ConfigureAwait(false);
            if (payment == null)
                return new MethodStatus<Payment>(StatusEnum.PaymentIsNull, null);

            if (payment.Type != PaymentTypeEnum.Gift && payment.Type != PaymentTypeEnum.FixedSalary)
                return new MethodStatus<Payment>(StatusEnum.Forbidden, null);

            var (checkStatus, newCheckout) = await _baseService.AddAsync(new Payment
            {
                Value = payment.Value,
                EmployeeId = payment.EmployeeId,
                Type = PaymentTypeEnum.Pay
            }, null, true).ConfigureAwait(false);
            if (checkStatus != StatusEnum.Success)
                return new MethodStatus<Payment>(StatusEnum.CheckoutIsNull, null);

            var updateStatus = await _baseService.UpdateAsync(payment,
                currentUser =>
                {
                    payment.CheckoutId = newCheckout.Id;
                }, null, save, StatusEnum.PaymentIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<MethodStatus<Payment>> PaymentAddAsync(PaymentInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Payment>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.EmployeeId))
                return new MethodStatus<Payment>(StatusEnum.EmployeeIsNull, null);

            if (model.Value <= 0)
                return new MethodStatus<Payment>(StatusEnum.PriceIsNull, null);

            var result = new List<StatusEnum>();
            Payment finalPayment;
            if (model.Type == PaymentTypeEnum.Pay)
            {
                //var (currentMoney, pays) = await PaymentLastStateAsync(model.EmployeeId).ConfigureAwait(false);
                //if (currentMoney <= 0)
                //    return new MethodStatus<Payment>(StatusEnum.PaymentsAreEmpty, null);

                var (checkStatus, newCheckout) = await _baseService.AddAsync(new Payment
                {
                    Value = model.Value,
                    EmployeeId = model.EmployeeId,
                    Text = model.Text,
                    Type = PaymentTypeEnum.Pay
                }, null, true).ConfigureAwait(false);
                if (checkStatus != StatusEnum.Success)
                    return new MethodStatus<Payment>(StatusEnum.CheckoutIsNull, null);

                //foreach (var pay in pays)
                //{
                //    var payment = await _payments.FirstOrDefaultAsync(x => x.Id == pay.Id).ConfigureAwait(false);
                //    var updateStatus = await _baseService.UpdateAsync(payment,
                //        currentUser =>
                //        {
                //            payment.CheckoutId = newCheckout.Id;
                //        }, null, true, StatusEnum.PaymentIsNull).ConfigureAwait(false);
                //    result.Add(updateStatus.Status);
                //}
                finalPayment = newCheckout;
            }
            else
            {
                var (addStatus, newPayment) = await _baseService.AddAsync(new Payment
                {
                    Type = model.Type,
                    EmployeeId = model.EmployeeId,
                    Value = model.Value,
                    Text = model.Text,
                }, new[]
                {
                        Role.SuperAdmin, Role.Admin
                    }, save).ConfigureAwait(false);
                result.Add(addStatus);
                finalPayment = newPayment;
            }

            var finalResult = result.Populate();
            if (finalResult == StatusEnum.Success)
                await _pictureService.PictureAddAsync(model.Pictures, null, null, null, null, finalPayment.Id, null, true).ConfigureAwait(false);

            return finalResult == StatusEnum.Success
                ? new MethodStatus<Payment>(StatusEnum.Success, finalPayment)
                : new MethodStatus<Payment>(StatusEnum.Failed, null);
        }

        public async Task<StatusEnum> PaymentRemoveAsync(string id, string employeeId)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var employee = await _payments.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id && x.EmployeeId == employeeId).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(employee,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
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

        public Task<MethodStatus<ManagementPercent>> ManagementPercentAddOrUpdateAsync(ManagementPercentInputViewModel model, bool update, bool save)
        {
            return update
                ? ManagementPercentUpdateAsync(model, save)
                : ManagementPercentAddAsync(model, save);
        }

        public async Task<MethodStatus<ManagementPercent>> ManagementPercentUpdateAsync(ManagementPercentInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<ManagementPercent>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<ManagementPercent>(StatusEnum.IdIsNull, null);

            if (string.IsNullOrEmpty(model.EmployeeId))
                return new MethodStatus<ManagementPercent>(StatusEnum.EmployeeIsNull, null);

            var employee = await _employees.FirstOrDefaultAsync(x => x.Id == model.EmployeeId).ConfigureAwait(false);
            if (employee == null)
                return new MethodStatus<ManagementPercent>(StatusEnum.EmployeeIsNull, null);

            var entity = await _managementPercents.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            if (entity == null)
                return new MethodStatus<ManagementPercent>(StatusEnum.ManagemenetPercentIsNull, null);

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
            var viewModel = entity.Map<ManagementPercent, ManagementPercentViewModel>();
            if (viewModel == null)
                return default;

            var result = new ManagementPercentInputViewModel
            {
                Id = viewModel.Id,
                EmployeeId = viewModel.Employee.Value?.Id,
                Percent = viewModel.Percent
            };

            return result;
        }

        public async Task<PaginationViewModel<ManagementPercentViewModel>> ManagementPercentListAsync(ManagementPercentSearchViewModel searchModel)
        {
            var query = _managementPercents.AsQueryable();

            if (searchModel?.Percent != null && searchModel.Percent > 0)
                query = query.Where(x => x.Percent == searchModel.Percent);

            var result = await _baseService.PaginateAsync(query, searchModel?.PageNo ?? 1,
                item => item.Map<ManagementPercent, ManagementPercentViewModel>()
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<PaginationViewModel<PaymentViewModel>> PaymentListAsync(PaymentSearchViewModel searchModel)
        {
            var models = _payments.AsQueryable();

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Map<Payment, PaymentViewModel>()
            ).ConfigureAwait(false);
            return result;
        }

        public async Task<MethodStatus<ManagementPercent>> ManagementPercentAddAsync(ManagementPercentInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<ManagementPercent>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.EmployeeId))
                return new MethodStatus<ManagementPercent>(StatusEnum.EmployeeIsNull, null);

            var employee = await _employees.FirstOrDefaultAsync(x => x.Id == model.EmployeeId).ConfigureAwait(false);
            if (employee == null)
                return new MethodStatus<ManagementPercent>(StatusEnum.EmployeeIsNull, null);

            var addStatus = await _baseService.AddAsync(new ManagementPercent
            {
                EmployeeId = model.EmployeeId,
                Percent = model.Percent
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<MethodStatus<FixedSalary>> FixedSalarySyncAsync(double value, string employeeId, bool save)
        {
            if (value <= 0 || string.IsNullOrEmpty(employeeId))
                return new MethodStatus<FixedSalary>(StatusEnum.ParamIsNull, null);

            var findSalary = await _fixedSalaries.OrderDescendingByCreationDateTime()
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Value.Equals(value))
                .ConfigureAwait(false);
            if (findSalary != null)
                return new MethodStatus<FixedSalary>(StatusEnum.AlreadyExists, findSalary);

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