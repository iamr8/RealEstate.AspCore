﻿using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.ViewModels.Input;
using System;
using System.Threading.Tasks;
using Beneficiary = RealEstate.Services.Database.Tables.Beneficiary;
using Deal = RealEstate.Services.Database.Tables.Deal;
using DealPayment = RealEstate.Services.Database.Tables.DealPayment;
using IUnitOfWork = RealEstate.Services.Database.IUnitOfWork;

namespace RealEstate.Services
{
    public interface IDealService
    {
        Task<(StatusEnum, Deal)> DealAddAsync(DealInputViewModel model, string itemRequestId, bool save);
    }

    public class DealService : IDealService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;

        public DealService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
        }

        public async Task<(StatusEnum, Deal)> DealAddAsync(DealInputViewModel model, string itemRequestId, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ModelIsNull, null);

            var (dealAddStatus, newDeal) = await _baseService.AddAsync(new Deal
            {
                Description = model.Description,
                ItemRequestId = itemRequestId,
            }, new[]
                {
                    Role.SuperAdmin
                },
                false).ConfigureAwait(false);

            var syncBeneficiaries = await _baseService.SyncAsync(
                newDeal.Beneficiaries,
                model.Beneficiaries,
                beneficiary => new Beneficiary
                {
                    CommissionPercent = beneficiary.CommissionPercent,
                    DealId = newDeal.Id,
                    TipPercent = beneficiary.TipPercent,
                    UserId = beneficiary.UserId
                }, (currentBeneficiary, newBeneficiary) => currentBeneficiary.UserId == newBeneficiary.UserId, new[]
                {
                    Role.SuperAdmin
                }, false).ConfigureAwait(false);

            var syncPayments = await _baseService.SyncAsync(
                newDeal.DealPayments,
                model.DealPayments,
                payment => new DealPayment
                {
                    CommissionPrice = payment.Commission,
                    DealId = newDeal.Id,
                    PayDate = payment.PayDate,
                    Text = payment.Text,
                    TipPrice = payment.Tip
                }, (currentPayment, newPayment) => currentPayment.Id == newPayment.Id, new[]
                {
                    Role.SuperAdmin
                }, false).ConfigureAwait(false);

            return await _baseService.SaveChangesAsync(newDeal, save).ConfigureAwait(false);
        }
    }
}