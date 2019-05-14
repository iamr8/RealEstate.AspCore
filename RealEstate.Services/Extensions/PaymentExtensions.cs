using RealEstate.Base.Enums;
using RealEstate.Services.ViewModels;
using System.Collections.Generic;
using System.Linq;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Services.Extensions
{
    public static class PaymentExtensions
    {
        public static EmployeeDetailPaymentViewModel Calculate(this List<PaymentViewModel> payments)
        {
            if (payments?.Any() != true)
                return default;

            var pays = payments.OrderByCreationDateTime().ToList();
            double currentMoney = 0;
            foreach (var payment in pays)
            {
                var type = payment.Type;
                var value = payment.Value;

                if (payment.IsDeleted)
                    continue;

                switch (type)
                {
                    case PaymentTypeEnum.Advance:
                        currentMoney -= value;
                        break;

                    case PaymentTypeEnum.Beneficiary:
                        currentMoney += value;
                        break;

                    case PaymentTypeEnum.Forfeit:
                        currentMoney -= value;
                        break;

                    case PaymentTypeEnum.Gift:
                        currentMoney += value;
                        break;

                    case PaymentTypeEnum.Checkout:
                        currentMoney -= value;
                        break;

                    case PaymentTypeEnum.Salary:
                    default:
                        currentMoney += value;
                        break;
                }
            }
            return new EmployeeDetailPaymentViewModel
            {
                Current = currentMoney < 0 ? currentMoney * -1 : currentMoney,
                Status = currentMoney < 0 ? ObligStatusEnum.Obligor : ObligStatusEnum.Obligee,
                List = pays.OrderDescendingByCreationDateTime().ToList()
            };
        }
    }
}