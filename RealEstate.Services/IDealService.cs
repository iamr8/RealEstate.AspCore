﻿using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IDealService
    {
        Task<PaginationViewModel<DealViewModel>> RequestListAsync(int pageNo);

        Task<(StatusEnum, Deal)> RequestAsync(ItemRequestInputViewModel model, bool save);
    }

    public class DealService : IDealService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IMapService _mapService;
        private readonly IContactService _contactService;
        private readonly IItemService _itemService;
        private readonly DbSet<Applicant> _applicants;
        private readonly DbSet<Contact> _contacts;
        private readonly DbSet<Deal> _deals;

        public DealService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IItemService itemService,
            IMapService mapService,
            IContactService contactService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _mapService = mapService;
            _contactService = contactService;
            _itemService = itemService;
            _applicants = _unitOfWork.Set<Applicant>();
            _contacts = _unitOfWork.Set<Contact>();
            _deals = _unitOfWork.Set<Deal>();
        }

        public async Task<PaginationViewModel<DealViewModel>> RequestListAsync(int pageNo)
        {
            var query = _deals as IQueryable<Deal>;

            var dd = from request in query
                     let applicants = (from applicant in request.Applicants
                                       let contact = applicant.Contact
                                       let applicantFeatures = (from applicantFeature in applicant.ApplicantFeatures
                                                                let features = applicantFeature.Feature
                                                                select applicantFeature)
                                       select applicant)
                     let item = request.Item
                     let itemCategory = item.Category
                     let property = item.Property
                     let propertyOwnerships = (from propertyOwnership in property.PropertyOwnerships
                                               let ownerships = (from ownership in propertyOwnership.Ownerships
                                                                 let ownershipContact = ownership.Contact
                                                                 select ownership)
                                               select propertyOwnership)
                     where request.Status == DealStatusEnum.Requested
                     select request;

            var result = await _baseService.PaginateAsync(dd, pageNo,
                item => _mapService.Map(item, _baseService.IsAllowed(Role.SuperAdmin, Role.Admin))).ConfigureAwait(false);
            return result;
        }

        public async Task<(StatusEnum, Deal)> RequestAsync(ItemRequestInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ModelIsNull, null);

            if (model?.Contacts?.Any() != true)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ApplicantsEmpty, null);

            var (requestAddStatus, newRequest) = await _baseService.AddAsync(new Deal
            {
                Description = model.Description,
                ItemId = model.ItemId,
                Status = DealStatusEnum.Requested
            }, null, false).ConfigureAwait(false);
            if (requestAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Deal>(StatusEnum.ItemRequestIsNull, null);

            await SyncApplicantsAsync(newRequest, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newRequest, save).ConfigureAwait(false);
        }

        public async Task<StatusEnum> SyncApplicantsAsync(Deal deal, ItemRequestInputViewModel model, bool save)
        {
            var allowedContacts = await _contactService.ListJsonAsync().ConfigureAwait(false);
            if (allowedContacts?.Any() != true)
                return StatusEnum.ContactIsNull;

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null) return StatusEnum.UserIsNull;

            var mustBeLeft = deal.Applicants.Where(ent => model.Contacts.Any(mdl => ent.ContactId == mdl.ContactId)).ToList();
            var mustBeRemoved = deal.Applicants.Where(x => !mustBeLeft.Contains(x)).ToList();
            if (mustBeRemoved.Count > 0)
            {
                foreach (var redundant in mustBeRemoved)
                {
                    await _baseService.UpdateAsync(redundant,
                        () => redundant.DealId = null, null, false, StatusEnum.ApplicantIsNull).ConfigureAwait(false);
                }
            }

            if (model.Contacts?.Any() != true)
                return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);

            foreach (var contact in model.Contacts)
            {
                var source = deal.Applicants.FirstOrDefault(ent => ent.ContactId == contact.ContactId);
                if (source == null)
                {
                    var cnt = await _contacts.FirstOrDefaultAsync(x => x.Id == contact.ContactId).ConfigureAwait(false);
                    if (cnt == null)
                        continue;

                    var (applicantAddStatus, newApplicant) = await _contactService.ApplicantAddAsync(new ApplicantInputViewModel
                    {
                        Address = cnt.Address,
                        Mobile = cnt.MobileNumber,
                        Name = cnt.Name,
                        Phone = cnt.PhoneNumber,
                        Type = ApplicantTypeEnum.Applicant
                    }, deal.Id, false).ConfigureAwait(false);
                }
                else
                {
                    var applicant = await _applicants.FirstOrDefaultAsync(x => x.Id == contact.ApplicantId).ConfigureAwait(false);
                    await _baseService.UpdateAsync(applicant,
                        () => applicant.DealId = deal.Id, null, false, StatusEnum.ApplicantIsNull).ConfigureAwait(false);
                }
            }

            return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);
        }
    }
}