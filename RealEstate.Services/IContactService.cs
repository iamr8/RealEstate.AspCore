using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Extensions;
using RealEstate.Services.Base;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Input;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IContactService
    {
        Task<(StatusEnum, Contact)> ContactAddAsync(ContactInputViewModel model, bool save);

        Task<(StatusEnum, Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, bool save);

        Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save);

        Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(int page);

        Task<ApplicantInputViewModel> ApplicantInputAsync(string id);

        Task<Applicant> ApplicantEntityAsync(string id);

        Task<StatusEnum> ApplicantRemoveAsync(string id);

        Task<(StatusEnum, Ownership)> OwnershipPlugPropertyAsync(string ownerId, string propertyOwnershipId, bool save);

        Task<(StatusEnum, Applicant)> ApplicantPlugItemRequestAsync(string applicantId, string itemRequestId, bool save);

        Task<Ownership> OwnershipFindEntityAsync(string id);
    }

    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IMapService _mapService;
        private readonly DbSet<Contact> _contacts;
        private readonly DbSet<Applicant> _applicants;
        private readonly DbSet<Ownership> _ownerships;

        public ContactService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IMapService mapService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _mapService = mapService;
            _contacts = _unitOfWork.Set<Contact>();
            _applicants = _unitOfWork.Set<Applicant>();
            _ownerships = _unitOfWork.Set<Ownership>();
        }

        public async Task<Applicant> ApplicantEntityAsync(string id)
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

        public async Task<(StatusEnum, Applicant)> ApplicantPlugItemRequestAsync(string applicantId, string itemRequestId, bool save)
        {
            if (string.IsNullOrEmpty(applicantId) || string.IsNullOrEmpty(itemRequestId))
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ParamIsNull, null);

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

            var query = _applicants.Where(x => x.Id == id);
            var model = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            var viewModel = _mapService.Map(model);
            if (viewModel == null)
                return default;

            var result = new ApplicantInputViewModel
            {
                Id = viewModel.Id,
                Type = viewModel.Type,
                Name = viewModel.Name,
                Description = viewModel.Description,
                Address = viewModel.Address,
                Mobile = viewModel.Contact.Mobile,
                Phone = viewModel.Phone,
                ApplicantFeaturesJson = viewModel.Features.JsonConversion(),
            };
            return result;
        }

        public async Task<PaginationViewModel<ApplicantViewModel>> ApplicantListAsync(int page)
        {
            var models = _applicants as IQueryable<Applicant>;
            models = models.Include(x => x.ApplicantFeatures);
            models = models.Include(x => x.Logs);

            var result = await _baseService.PaginateAsync(models, page, _mapService.Map,
                new[]
                {
                    Role.Admin, Role.SuperAdmin
                }
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

        public async Task<(StatusEnum, Applicant)> ApplicantAddAsync(ApplicantInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Applicant>(StatusEnum.ModelIsNull, null);

            var (contactAddStatus, newContact) = await ContactAddAsync(new ContactInputViewModel
            {
                Mobile = model.Mobile
            }, false).ConfigureAwait(false);
            if (contactAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Applicant>(contactAddStatus, null);

            var (addStatus, newApplicant) = await _baseService.AddAsync(currentUser => new Applicant
            {
                ContactId = newContact.Id,
                UserId = currentUser.Id,
                Type = model.Type,
                Address = model.Address,
                Description = model.Description,
                Name = model.Name,
                PhoneNumber = model.Phone
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
    }
}