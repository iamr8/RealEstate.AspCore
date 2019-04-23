﻿using Microsoft.EntityFrameworkCore;
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
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IReminderService
    {
        Task<ReminderInputViewModel> ReminderInputAsync(string id);
        Task<StatusEnum> ReminderRemoveAsync(string id);

        Task<(StatusEnum, Reminder)> ReminderAddOrUpdateAsync(ReminderInputViewModel model, bool update, bool save);

        Task<PaginationViewModel<ReminderViewModel>> ReminderListAsync(ReminderSearchViewModel searchModel);
    }

    public class ReminderService : IReminderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<Reminder> _reminders;
        private readonly DbSet<Check> _checks;

        public ReminderService(
            IUnitOfWork unitOfWork,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;

            _reminders = _unitOfWork.Set<Reminder>();
            _checks = _unitOfWork.Set<Check>();
        }

        public async Task<ReminderInputViewModel> ReminderInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await _reminders.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var viewModel = entity.Into<Reminder, ReminderViewModel>();
            if (viewModel == null)
                return default;

            var result = new ReminderInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                Date = viewModel.Date.GregorianToPersian(true)
            };

            return result;
        }

        public Task<(StatusEnum, Reminder)> ReminderAddOrUpdateAsync(ReminderInputViewModel model, bool update, bool save)
        {
            return update
                ? ReminderUpdateAsync(model, save)
                : ReminderAddAsync(model, save);
        }

        public async Task<StatusEnum> ReminderRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await _reminders.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
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

        public async Task<(StatusEnum, Reminder)> ReminderAddAsync(ReminderInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Reminder>(StatusEnum.ModelIsNull, null);

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return new ValueTuple<StatusEnum, Reminder>(StatusEnum.UserIsNull, null);

            var addStatus = await _baseService.AddAsync(new Reminder
            {
                Date = model.Date.PersianToGregorian(),
                Description = model.Description,
                UserId = currentUser.Id,
            }, new[]
            {
                Role.SuperAdmin, Role.Admin
            }, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, Reminder)> ReminderUpdateAsync(ReminderInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Reminder>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Reminder>(StatusEnum.IdIsNull, null);

            var entity = await _reminders.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            var updateStatus = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.Description = model.Description;
                    entity.Date = model.Date.PersianToGregorian();
                }, new[]
                {
                    Role.SuperAdmin
                }, save, StatusEnum.UserIsNull).ConfigureAwait(false);
            return updateStatus;
        }

        public async Task<PaginationViewModel<ReminderViewModel>> ReminderListAsync(ReminderSearchViewModel searchModel)
        {
            var models = _reminders.AsQueryable();

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Reminder, ReminderViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetChecks(false, act2 =>
                    {
                        act2.GetDeal();
                        act2.GetPictures();
                    });
                })
            ).ConfigureAwait(false);

            return result;
        }
    }
}