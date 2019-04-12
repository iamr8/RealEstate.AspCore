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
    public interface IItemService
    {
        Task<(StatusEnum, ItemRequest)> ItemRequestAddAsync(ItemRequestInputViewModel model, bool save);

        Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel);

        Task<StatusEnum> ItemRemoveAsync(string id);

        Task<(StatusEnum, Item)> ItemAddOrUpdateAsync(ItemInputViewModel model, bool update, bool save);

        Task<Item> ItemEntityAsync(string id);

        Task<ItemInputViewModel> ItemInputAsync(string id);

        Task<(StatusEnum, ItemRequest)> ItemRequestRejectAsync(string itemRequestId, bool save);

        Task<ItemRequest> ItemRequestEntityAsync(string id);

        Task<(StatusEnum, Item)> ItemAddAsync(ItemInputViewModel model, bool save);
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IContactService _contactService;
        private readonly DbSet<ItemRequest> _itemRequests;
        private readonly DbSet<Item> _items;

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
            _items = _unitOfWork.Set<Item>();
        }

        public async Task<ItemRequest> ItemRequestEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _itemRequests.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel)
        {
            var models = _items as IQueryable<Item>;
            models.Include(x => x.ItemRequests);

            if (searchModel != null)
            {
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => new ItemViewModel(item, _baseService.IsAllowed(Role.SuperAdmin, Role.Admin)).Instance).ConfigureAwait(false);

            return result;
        }

        public async Task<ItemInputViewModel> ItemInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await ItemEntityAsync(id).ConfigureAwait(false);
            if (entity == null)
                return default;

            var viewModel = new ItemViewModel(entity, false).Instance?
                .R8Include(model =>
                {
                    model.Category = new CategoryViewModel(model.Entity?.Category, false).Instance;
                    model.Features = model.Entity?.ItemFeatures.Select(propEntity =>
                        new ItemFeatureViewModel(propEntity, false).Instance?
                            .R8Include(x => x.Feature = new FeatureViewModel(x.Entity?.Feature, false).Instance)).R8ToList();
                    model.Property = new PropertyViewModel(model.Entity?.Property, false).Instance;
                });
            if (viewModel == null)
                return default;

            var result = new ItemInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                CategoryId = viewModel.Category?.Id,
                ItemFeatures = viewModel.Features?.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature?.Id,
                    Name = x.Feature?.Name,
                    Value = x.Value
                }).ToList(),
                PropertyId = viewModel.Property?.Id
            };
            return result;
        }

        public async Task<Item> ItemEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _items.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<StatusEnum> ItemRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var property = await ItemEntityAsync(id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(property,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<(StatusEnum, ItemRequest)> ItemRequestRejectAsync(string itemRequestId, bool save)
        {
            if (string.IsNullOrEmpty(itemRequestId))
                return new ValueTuple<StatusEnum, ItemRequest>(StatusEnum.ParamIsNull, null);

            var itemRequest = await ItemRequestEntityAsync(itemRequestId).ConfigureAwait(false);
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

        public Task<(StatusEnum, Item)> ItemAddOrUpdateAsync(ItemInputViewModel model, bool update, bool save)
        {
            return update
                ? ItemUpdateAsync(model, save)
                : ItemAddAsync(model, save);
        }

        private async Task<(StatusEnum, Item)> ItemUpdateAsync(ItemInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.IdIsNull, null);

            var entity = await ItemEntityAsync(model.Id).ConfigureAwait(false);
            var (updateStatus, updatedItem) = await _baseService.UpdateAsync(entity,
                () =>
                {
                    entity.CategoryId = model.CategoryId;
                    entity.Description = model.Description;
                    entity.PropertyId = model.PropertyId;
                }, null, false, StatusEnum.PropertyIsNull).ConfigureAwait(false);

            if (updatedItem == null)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ItemIsNull, null);

            await ItemSyncAsync(updatedItem, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedItem, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Item)> ItemAddAsync(ItemInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ModelIsNull, null);

            var (itemAddStatus, newItem) = await _baseService.AddAsync(new Item
            {
                CategoryId = model.CategoryId,
                Description = model.Description,
                PropertyId = model.PropertyId,
            }, null, false).ConfigureAwait(false);

            await ItemSyncAsync(newItem, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newItem, save).ConfigureAwait(false);
        }

        private async Task<StatusEnum> ItemSyncAsync(Item newItem, ItemInputViewModel model, bool save)
        {
            var syncFeatures = await _baseService.SyncAsync(
                newItem.ItemFeatures,
                model.ItemFeatures,
                feature => new ItemFeature
                {
                    FeatureId = feature.Id,
                    Value = feature.Value,
                    ItemId = newItem.Id
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                null,
                save).ConfigureAwait(false);
            return syncFeatures;
        }
    }
}