using System;
using System.Linq;
using System.Threading.Tasks;
using EFSecondLevelCache.Core;
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
    public interface IReminderService
    {
        Task<MethodStatus<Reminder>> ReminderAddAsync(ReminderInputViewModel model, bool save);

        Task<bool> HasReminderAsync();

        Task<PaginationViewModel<ReminderViewModel>> ReminderListAsync(ReminderSearchViewModel searchModel, string currentUserId, bool loadData = true);

        Task<StatusEnum> ReminderRemoveAsync(string id);

        Task<PaginationViewModel<ReminderViewModel>> ReminderListAsync(ReminderSearchViewModel searchModel, bool loadData = true);
    }

    public class ReminderService : IReminderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IPictureService _pictureService;
        private readonly ISmsService _smsService;
        private readonly DbSet<Reminder> _reminders;

        public ReminderService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IPictureService pictureService,
            ISmsService smsService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _pictureService = pictureService;
            _smsService = smsService;

            _reminders = _unitOfWork.Set<Reminder>();
        }

        public async Task<bool> HasReminderAsync()
        {
            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return false;

            var query = _reminders.WhereNotDeleted();

            //if (currentUser.Role == Role.Admin || currentUser.Role == Role.SuperAdmin)
            //    query = query.IgnoreQueryFilters();

            query = query
                .Where(x => x.Date.Date == DateTime.Today || x.Date.Date == DateTime.Today.AddDays(1))
                .Where(x => x.UserId == currentUser.Id);

            var models = await query.Cacheable().AnyAsync();
            return models;
        }

        public async Task<StatusEnum> ReminderRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await _reminders.FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<MethodStatus<Reminder>> ReminderAddAsync(ReminderInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Reminder>(StatusEnum.ModelIsNull, null);

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return new MethodStatus<Reminder>(StatusEnum.UserIsNull, null);

            var (addStatus, addedReminder) = await _baseService.AddAsync(new Reminder
            {
                Date = model.Date.PersianToGregorian(),
                Description = model.Description,
                UserId = currentUser.Id,
                CheckBank = model.CheckBank,
                CheckNumber = model.CheckNumber,
                Price = model.Price != null && model.Price > 0 ? (decimal)model.Price : 0,
            }, null, save);
            if (addStatus != StatusEnum.Success)
                return new MethodStatus<Reminder>(addStatus, null);

            //await _smsService.SendAsync(new[] {currentUser.Mobile},SmsTemplateEnum.InviteToDiscuss,)
            await _pictureService.PictureAddAsync(model.Pictures, null, null, null, null, null, addedReminder.Id, true);
            return new MethodStatus<Reminder>(StatusEnum.Success, addedReminder);
        }

        public async Task<PaginationViewModel<ReminderViewModel>> ReminderListAsync(ReminderSearchViewModel searchModel, string currentUserId, bool loadData = true)
        {
            if (string.IsNullOrEmpty(currentUserId))
                return default;

            var query = _reminders
                .Include(x => x.Pictures)
                .Where(x => x.UserId == currentUserId);

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.CheckBank))
                    query = query.Where(x => EF.Functions.Like(x.CheckBank, searchModel.CheckBank.Like()));

                if (!string.IsNullOrEmpty(searchModel.CheckNumber))
                    query = query.Where(x => EF.Functions.Like(x.CheckNumber, searchModel.CheckNumber.Like()));

                if (!string.IsNullOrEmpty(searchModel.Subject))
                    query = query.Where(x => EF.Functions.Like(x.Description, searchModel.Subject.Like()));

                if (searchModel.Price != null && searchModel.Price > 0)
                    query = query.Where(x => x.Price == searchModel.Price);

                if (!string.IsNullOrEmpty(searchModel.FromDate))
                {
                    var dateTime = searchModel.FromDate.PersianToGregorian();
                    query = query.Where(x => EF.Functions.DateDiffHour(dateTime, x.Date) <= 0);
                }

                if (!string.IsNullOrEmpty(searchModel.ToDate))
                {
                    var dateTime = searchModel.FromDate.PersianToGregorian();
                    query = query.Where(x => EF.Functions.DateDiffHour(dateTime, x.Date) >= 0);
                }

                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            //            if (currentUser?.Role == Role.SuperAdmin)
            //                models = models.IgnoreQueryFilters();

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<ReminderViewModel>(act =>
                {
                    act.IncludeAs<Reminder, Picture, PictureViewModel>(_unitOfWork, x => x.Pictures);
                }), loadData: loadData);

            return result;
        }

        public async Task<PaginationViewModel<ReminderViewModel>> ReminderListAsync(ReminderSearchViewModel searchModel, bool loadData = true)
        {
            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return default;

            return await ReminderListAsync(searchModel, currentUser.Id);
        }
    }
}