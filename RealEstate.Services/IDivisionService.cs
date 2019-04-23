using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IDivisionService
    {
        Task<(StatusEnum, Division)> UpdateAsync(DivisionInputViewModel model, bool save);

        Task<PaginationViewModel<DivisionViewModel>> ListAsync(DivisionSearchViewModel searchModel);

        Task<List<DivisionViewModel>> ListAsync();

        Task<StatusEnum> RemoveAsync(string id);

        Task<DivisionInputViewModel> InputAsync(string id);

        Task<(StatusEnum, Division)> AddOrUpdateAsync(DivisionInputViewModel model, bool update, bool save);

        Task<Division> EntityAsync(string id);

        Task<(StatusEnum, Division)> AddAsync(DivisionInputViewModel model, bool save);
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

            var user = await EntityAsync(id).ConfigureAwait(false);
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

        public async Task<List<DivisionViewModel>> ListAsync()
        {
            var query = _divisions.AsQueryable();
            query = query.WhereNotDeleted();

            var divisions = await query.ToListAsync().ConfigureAwait(false);
            return divisions.Into<Division, DivisionViewModel>();
        }

        public async Task<PaginationViewModel<DivisionViewModel>> ListAsync(DivisionSearchViewModel searchModel)
        {
            var models = _divisions.AsQueryable();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                    models = models.Where(x => EF.Functions.Like(x.Name, searchModel.Name.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.Id))
                    models = models.Where(x => EF.Functions.Like(x.Id, searchModel.Id.LikeExpression()));
            }
            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Division, DivisionViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin))
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

        public Task<(StatusEnum, Division)> AddOrUpdateAsync(DivisionInputViewModel model, bool update, bool save)
        {
            return update
                ? UpdateAsync(model, save)
                : AddAsync(model, save);
        }

        public async Task<(StatusEnum, Division)> UpdateAsync(DivisionInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Division>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Division>(StatusEnum.IdIsNull, null);

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
            var viewModel = entity.Into<Division, DivisionViewModel>();
            if (viewModel == null)
                return default;

            var result = new DivisionInputViewModel
            {
                Id = viewModel.Id,
                Name = viewModel.Name
            };

            return result;
        }

        public async Task<(StatusEnum, Division)> AddAsync(DivisionInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Division>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.Name))
                return new ValueTuple<StatusEnum, Division>(StatusEnum.NameIsNull, null);

            var add = await _baseService.AddAsync(new Division
            {
                Name = model.Name
            }, null, save).ConfigureAwait(false);
            return add;
        }
    }
}