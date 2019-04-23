﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.Extensions.KavenNegarProvider;
using RealEstate.Services.Extensions.KavenNegarProvider.Response;
using RealEstate.Services.Extensions.KavenNegarProvider.Response.ResultModels;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface ISmsService
    {
        Task<PaginationViewModel<SmsViewModel>> ListAsync(SmsSearchViewModel searchModel);
        Task<(StatusEnum, List<Sms>)> SendAsync(List<string> recipients, string message);

        Task<(StatusEnum, List<Sms>)> SendAsync(string[] recipients, SmsTemplateEnum templateEnum, params string[] tokens);
    }

    public class SmsService : ISmsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IKavehNegarProvider _kavehNegarProvider;
        private readonly DbSet<Sms> _smses;

        public SmsService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IKavehNegarProvider kavehNegarProvider
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _kavehNegarProvider = kavehNegarProvider;
            _smses = _unitOfWork.Set<Sms>();
        }

        public const string TemplateToken = "%%%";

        public async Task<PaginationViewModel<SmsViewModel>> ListAsync(SmsSearchViewModel searchModel)
        {
            var models = _smses.AsQueryable();
            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Sms, SmsViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin))
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<(StatusEnum, List<Sms>)> SendAsync(List<string> recipients, string message)
        {
            if (recipients?.Any() != true)
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.RecipientIsNull, default);

            var status = _kavehNegarProvider.Send(recipients, message);
            if (status?.Result?.Any() != true)
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.UnexpectedError, default);

            var finalSmses = new List<Sms>();
            foreach (var smsResult in status.Result)
            {
                var (smsAddStatus, newSms) = await _baseService.AddAsync(
                    new Sms
                    {
                        Provider = SmsProvider.KavehNegar,
                        Receiver = smsResult.Receptor,
                        Sender = smsResult.Sender,
                        Text = smsResult.Message,
                        ReferenceId = smsResult.MessageId.ToString(),
                        StatusJson = JsonConvert.SerializeObject(smsResult)
                    }, null, false).ConfigureAwait(false);

                if (smsAddStatus == StatusEnum.Success)
                    finalSmses.Add(newSms);
            }

            await _baseService.SaveChangesAsync(true).ConfigureAwait(false);
            return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.Success, finalSmses);
        }

        public async Task<(StatusEnum, List<Sms>)> SendAsync(string[] recipients, SmsTemplateEnum templateEnum, params string[] tokens)
        {
            if (recipients?.Any() != true)
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.RecipientIsNull, default);

            var status = new Response<List<Send>>();
            foreach (var recipient in recipients)
            {
                var token1 = tokens[0];
                var token2 = tokens.Length > 1 ? tokens[1] : null;
                var token3 = tokens.Length > 2 ? tokens[2] : null;
                status = _kavehNegarProvider.VerifyLookup(recipient, token1, token2, token3, templateEnum.GetDisplayName());
            }

            if (status?.Result?.Any() != true)
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.UnexpectedError, default);

            var finalSmses = new List<Sms>();
            foreach (var smsResult in status.Result)
            {
                var (smsAddStatus, newSms) = await _baseService.AddAsync(
                    new Sms
                    {
                        Provider = SmsProvider.KavehNegar,
                        Receiver = smsResult.Receptor,
                        Sender = smsResult.Sender,
                        Text = smsResult.Message,
                        ReferenceId = smsResult.MessageId.ToString(),
                        StatusJson = JsonConvert.SerializeObject(smsResult)
                    }, null, false).ConfigureAwait(false);

                if (smsAddStatus == StatusEnum.Success)
                    finalSmses.Add(newSms);
            }

            await _baseService.SaveChangesAsync(true).ConfigureAwait(false);
            return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.Success, finalSmses);
        }
    }
}