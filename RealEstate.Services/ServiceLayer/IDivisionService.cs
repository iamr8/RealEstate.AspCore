using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;

namespace RealEstate.Services.ServiceLayer
{
    public interface IDivisionService
    {
        Task<MethodStatus<Division>> UpdateAsync(DivisionInputViewModel model, bool save);

        Task<PaginationViewModel<DivisionViewModel>> ListAsync(DivisionSearchViewModel searchModel);

        Task<List<DivisionViewModel>> ListAsync();

        Task<StatusEnum> RemoveAsync(string id);

        Task<DivisionInputViewModel> InputAsync(string id);

        Task<MethodStatus<Division>> AddOrUpdateAsync(DivisionInputViewModel model, bool update, bool save);

        Task<Division> EntityAsync(string id);

        Task<MethodStatus<Division>> AddAsync(DivisionInputViewModel model, bool save);
    }

    public class DivisionService : IDivisionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<Division> _divisions;

        public DivisionService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _divisions = _unitOfWork.Set<Division>();
        }

        public async Task<StatusEnum> RemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var entity = await _divisions.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<List<DivisionViewModel>> ListAsync()
        {
            var query = _divisions.AsQueryable();
            query = query.WhereNotDeleted();

            var divisions = await query.ToListAsync().ConfigureAwait(false);
            return divisions.Map<Division, DivisionViewModel>();
        }

        public async Task<PaginationViewModel<DivisionViewModel>> ListAsync(DivisionSearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_divisions, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<DivisionViewModel>();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    query = query.Where(x => EF.Functions.Like(x.Name, searchModel.Name.Like()));

                if (!string.IsNullOrEmpty(searchModel.Id))
                    query = query.Where(x => EF.Functions.Like(x.Id, searchModel.Id.Like()));

                query = _baseService.AdminSeachConditions(query, searchModel);
            }
            var result = await _baseService.PaginateAsync(query, searchModel?.PageNo ?? 1,
                item => item.Map<Division, DivisionViewModel>()
            ).ConfigureAwait(false);

            return result;
        }

        public async Task<Division> EntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _divisions.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return result;
        }

        public Task<MethodStatus<Division>> AddOrUpdateAsync(DivisionInputViewModel model, bool update, bool save)
        {
            return update
                ? UpdateAsync(model, save)
                : AddAsync(model, save);
        }

        public async Task<MethodStatus<Division>> UpdateAsync(DivisionInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Division>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Division>(StatusEnum.IdIsNull, null);

            var entity = await EntityAsync(model.Id).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                _ => entity.Name = model.Name,
                new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<DivisionInputViewModel> InputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var query = _divisions.Where(x => x.Id == id);

            var entity = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            var viewModel = entity.Map<Division, DivisionViewModel>();
            if (viewModel == null)
                return default;

            var result = new DivisionInputViewModel
            {
                Id = viewModel.Id,
                Name = viewModel.Name
            };

            return result;
        }

        public async Task<MethodStatus<Division>> AddAsync(DivisionInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Division>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.Name))
                return new MethodStatus<Division>(StatusEnum.NameIsNull, null);

            var add = await _baseService.AddAsync(new Division
            {
                Name = model.Name
            }, null, save).ConfigureAwait(false);
            return add;
        }
    }
}