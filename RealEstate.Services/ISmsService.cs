using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Extensions.KavenNegarProvider;
using RealEstate.Services.Extensions.KavenNegarProvider.Response;
using RealEstate.Services.Extensions.KavenNegarProvider.Response.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IUnitOfWork = RealEstate.Services.Database.IUnitOfWork;
using Sms = RealEstate.Services.Database.Tables.Sms;
using SmsTemplate = RealEstate.Services.Database.Tables.SmsTemplate;

namespace RealEstate.Services
{
    public interface ISmsService
    {
        Task<(StatusEnum, Sms)> SendAsync(string recipient, string smsTemplateId, params string[] tokens);

        Task<(StatusEnum, Sms)> SendAsync(string recipient, SmsTemplate smsTemplate, params string[] tokens);

        Task<(StatusEnum, SmsTemplate)> SmsTemplateAddAsync(string templateText, bool save);

        Task<(StatusEnum, List<Sms>)> SendAsync(string[] recipients, SmsTemplate smsTemplate, params string[] tokens);
    }

    public class SmsService : ISmsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IKavehNegarProvider _kavehNegarProvider;
        private readonly DbSet<SmsTemplate> _smsTemplates;
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
            _smsTemplates = _unitOfWork.Set<SmsTemplate>();
            _smses = _unitOfWork.Set<Sms>();
        }

        public const string TemplateToken = "%%%";

        public async Task<SmsTemplate> SmsTemplateFindEntityAsync(string smsTemplateId)
        {
            if (string.IsNullOrEmpty(smsTemplateId))
                return default;

            var result = await _smsTemplates.FirstOrDefaultAsync(x => x.Id == smsTemplateId).ConfigureAwait(false);
            return result;
        }

        public async Task<(StatusEnum, SmsTemplate)> SmsTemplateAddAsync(string templateText, bool save)
        {
            if (string.IsNullOrEmpty(templateText))
                return new ValueTuple<StatusEnum, SmsTemplate>(StatusEnum.ParamIsNull, default);

            if (!templateText.Contains(TemplateToken))
                return new ValueTuple<StatusEnum, SmsTemplate>(StatusEnum.TokenIsNull, default);

            var addStatus = await _baseService.AddAsync(
                new SmsTemplate
                {
                    Text = templateText
                },
                null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, Sms)> SendAsync(string recipient, SmsTemplate smsTemplate, params string[] tokens)
        {
            if (smsTemplate == null)
                return new ValueTuple<StatusEnum, Sms>(StatusEnum.SmsTemplateIsNull, default);

            var (smsSendStatus, smses) = await SendAsync(new[]
            {
                recipient
            }, smsTemplate, tokens).ConfigureAwait(false);

            var status = new ValueTuple<StatusEnum, Sms>(smsSendStatus, smses[0]);
            return status;
        }

        public async Task<(StatusEnum, Sms)> SendAsync(string recipient, string smsTemplateId, params string[] tokens)
        {
            if (string.IsNullOrEmpty(smsTemplateId))
                return new ValueTuple<StatusEnum, Sms>(StatusEnum.SmsTemplateIdIsNull, default);

            var smsTemplate = await SmsTemplateFindEntityAsync(smsTemplateId).ConfigureAwait(false);
            if (smsTemplate == null)
                return new ValueTuple<StatusEnum, Sms>(StatusEnum.SmsTemplateIsNull, default);

            var result = await SendAsync(recipient, smsTemplate, tokens).ConfigureAwait(false);
            return result;
        }

        public async Task<(StatusEnum, List<Sms>)> SendAsync(string[] recipients, string smsTemplateId, params string[] tokens)
        {
            if (string.IsNullOrEmpty(smsTemplateId))
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.SmsTemplateIdIsNull, default);

            var smsTemplate = await SmsTemplateFindEntityAsync(smsTemplateId).ConfigureAwait(false);
            if (smsTemplate == null)
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.SmsTemplateIsNull, default);

            var result = await SendAsync(recipients, smsTemplate, tokens).ConfigureAwait(false);
            return result;
        }

        public async Task<(StatusEnum, List<Sms>)> SendAsync(string[] recipients, SmsTemplate smsTemplate, params string[] tokens)
        {
            if (recipients?.Any() != true)
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.RecipientIsNull, default);

            if (smsTemplate == null)
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.SmsTemplateIsNull, default);

            var templateTokensCount = Regex.Matches(smsTemplate.Text, TemplateToken).Count;
            if (tokens.Length != templateTokensCount)
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.TokensCountMismatch, default);

            try
            {
                var status = new Response<List<Send>>();
                foreach (var recipient in recipients)
                    status = _kavehNegarProvider.VerifyLookup(recipient, tokens[0], tokens[1], tokens[2], smsTemplate.Text);

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
                            SmsTemplateId = smsTemplate.Id,
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
            catch
            {
                return new ValueTuple<StatusEnum, List<Sms>>(StatusEnum.UnexpectedError, default);
            }
        }
    }
}