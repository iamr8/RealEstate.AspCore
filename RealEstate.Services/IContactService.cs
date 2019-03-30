using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Services.Connector;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IContactService
    {
        Task<(StatusEnum, Contact)> ContactAddAsync(ContactInputViewModel model, bool save, UserViewModel currentUser);

        Task<(StatusEnum, Owner)> OwnerAddAsync(OwnerInputViewModel model, bool save, UserViewModel currentUser);

        Task<(StatusEnum, Owner)> OwnerUpdateAsync(string ownerId, string ownershipId, bool save, UserViewModel currentUser);

        Task<(StatusEnum, Ownership)> OwnershipUpdateAsync(string ownershipId, string propertyId, bool save, UserViewModel currentUser);

        Task<(StatusEnum, Owner)> OwnerUpdateAsync(Owner owner, string ownershipId, bool save, UserViewModel currentUser);

        Task<(StatusEnum, Ownership)> OwnershipUpdateAsync(Ownership ownership, string propertyId, bool save, UserViewModel currentUser);

        Task<(StatusEnum, Ownership)> OwnershipAddAsync(string propertyId, bool save, UserViewModel currentUser);

        OwnerViewModel OwnerFind(Owner model);

        OwnershipViewModel OwnershipFind(Ownership model);

        Task<List<OwnerViewModel>> OwnershipFindAsync(string id);

        Task<Ownership> OwnershipFindEntityAsync(string id);

        Task<Owner> OwnerFindEntityAsync(string id);

        ContactViewModel ContactFind(Contact model);
    }

    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<Contact> _contacts;
        private readonly DbSet<Applicant> _applicants;
        private readonly DbSet<Owner> _owners;
        private readonly DbSet<Ownership> _ownerships;

        public ContactService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _contacts = _unitOfWork.PlugIn<Contact>();
            _applicants = _unitOfWork.PlugIn<Applicant>();
            _owners = _unitOfWork.PlugIn<Owner>();
            _ownerships = _unitOfWork.PlugIn<Ownership>();
        }

        public async Task<Owner> OwnerFindEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _owners.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<Ownership> OwnershipFindEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _ownerships.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<List<OwnerViewModel>> OwnershipFindAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await OwnershipFindEntityAsync(id).ConfigureAwait(false);
            if (entity == null)
                return default;

            return entity.Owners?.Select(OwnerFind).ToList();
        }

        public OwnershipViewModel OwnershipFind(Ownership model)
        {
            var owners = model?.Owners.ToList();
            if (model?.Id == null || owners?.Any() != true)
                return default;

            var finalVM = new OwnershipViewModel
            {
                Id = model.Id,
                PropertyId = model.PropertyId,
                Owners = model.Owners.Select(OwnerFind).ToList()
            };
            return finalVM;
        }

        public OwnerViewModel OwnerFind(Owner model)
        {
            if (model == null)
                return default;

            var result = new OwnerViewModel
            {
                Contact = ContactFind(model.Contact),
                Dong = model.Dong,
                Id = model.Id,
                OwnershipId = model.OwnershipId,
            };

            return result;
        }

        public async Task<(StatusEnum, Ownership)> OwnershipAddAsync(string propertyId, bool save, UserViewModel currentUser)
        {
            currentUser = _baseService.CurrentUser(currentUser);
            if (currentUser == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.UserIsNull, null);

            var newOwnership = _unitOfWork.Add(new Ownership
            {
                PropertyId = propertyId
            }, currentUser.Id);
            if (newOwnership == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.OwnershipIsNull, null);

            return await _baseService.SaveChangesAsync(newOwnership, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Ownership)> OwnershipUpdateAsync(Ownership ownership, string propertyId, bool save, UserViewModel currentUser)
        {
            if (ownership == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.OwnershipIsNull, null);

            currentUser = _baseService.CurrentUser(currentUser);
            if (currentUser == null)
                return new ValueTuple<StatusEnum, Ownership>(StatusEnum.UserIsNull, null);

            ownership.PropertyId = propertyId;
            _unitOfWork.Update(ownership, currentUser.Id);

            return await _baseService.SaveChangesAsync(ownership, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Owner)> OwnerUpdateAsync(Owner owner, string ownershipId, bool save, UserViewModel currentUser)
        {
            if (owner == null)
                return new ValueTuple<StatusEnum, Owner>(StatusEnum.OwnerIsNull, null);

            currentUser = _baseService.CurrentUser(currentUser);
            if (currentUser == null)
                return new ValueTuple<StatusEnum, Owner>(StatusEnum.UserIsNull, null);

            owner.OwnershipId = ownershipId;
            _unitOfWork.Update(owner, currentUser.Id);

            return await _baseService.SaveChangesAsync(owner, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Ownership)> OwnershipUpdateAsync(string ownershipId, string propertyId, bool save, UserViewModel currentUser)
        {
            var ownership = await OwnershipFindEntityAsync(ownershipId).ConfigureAwait(false);
            return await OwnershipUpdateAsync(ownership, propertyId, save, currentUser).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Owner)> OwnerUpdateAsync(string ownerId, string ownershipId, bool save, UserViewModel currentUser)
        {
            var owner = await OwnerFindEntityAsync(ownerId).ConfigureAwait(false);
            return await OwnerUpdateAsync(owner, ownershipId, save, currentUser).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Owner)> OwnerAddAsync(OwnerInputViewModel model, bool save, UserViewModel currentUser)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Owner>(StatusEnum.ModelIsNull, null);

            currentUser = _baseService.CurrentUser(currentUser);
            if (currentUser == null)
                return new ValueTuple<StatusEnum, Owner>(StatusEnum.UserIsNull, null);

            var (contactAddStatus, contact) = await ContactAddAsync(new ContactInputViewModel
            {
                Address = model.Address,
                Description = model.Description,
                Mobile = model.Mobile,
                Name = model.Name,
                Phone = model.Phone
            }, save, currentUser).ConfigureAwait(false);
            if (contactAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Owner>(contactAddStatus, null);

            var newOwner = _unitOfWork.Add(new Owner
            {
                ContactId = contact.Id,
                Dong = model.Dong,
            }, currentUser.Id);
            if (newOwner == null)
                return new ValueTuple<StatusEnum, Owner>(StatusEnum.OwnerIsNull, null);

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

        public ContactViewModel ContactFind(Contact model)
        {
            if (model == null)
                return default;

            var result = new ContactViewModel
            {
                Id = model.Id,
                Description = model.Description,
                Address = model.Address,
                Mobile = model.MobileNumber,
                Name = model.Name,
                Phone = model.PhoneNumber
            };
            return result;
        }
    }
}