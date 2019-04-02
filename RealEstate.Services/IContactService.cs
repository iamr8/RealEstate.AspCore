using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Services.Base;
using RealEstate.ViewModels.Input;
using System;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IContactService
    {
        Task<(StatusEnum, Contact)> ContactAddAsync(ContactInputViewModel model, bool save);

        Task<(StatusEnum, Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, bool save);

        Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save);

        Task<Applicant> ApplicantFindEntityAsync(string id);

        Task<(StatusEnum, Ownership)> OwnershipUpdatePropertyAsync(string ownerId, string propertyOwnershipId, bool save);

        Task<(StatusEnum, Applicant)> ApplicantUpdateItemRequestAsync(string applicantId, string itemRequestId, bool save);

        Task<Ownership> OwnershipFindEntityAsync(string id);
    }

    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<Contact> _contacts;
        private readonly DbSet<Applicant> _applicants;
        private readonly DbSet<Ownership> _ownerships;

        public ContactService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _contacts = _unitOfWork.Set<Contact>();
            _applicants = _unitOfWork.Set<Applicant>();
            _ownerships = _unitOfWork.Set<Ownership>();
        }

        public async Task<Applicant> ApplicantFindEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _applicants.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<Ownership> OwnershipFindEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _ownerships.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<(StatusEnum, Applicant)> ApplicantUpdateItemRequestAsync(string applicantId, string itemRequestId, bool save)
        {
            if (string.IsNullOrEmpty(applicantId) || string.IsNullOrEmpty(itemRequestId))
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ParamIsNull, null);

            var applicant = await ApplicantFindEntityAsync(applicantId).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(applicant,
                () => applicant.ItemRequestId = itemRequestId,
                null, save, StatusEnum.ApplicantIsNull
            ).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<(StatusEnum, Ownership)> OwnershipUpdatePropertyAsync(string ownerId, string propertyOwnershipId, bool save)
        {
            if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(propertyOwnershipId))
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.ParamIsNull, null);

            var ownership = await OwnershipFindEntityAsync(ownerId).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(ownership,
                () => ownership.PropertyOwnershipId = propertyOwnershipId,
                null, save, StatusEnum.OwnershipIsNull
            ).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<(StatusEnum, Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ModelIsNull, null);

            var (contactAddStatus, newContact) = await ContactAddAsync(new ContactInputViewModel
            {
                Address = model.Address,
                Description = model.Description,
                Mobile = model.Mobile,
                Name = model.Name,
                Phone = model.Phone
            }, false).ConfigureAwait(false);
            if (contactAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Applicant>(contactAddStatus, null);

            var (addStatus, newApplicant) = await _baseService.AddAsync(currentUser => new Applicant
            {
                ContactId = newContact.Id,
                UserId = currentUser.Id,
                Type = model.Type,
            }, null, false).ConfigureAwait(false);

            var syncFeaturesStatus = await _baseService.SyncAsync(
                newApplicant.ApplicantFeatures,
                model.ApplicantFeatures,
                (feature, currentUser) => new ApplicantFeature
                {
                    ApplicantId = newApplicant.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                null,
                false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newApplicant, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.ModelIsNull, null);

            var (contactAddStatus, newContact) = await ContactAddAsync(new ContactInputViewModel
            {
                Address = model.Address,
                Description = model.Description,
                Mobile = model.Mobile,
                Name = model.Name,
                Phone = model.Phone
            }, true).ConfigureAwait(false);
            if (contactAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Ownership>(contactAddStatus, null);

            var addStatus = await _baseService.AddAsync(new Ownership
            {
                ContactId = newContact.Id,
                Dong = model.Dong,
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, Contact)> ContactAddAsync(ContactInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Contact>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(new Contact
            {
                Address = model.Address,
                Description = model.Description,
                MobileNumber = model.Mobile,
                Name = model.Name,
                PhoneNumber = model.Phone
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }
    }
}