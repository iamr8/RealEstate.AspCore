using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IContactService
    {
        Task<(StatusEnum, Contact)> ContactAddAsync(ContactInputViewModel model, bool isForApplicantUse, bool save);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantAddOrUpdateAsync(ApplicantInputViewModel model, bool update, bool save);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantUpdateAsync(ApplicantInputViewModel model, bool save);

        Task<OwnershipJsonViewModel> OwnershipJsonAsync(string id);

        Task<(StatusEnum, Database.Tables.Contact)> ContactAddOrUpdateAsync(ContactInputViewModel model, bool update, bool save);

        Task<(StatusEnum, Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, string dealId, bool save);

        Task<(StatusEnum, Ownership)> OwnershipAddAsync(Ownership model, bool save);

        Task<PaginationViewModel<ContactViewModel>> ContactListAsync(ContactSearchViewModel searchModel);

        Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save);

        Task<StatusEnum> ContactPrivacyCheckAsync(string mobile);

        Task<(StatusEnum, Database.Tables.Contact)> ContactUpdateAsync(ContactInputViewModel model, bool save);

        Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(ApplicantSearchViewModel searchModel);

        Task<ApplicantInputViewModel> ApplicantInputAsync(string id);

        Task<List<ApplicantJsonViewModel>> ListJsonAsync();

        Task<List<ContactViewModel>> ContactListAsync();

        Task<Applicant> ApplicantEntityAsync(string id, bool includeContact = true, bool includeApplicantFeatures = true, bool includeItemRequest = true);

        Task<StatusEnum> ApplicantRemoveAsync(string id);

        Task<(StatusEnum, Ownership)> OwnershipPlugPropertyAsync(string ownerId, string propertyOwnershipId, bool save);

        Task<Contact> ContactEntityAsync(string id, string mobile, bool includeApplicants = true, bool includeOwnerships = true, bool includeSmses = true);

        Task<(StatusEnum, Database.Tables.Applicant)> ApplicantPlugItemRequestAsync(string applicantId, string dealId, bool save);

        Task<StatusEnum> ContactRemoveAsync(string id);

        Task<Ownership> OwnershipEntityAsync(string id);
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

        public async Task<List<ApplicantJsonViewModel>> ListJsonAsync()
        {
            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return default;

            var applicantsQuery = await _applicants.Where(x => x.UserId == currentUser.Id && x.DealId == null).ToListAsync().ConfigureAwait(false);
            var contactsQuery = await _contacts.WhereItIsPublic().ToListAsync().ConfigureAwait(false);

            var result = new List<ApplicantJsonViewModel>();
            if (applicantsQuery?.Any() == true)
                foreach (var applicant in applicantsQuery)
                {
                    var item = new ApplicantJsonViewModel
                    {
                        Name = applicant.Contact.Name,
                        Mobile = applicant.Contact.MobileNumber,
                        ApplicantId = applicant.Id,
                        ContactId = applicant.ContactId
                    };
                    result.Add(item);
                }

            if (contactsQuery?.Any() == true)
                foreach (var contact in contactsQuery)
                {
                    var duplicate = result.Find(x => x.ContactId == contact.Id);
                    if (duplicate != null)
                        continue;

                    var item = new ApplicantJsonViewModel
                    {
                        Name = contact.Name,
                        Mobile = contact.MobileNumber,
                        ContactId = contact.Id,
                    };
                    result.Add(item);
                }

            return result;
        }

        public async Task<Database.Tables.Contact> ContactEntityAsync(string id, string mobile, bool includeApplicants = true, bool includeOwnerships = true, bool includeSmses = true)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var query = _contacts as IQueryable<Contact>;

            if (!string.IsNullOrEmpty(id))
                query = query.Where(x => x.Id == id);

            if (!string.IsNullOrEmpty(mobile))
                query = query.Where(x => x.MobileNumber == mobile);

            if (includeApplicants)
                query = query.Include(x => x.Applicants);

            if (includeOwnerships)
                query = query.Include(x => x.Ownerships);

            if (includeSmses)
                query = query.Include(x => x.Smses);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<List<ContactViewModel>> ContactListAsync()
        {
            var query = _contacts as IQueryable<Contact>;
            query = query.Filtered();
            query = query.Include(x => x.Ownerships)
                .Include(x => x.Applicants);
            query = query.WhereItIsPublic();

            var contacts = await query.ToListAsync().ConfigureAwait(false);
            return contacts.Into<Contact, ContactViewModel>();
        }

        public async Task<Applicant> ApplicantEntityAsync(string id, bool includeContact = true, bool includeApplicantFeatures = true, bool includeItemRequest = true)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var query = _applicants as IQueryable<Applicant>;

            if (includeContact)
                query = query.Include(x => x.Contact);

            if (includeApplicantFeatures)
                query.Include(x => x.ApplicantFeatures);

            if (includeItemRequest)
                query.Include(x => x.Deal);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<Ownership> OwnershipEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _ownerships.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<OwnershipJsonViewModel> OwnershipJsonAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var entity = await OwnershipEntityAsync(id).ConfigureAwait(false);
            if (entity == null) return null;

            var viewModel = entity.Into<Ownership, OwnershipViewModel>(false, act => act.GetContact());
            if (viewModel == null)
                return default;

            return new OwnershipJsonViewModel
            {
                ContactId = viewModel.Contact.Id,
                Name = viewModel.Contact.Name,
                Dong = viewModel.Dong,
                Mobile = viewModel.Contact.Mobile
            };
        }

        public async Task<StatusEnum> ContactRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await ContactEntityAsync(id, null).ConfigureAwait(false);
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
            var mobile = user.Contact.MobileNumber;
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);
            if (result != StatusEnum.Success)
                return result;

            var contact = await ContactPrivacyCheckAsync(mobile).ConfigureAwait(false);
            return contact;
        }

        public async Task<StatusEnum> ContactPrivacyCheckAsync(string mobile)
        {
            var contact = await ContactEntityAsync(null, mobile, true, false, false).ConfigureAwait(false);
            if (contact == null)
                return StatusEnum.ContactIsNull;

            if (contact.Applicants.Count > 0 && contact.Ownerships.Count == 0 && contact.Applicants.All(c => c.Deal == null))
                return StatusEnum.Success;

            var (updateStatus, updatedContact) = await _baseService.UpdateAsync(contact,
                () => contact.IsPrivate = false,
                null, true, StatusEnum.ContactIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<(StatusEnum, Database.Tables.Applicant)> ApplicantPlugItemRequestAsync(string applicantId, string dealId, bool save)
        {
            if (string.IsNullOrEmpty(applicantId) || string.IsNullOrEmpty(dealId))
                return new ValueTuple<StatusEnum, Database.Tables.Applicant>(StatusEnum.ParamIsNull, null);

            var applicant = await ApplicantEntityAsync(applicantId).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(applicant,
                () => applicant.DealId = dealId,
                null, save, StatusEnum.ApplicantIsNull
            ).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<ApplicantInputViewModel> ApplicantInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await ApplicantEntityAsync(id).ConfigureAwait(false);
            if (entity == null)
                return default;

            var viewModel = entity.Into<Applicant, ApplicantViewModel>(false, act =>
            {
                act.GetContact();
                act.GetApplicantFeatures(false, act2 =>
                {
                    act2.GetFeature();
                });
            });
            if (viewModel == null)
                return default;

            var result = new ApplicantInputViewModel
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Name = viewModel.Contact.Name,
                Description = viewModel.Description,
                Address = viewModel.Contact.Address,
                Mobile = viewModel.Contact.Mobile,
                Phone = viewModel.Contact.Phone,
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
            models = models.Include(x => x.ApplicantFeatures);
            models = models.Include(x => x.Contact);

            if (searchModel != null)
            {
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Applicant, ApplicantViewModel>(false, act =>
                {
                    act.GetContact();
                    act.GetApplicantFeatures(false, act2 =>
                    {
                        act2.GetFeature();
                    });
                })).ConfigureAwait(false);

            return result;
        }

        public async Task<PaginationViewModel<ContactViewModel>> ContactListAsync(ContactSearchViewModel searchModel)
        {
            var models = _contacts as IQueryable<Contact>;
            models = models.Include(x => x.Applicants);
            models = models.Include(x => x.Ownerships);
            models = models.WhereItIsPublic();

            if (searchModel != null)
            {
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Contact, ContactViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetApplicants(false, act2 => act2.GetContact());
                    act.GetOwnerships(false, act2 => act2.GetContact());
                })
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<(StatusEnum, Ownership)> OwnershipPlugPropertyAsync(string ownerId, string propertyOwnershipId, bool save)
        {
            if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(propertyOwnershipId))
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.ParamIsNull, null);

            var ownership = await OwnershipEntityAsync(ownerId).ConfigureAwait(false);
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

            var entity = await ApplicantEntityAsync(model.Id, true, false, true).ConfigureAwait(false);
            if (entity?.Contact?.IsPrivate != true || entity.Deal != null)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ApplicantIsNull, null);

            var (updateStatus, updatedApplicant) = await _baseService.UpdateAsync(entity,
                () =>
                {
                    entity.Description = model.Description;
                    entity.Type = model.Type;
                }, null, false, StatusEnum.UserIsNull).ConfigureAwait(false);

            var contact = updatedApplicant.Contact;
            var (updateContactStatus, updatedContact) = await _baseService.UpdateAsync(contact,
                () =>
                {
                    contact.Address = model.Address;
                    contact.MobileNumber = model.Mobile;
                    contact.Name = model.Name;
                    contact.PhoneNumber = model.Phone;
                }, null, false, StatusEnum.ContactIsNull).ConfigureAwait(false);

            var syncFeaturesStatus = await _baseService.SyncAsync(
                            updatedApplicant.ApplicantFeatures,
                            model.ApplicantFeatures,
                            feature => new ApplicantFeature
                            {
                                ApplicantId = updatedApplicant.Id,
                                FeatureId = feature.Id,
                                Value = feature.Value,
                            }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                            null,
                            false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedApplicant, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, string dealId, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ModelIsNull, null);

            var contact = await ContactEntityAsync(null, model.Mobile).ConfigureAwait(false);
            if (contact == null)
            {
                var (contactAddStatus, newContact) = await ContactAddAsync(new ContactInputViewModel
                {
                    Mobile = model.Mobile,
                    Address = model.Address,
                    Name = model.Name,
                    Phone = model.Phone,
                    Id = model.Id
                }, true, true).ConfigureAwait(false);
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
                Description = model.Description,
                DealId = !string.IsNullOrEmpty(dealId) ? dealId : null
            },
                null,
                false).ConfigureAwait(false);

            var syncFeaturesStatus = await _baseService.SyncAsync(
                newApplicant.ApplicantFeatures,
                model.ApplicantFeatures,
                feature => new ApplicantFeature
                {
                    ApplicantId = newApplicant.Id,
                    FeatureId = feature.Id,
                    Value = feature.Value,
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                null,
                false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newApplicant, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Ownership)> OwnershipAddAsync(Ownership model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.ModelIsNull, null);

            var addStatus = await _baseService.AddAsync(model, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.ModelIsNull, null);

            var existingContact = await ContactEntityAsync(null, model.Mobile).ConfigureAwait(false);
            if (existingContact == null)
            {
                var (contactAddStatus, newContact) = await ContactAddAsync(new ContactInputViewModel
                {
                    Mobile = model.Mobile,
                    Address = model.Address,
                    Name = model.Name,
                    Phone = model.Phone,
                }, false, true).ConfigureAwait(false);
                if (contactAddStatus != StatusEnum.Success)
                    return new ValueTuple<StatusEnum, Ownership>(contactAddStatus, null);

                existingContact = newContact;
            }

            var addStatus = await _baseService.AddAsync(new Ownership
            {
                ContactId = existingContact.Id,
                Dong = model.Dong,
                Description = model.Description,
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, Contact)> ContactAddAsync(ContactInputViewModel model, bool isForApplicantUse, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Contact>(StatusEnum.ModelIsNull, null);

            var existing = await ContactEntityAsync(null, model.Mobile).ConfigureAwait(false);
            if (existing != null)
            {
                if (!existing.IsPrivate)
                    return new ValueTuple<StatusEnum, Contact>(StatusEnum.AlreadyExists, existing);

                if (isForApplicantUse)
                    return new ValueTuple<StatusEnum, Contact>(StatusEnum.Success, existing);

                var updateStatus = await _baseService.UpdateAsync(existing,
                    () => existing.IsPrivate = false,
                    null, save, StatusEnum.ContactIsNull).ConfigureAwait(false);
                return updateStatus;
            }

            var addStatus = await _baseService.AddAsync(new Contact
            {
                MobileNumber = model.Mobile,
                Name = model.Name,
                Address = model.Address,
                PhoneNumber = model.Phone,
                IsPrivate = isForApplicantUse
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public Task<(StatusEnum, Applicant)> ApplicantAddOrUpdateAsync(ApplicantInputViewModel model, bool update, bool save)
        {
            return update
                ? ApplicantUpdateAsync(model, save)
                : ApplicantAddAsync(model, null, save);
        }

        public Task<(StatusEnum, Contact)> ContactAddOrUpdateAsync(ContactInputViewModel model, bool update, bool save)
        {
            return update
                ? ContactUpdateAsync(model, save)
                : ContactAddAsync(model, false, save);
        }

        public async Task<(StatusEnum, Contact)> ContactUpdateAsync(ContactInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Contact>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Contact>(StatusEnum.IdIsNull, null);

            var entity = await ContactEntityAsync(model.Id, null).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                () => entity.MobileNumber = model.Mobile,
                null, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }
    }
}