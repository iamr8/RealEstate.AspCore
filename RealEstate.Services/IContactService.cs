using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Services.Base;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Input;
using System;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IContactService
    {
        Task<(StatusEnum, Contact)> ContactAddAsync(ContactInputViewModel model, bool save, UserViewModel currentUser);

        Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save, UserViewModel currentUser);

        Task<(StatusEnum, Ownership)> OwnershipUpdateAsync(string ownerId, string ownershipId, bool save, UserViewModel currentUser);

        Task<(StatusEnum, Ownership)> OwnershipUpdateAsync(Ownership owner, string ownershipId, bool save, UserViewModel currentUser);

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

        public async Task<Ownership> OwnershipFindEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _ownerships.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<(StatusEnum, Ownership)> OwnershipUpdateAsync(Ownership owner, string ownershipId, bool save, UserViewModel currentUser)
        {
            if (owner == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.OwnershipIsNull, null);

            currentUser = _baseService.CurrentUser(currentUser);
            if (currentUser == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.UserIsNull, null);

            owner.PropertyOwnershipId = ownershipId;
            _unitOfWork.Update(owner, currentUser.Id);

            return await _baseService.SaveChangesAsync(owner, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Ownership)> OwnershipUpdateAsync(string ownerId, string ownershipId, bool save, UserViewModel currentUser)
        {
            var owner = await OwnershipFindEntityAsync(ownerId).ConfigureAwait(false);
            return await OwnershipUpdateAsync(owner, ownershipId, save, currentUser).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Ownership)> OwnershipAddAsync(OwnershipInputViewModel model, bool save, UserViewModel currentUser)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.ModelIsNull, null);

            currentUser = _baseService.CurrentUser(currentUser);
            if (currentUser == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.UserIsNull, null);

            var (contactAddStatus, contact) = await ContactAddAsync(new ContactInputViewModel
            {
                Address = model.Address,
                Description = model.Description,
                Mobile = model.Mobile,
                Name = model.Name,
                Phone = model.Phone
            }, save, currentUser).ConfigureAwait(false);
            if (contactAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Ownership>(contactAddStatus, null);

            var newOwner = _unitOfWork.Add(new Ownership
            {
                ContactId = contact.Id,
                Dong = model.Dong,
            }, currentUser.Id);
            if (newOwner == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.OwnershipIsNull, null);

            return await _baseService.SaveChangesAsync(newOwner, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Contact)> ContactAddAsync(ContactInputViewModel model, bool save, UserViewModel currentUser)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Contact>(StatusEnum.ModelIsNull, null);

            currentUser = _baseService.CurrentUser(currentUser);
            if (currentUser == null)
                return new ValueTuple<StatusEnum, Contact>(StatusEnum.UserIsNull, null);

            var newContact = _unitOfWork.Add(new Contact
            {
                Address = model.Address,
                Description = model.Description,
                MobileNumber = model.Mobile,
                Name = model.Name,
                PhoneNumber = model.Phone
            }, currentUser.Id);

            return await _baseService.SaveChangesAsync(newContact, save).ConfigureAwait(false);
        }
    }
}