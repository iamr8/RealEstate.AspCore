using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class PaymentExtensions
    {
        private static double CalculateCore(this double currentMoney, PaymentTypeEnum type, double value)
        {
            switch (type)
            {
                case PaymentTypeEnum.Advance:
                case PaymentTypeEnum.Forfeit:
                case PaymentTypeEnum.Pay:
                    currentMoney -= value;
                    break;

                case PaymentTypeEnum.Beneficiary:
                case PaymentTypeEnum.Gift:
                default:
                    currentMoney += value;
                    break;
            }

            return currentMoney;
        }

        public static List<Payment> PaymentsRemained(this List<Payment> payments, BaseEntity lastRemain)
        {
            var pays = payments.Where(x =>
                x.Audits.Find(c => c.Type == LogTypeEnum.Create).DateTime > lastRemain.Audits.Find(c => c.Type == LogTypeEnum.Create).DateTime).ToList();

            // last remain, pardakhtaye anjam nashodeye ghabli ra neshan nemidahad
            return pays;
        }

        public static double Calculate(this ICollection<Payment> payments)
        {
            if (payments?.Any() != true)
                return default;

            double currentMoney = 0;
            foreach (var payment in payments)
            {
                var type = payment.Type;
                var value = payment.Value;

                if (payment.IsDeleted)
                    continue;

                if (!string.IsNullOrEmpty(payment.CheckoutId))
                    continue;

                if (payment.Type == PaymentTypeEnum.Pay)
                    continue;

                currentMoney = currentMoney.CalculateCore(type, value);
            }
            return currentMoney;
        }
    }
}