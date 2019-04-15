using EFSecondLevelCache.Core;
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
        Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel);

        Task<StatusEnum> ItemRemoveAsync(string id);

        Task<(StatusEnum, Item)> ItemAddOrUpdateAsync(ItemInputViewModel model, bool update, bool save);

        Task<Item> ItemEntityAsync(string id);

        Task<ItemInputViewModel> ItemInputAsync(string id);

        Task<ItemViewModel> ItemAsync(string id);

        Task<(StatusEnum, Item)> ItemAddAsync(ItemInputViewModel model, bool save);
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IContactService _contactService;
        private readonly IMapService _mapService;
        private readonly IFeatureService _featureService;
        private readonly IPropertyService _propertyService;
        private readonly DbSet<Deal> _itemRequests;
        private readonly DbSet<Item> _items;
        private readonly DbSet<Ownership> _ownerships;
        private readonly DbSet<Contact> _contacts;

        public ItemService(
            IBaseService baseService,
            IUnitOfWork unitOfWork,
            IMapService mapService,
            IFeatureService featureService,
            IPropertyService propertyService,
            IContactService contactService
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
            _mapService = mapService;
            _featureService = featureService;
            _contactService = contactService;
            _propertyService = propertyService;
            _itemRequests = _unitOfWork.Set<Deal>();
            _items = _unitOfWork.Set<Item>();
            _ownerships = _unitOfWork.Set<Ownership>();
            _contacts = _unitOfWork.Set<Contact>();
        }

        public async Task<Deal> ItemRequestAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _itemRequests.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel)
        {
            var query = _items.AsQueryable().MarkAsNoTracking();
            query = query.Include(item => item.Deals);
            query = query.Include(item => item.Category);
            query = query.Include(item => item.ItemFeatures)
                .ThenInclude(itemFeature => itemFeature.Feature);

            query = query.Include(item => item.Property.PropertyOwnerships)
                .ThenInclude(propertyOwnership => propertyOwnership.Ownerships)
                .ThenInclude(ownership => ownership.Contact);
            query = query.Include(item => item.Property.Category);
            query = query.Include(item => item.Property.District);

            query = query.Where(x => x.Deals.Count == 0
                                                 || x.Deals.OrderByDescending(c => c.DateTime).FirstOrDefault().Status != DealStatusEnum.Finished);

            query = query.Where(x => x.Deals.Count == 0
                                 || x.Deals.OrderByDescending(c => c.DateTime).FirstOrDefault().Status != DealStatusEnum.Finished);

            if (searchModel != null)
            {
            }

            var result = await _baseService.PaginateAsync(query, searchModel?.PageNo ?? 1,
                item => _mapService.Map(item, _baseService.IsAllowed(Role.SuperAdmin, Role.Admin))).ConfigureAwait(false);
            return result;
        }

        public async Task<ItemInputViewModel> ItemInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var models = _items as IQueryable<Item>;
            models = models.Include(x => x.Deals);

            models = models.Include(x => x.ItemFeatures)
                .ThenInclude(x => x.Feature);

            models = models.Include(x => x.Category);

            models = models.Include(x => x.Property)
                .ThenInclude(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Contact);

            var entity = await models.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (entity == null)
                return default;

            var viewModel = _mapService.Map(entity, _baseService.IsAllowed(Role.SuperAdmin, Role.Admin));
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

        public async Task<ItemViewModel> ItemAsync(string id)
        {
            var query = _items as IQueryable<Item>;
            query = query.Include(x => x.Deals);

            query = query.Include(x => x.ItemFeatures)
                .ThenInclude(x => x.Feature);

            query = query.Include(x => x.Category);

            query = query.Include(x => x.Property)
                .ThenInclude(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Contact);

            query = query.Where(x => x.Deals.Count == 0
                                     || x.Deals.OrderByDescending(c => c.DateTime).FirstOrDefault().Status != DealStatusEnum.Finished);

            var model = await query.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (model == null)
                return default;

            var viewModel = _mapService.Map(model, _baseService.IsAllowed(Role.SuperAdmin, Role.Admin));
            if (viewModel == null)
                return default;

            return viewModel;
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