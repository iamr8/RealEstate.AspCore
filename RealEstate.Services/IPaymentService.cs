using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using System;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Extensions;

namespace RealEstate.Services
{
    public interface IPaymentService
    {
        //Task<(StatusEnum, Payment)> PaymentAddAsync(PaymentInputViewModel model, bool save);
        Task<(StatusEnum, FixedSalary)> FixedSalarySyncAsync(double value, string employeeId, bool save);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<FixedSalary> _fixedSalaries;

        public PaymentService(
            IBaseService baseService,
            IUnitOfWork unitOfWork
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
            _fixedSalaries = _unitOfWork.Set<FixedSalary>();
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