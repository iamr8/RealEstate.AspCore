using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Services.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Services.ViewModels.Input;

namespace RealEstate.Services
{
    public interface IItemService
    {
        Task<(StatusEnum, ItemRequest)> ItemRequestAddAsync(ItemRequestInputViewModel model, bool save);

        Task<(StatusEnum, ItemRequest)> ItemRequestRejectAsync(string itemRequestId, bool save);

        Task<ItemRequest> ItemRequestFindEntityAsync(string id);

        Task<(StatusEnum, Item)> ItemAddAsync(ItemInputViewModel model, bool save);
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IContactService _contactService;
        private readonly DbSet<ItemRequest> _itemRequests;

        public ItemService(
            IBaseService baseService,
            IUnitOfWork unitOfWork,
            IContactService contactService
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
            _contactService = contactService;
            _itemRequests = _unitOfWork.Set<ItemRequest>();
        }

        public async Task<ItemRequest> ItemRequestFindEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _itemRequests.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<(StatusEnum, ItemRequest)> ItemRequestRejectAsync(string itemRequestId, bool save)
        {
            if (string.IsNullOrEmpty(itemRequestId))
                return new ValueTuple<StatusEnum, ItemRequest>(StatusEnum.ParamIsNull, null);

            var itemRequest = await ItemRequestFindEntityAsync(itemRequestId).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(itemRequest,
                () => itemRequest.IsReject = true,
                new[]
                {
                    Role.Admin, Role.SuperAdmin
                }, save, StatusEnum.ItemRequestIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<(StatusEnum, ItemRequest)> ItemRequestAddAsync(ItemRequestInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, ItemRequest>(StatusEnum.ModelIsNull, null);

            if (model.Applicants?.Any() != true)
                return new ValueTuple<StatusEnum, ItemRequest>(StatusEnum.ApplicantIsNull, null);

            var (itemAddStatus, newItemRequest) = await _baseService.AddAsync(new ItemRequest
            {
                ItemId = model.ItemId,
                IsReject = false,
                Description = model.Description,
            }, null, false);

            foreach (var applicant in model.Applicants)
                await _contactService.ApplicantPlugItemRequestAsync(applicant.Id, newItemRequest.Id, false).ConfigureAwait(false);

            return await _baseService.SaveChangesAsync(newItemRequest, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Item)> ItemAddAsync(ItemInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ModelIsNull, null);

            var (itemAddStatus, newItem) = await _baseService.AddAsync(new Item
            {
                CategoryId = model.CategoryId,
                Description = model.Description,
                PropertyId = model.PropertyId
            }, null, false).ConfigureAwait(false);

            var syncFeatures = await _baseService.SyncAsync(
                newItem.ItemFeatures,
                model.ItemFeatures,
                (feature, currentUser) => new ItemFeature
                {
                    FeatureId = feature.Id,
                    Value = feature.Value,
                    ItemId = newItem.Id
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id, null, false);

            return await _baseService.SaveChangesAsync(newItem, save).ConfigureAwait(false);
        }
    }
}