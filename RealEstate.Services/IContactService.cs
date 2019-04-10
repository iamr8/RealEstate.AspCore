using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IContactService
    {
        Task<(StatusEnum, Database.Tables.Contact)> ContactAddAsync(ContactInputViewModel model, bool save);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantAddOrUpdateAsync(ApplicantInputViewModel model, bool update, bool save);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantUpdateAsync(ApplicantInputViewModel model, bool save);

        Task<Database.Tables.Contact> ContactEntityByMobileAsync(string mobile);

        Task<(StatusEnum, Database.Tables.Contact)> ContactAddOrUpdateAsync(ContactInputViewModel model, bool update, bool save);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, bool save);

        Task<PaginationViewModel<ContactViewModel>> ContactListAsync(ContactSearchViewModel searchModel);

        Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save);

        Task<(StatusEnum, Database.Tables.Contact)> ContactUpdateAsync(ContactInputViewModel model, bool save);

        Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(ApplicantSearchViewModel searchModel);

        Task<ApplicantInputViewModel> ApplicantInputAsync(string id);

        Task<Database.Tables.Applicant> ApplicantEntityAsync(string id);

        Task<StatusEnum> ApplicantRemoveAsync(string id);

        Task<(StatusEnum, Ownership)> OwnershipPlugPropertyAsync(string ownerId, string propertyOwnershipId, bool save);

        Task<Database.Tables.Contact> ContactEntityAsync(string id);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantPlugItemRequestAsync(string applicantId, string itemRequestId, bool save);

        Task<StatusEnum> ContactRemoveAsync(string id);

        Task<Ownership> OwnershipFindEntityAsync(string id);
    }

    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;

        private readonly DbSet<Database.Tables.Contact> _contacts;
        private readonly DbSet<Database.Tables.Applicant> _applicants;
        private readonly DbSet<Ownership> _ownerships;

        public ContactService(
            IUnitOfWork unitOfWork,
            IBaseService baseService

            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;

            _contacts = _unitOfWork.Set<Database.Tables.Contact>();
            _applicants = _unitOfWork.Set<Database.Tables.Applicant>();
            _ownerships = _unitOfWork.Set<Ownership>();
        }

        public async Task<Database.Tables.Contact> ContactEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _contacts.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<Database.Tables.Contact> ContactEntityByMobileAsync(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return default;

            var entity = await _contacts.FirstOrDefaultAsync(x => x.MobileNumber == mobile).ConfigureAwait(false);
            return entity;
        }

        public async Task<Database.Tables.Applicant> ApplicantEntityAsync(string id)
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

        public async Task<StatusEnum> ContactRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await ContactEntityAsync(id).ConfigureAwait(false);
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

        public async Task<StatusEnum> ApplicantRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await ApplicantEntityAsync(id).ConfigureAwait(false);
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

        public async Task<(StatusEnum, Database.Tables.Applicant)> ApplicantPlugItemRequestAsync(string applicantId, string itemRequestId, bool save)
        {
            if (string.IsNullOrEmpty(applicantId) || string.IsNullOrEmpty(itemRequestId))
                return new ValueTuple<StatusEnum, Database.Tables.Applicant>(StatusEnum.ParamIsNull, null);

            var applicant = await ApplicantEntityAsync(applicantId).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(applicant,
                () => applicant.ItemRequestId = itemRequestId,
                null, save, StatusEnum.ApplicantIsNull
            ).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<ApplicantInputViewModel> ApplicantInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var model = await ApplicantEntityAsync(id).ConfigureAwait(false);
            if (model == null)
                return default;

            var viewModel = new ApplicantViewModel(model).Include(x =>
            {
                x.ApplicantFeatures = x.Entity.ApplicantFeatures.Select(c => new FeatureValueViewModel(c).Include(v =>
                {
                    v.Feature = new FeatureViewModel(v.Entity.Feature);
                })).ToList();
                x.Contact = new ContactViewModel(x.Entity.Contact);
            });
            var result = new ApplicantInputViewModel
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Name = viewModel.Name,
                Description = viewModel.Description,
                Address = viewModel.Address,
                Mobile = viewModel.Contact.Mobile,
                Phone = viewModel.Phone,
                ApplicantFeatures = viewModel.ApplicantFeatures.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature.Id,
                    Name = x.Feature.Name,
                    Value = x.Value
                }).ToList(),
            };
            return result;
        }

        public async Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(ApplicantSearchViewModel searchModel)
        {
            var models = _applicants as IQueryable<Applicant>;
            models = models.Include(x => x.ApplicantFeatures)
                .ThenInclude(x => x.Applicant);
            models = models.Include(x => x.Contact);

            if (searchModel != null)
            {
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => new ApplicantViewModel(item)
                    .Include(model =>
                    {
                        model.ApplicantFeatures = model.Entity.ApplicantFeatures.Select(propEntity => new FeatureValueViewModel(propEntity).Include(x =>
                         {
                             x.Feature = new FeatureViewModel(x.Entity.Feature);
                         })).ToList();
                        model.Contact = new ContactViewModel(model.Entity.Contact);
                    })).ConfigureAwait(false);

            return result;
        }

        public async Task<PaginationViewModel<ContactViewModel>> ContactListAsync(ContactSearchViewModel searchModel)
        {
            var models = _contacts as IQueryable<Contact>;
            models = models.Include(x => x.Applicants);
            models = models.Include(x => x.Smses);
            models = models.Include(x => x.Ownerships);
            models = models.Where(x => x.Ownerships.Count > 0 || x.Applicants.Any(c => c.ItemRequest.Deal != null));

            if (searchModel != null)
            {
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => new ContactViewModel(item)
                    .Include(model =>
                    {
                        model.Applicants = model.Entity.Applicants.Select(propEntity => new ApplicantViewModel(propEntity)).ToList();
                        model.Ownerships = model.Entity.Ownerships.Select(propEntity => new OwnershipViewModel(propEntity)).ToList();
                    })
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<(StatusEnum, Ownership)> OwnershipPlugPropertyAsync(string ownerId, string propertyOwnershipId, bool save)
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

        public async Task<(StatusEnum, Applicant)> ApplicantUpdateAsync(ApplicantInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.IdIsNull, null);

            var entity = await ApplicantEntityAsync(model.Id).ConfigureAwait(false);
            var (updateStatus, updatedApplicant) = await _baseService.UpdateAsync(entity,
                () =>
                {
                    entity.Address = model.Address;
                    entity.Description = model.Description;
                    entity.Name = model.Name;
                    entity.PhoneNumber = model.Phone;
                    entity.Type = model.Type;
                }, null, save, StatusEnum.UserIsNull).ConfigureAwait(false);

            var syncFeaturesStatus = await _baseService.SyncAsync(
                updatedApplicant.ApplicantFeatures,
                model.ApplicantFeatures,
                (feature, currentUser) => new ApplicantFeature
                {
                    ApplicantId = updatedApplicant.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                null,
                false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedApplicant, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ModelIsNull, null);

            var contact = await ContactEntityByMobileAsync(model.Mobile).ConfigureAwait(false);
            if (contact == null)
            {
                var (contactAddStatus, newContact) = await ContactAddAsync(new ContactInputViewModel
                {
                    Mobile = model.Mobile
                }, true).ConfigureAwait(false);
                if (contactAddStatus == StatusEnum.Success)
                    contact = newContact;
                else
                    return new ValueTuple<StatusEnum, Applicant>(contactAddStatus, null);
            }

            var (addStatus, newApplicant) = await _baseService.AddAsync(currentUser => new Applicant
            {
                ContactId = contact.Id,
                UserId = currentUser.Id,
                Type = model.Type,
                Address = model.Address,
                Description = model.Description,
                Name = model.Name,
                PhoneNumber = model.Phone
            },
                null,
                false).ConfigureAwait(false);

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
                Mobile = model.Mobile,
            }, true).ConfigureAwait(false);
            if (contactAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Ownership>(contactAddStatus, null);

            var addStatus = await _baseService.AddAsync(new Ownership
            {
                ContactId = newContact.Id,
                Dong = model.Dong,
                Description = model.Description,
                Address = model.Address,
                Name = model.Name,
                PhoneNumber = model.Phone,
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, Contact)> ContactAddAsync(ContactInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Contact>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(new Contact
            {
                MobileNumber = model.Mobile,
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public Task<(StatusEnum, Applicant)> ApplicantAddOrUpdateAsync(ApplicantInputViewModel model, bool update, bool save)
        {
            return update
                ? ApplicantUpdateAsync(model, save)
                : ApplicantAddAsync(model, save);
        }

        public Task<(StatusEnum, Contact)> ContactAddOrUpdateAsync(ContactInputViewModel model, bool update, bool save)
        {
            return update
                ? ContactUpdateAsync(model, save)
                : ContactAddAsync(model, save);
        }

        public async Task<(StatusEnum, Contact)> ContactUpdateAsync(ContactInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Contact>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Contact>(StatusEnum.IdIsNull, null);

            var entity = await ContactEntityAsync(model.Id).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                () =>
                {
                    entity.MobileNumber = model.Mobile;
                }, null, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }
    }
}