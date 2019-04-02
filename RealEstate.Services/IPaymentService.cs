using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Services.Base;
using RealEstate.ViewModels.Input;
using System;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IPaymentService
    {
        Task<(StatusEnum, Payment)> PaymentAddAsync(PaymentInputViewModel model, bool save);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;

        public PaymentService(
            IBaseService baseService,
            IUnitOfWork unitOfWork
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
        }

        public async Task<(StatusEnum, Payment)> PaymentAddAsync(PaymentInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Payment>(StatusEnum.ModelIsNull, null);

            var newPayment = await _baseService.AddAsync(new Payment
            {
                Text = model.Text,
                Type = model.Type,
                Value = model.Value,
                UserId = model.UserId
            }, null, save).ConfigureAwait(false);
            return newPayment;
        }
    }
}