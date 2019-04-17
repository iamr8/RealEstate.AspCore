using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IUserService
    {
        Task<UserInputViewModel> FindInputAsync(string id);

        Task<(StatusEnum, User)> CategoryAddOrUpdateAsync(UserInputViewModel model, bool update, bool save);

        Task<List<BeneficiaryJsonViewModel>> ListJsonAsync();

        Task<(StatusEnum, FixedSalary)> FixedSalarySyncAsync(double value, string userId, bool save);

        Task<PaginationViewModel<UserViewModel>> ListAsync(UserSearchViewModel searchModel);

        Task<bool> IsUserValidAsync(List<Claim> claims);

        Task<StatusEnum> RemoveAsync(string userId);

        Task SignOutAsync();

        Task<StatusEnum> SignInAsync(UserLoginViewModel model);
    }

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly DbSet<User> _users;
        private readonly DbSet<FixedSalary> _fixedSalaries;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _httpContextAccessor = httpContextAccessor;
            _users = _unitOfWork.Set<User>();
            _fixedSalaries = _unitOfWork.Set<FixedSalary>();
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public async Task<UserInputViewModel> FindInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var model = await EntityAsync(id, null).ConfigureAwait(false);
            var viewModel = model?.Into<User, UserViewModel>(false, act =>
            {
                act.GetUserItemCategories(false, act2 => act2.GetCategory());
                act.GetUserPropertyCategories(false, act2 => act2.GetCategory());
                act.GetFixedSalaries();
            });
            if (viewModel == null)
                return default;

            var result = new UserInputViewModel
            {
                Role = viewModel.Role,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Mobile = viewModel.Mobile,
                Password = viewModel.EncryptedPassword.Cipher(CryptologyExtension.CypherMode.Decryption),
                Username = viewModel.Username,
                Address = viewModel.Address,
                Phone = viewModel.Phone,
                UserItemCategories = viewModel.UserItemCategories?.Select(x => new UserItemCategoryJsonViewModel
                {
                    Id = x.Id,
                    Name = x.Category?.Name
                }).ToList(),
                UserPropertyCategories = viewModel.UserPropertyCategories?.Select(x => new UserPropertyCategoryJsonViewModel
                {
                    Id = x.Id,
                    Name = x.Category?.Name
                }).ToList(),
                FixedSalary = viewModel.FixedSalaries?.OrderByDescending(x => x.Logs.Create).FirstOrDefault()?.Value ?? 0,
                Id = viewModel.Id
            };
            return result;
        }

        public async Task<List<BeneficiaryJsonViewModel>> ListJsonAsync()
        {
            var users = await _users.Filtered().ToListAsync().ConfigureAwait(false);
            if (users?.Any() != true)
                return default;

            var result = new List<BeneficiaryJsonViewModel>();
            foreach (var user in users)
            {
                var item = new BeneficiaryJsonViewModel
                {
                    Id = user.Id,
                    UserId = user.Id,
                    UserFullName = $"{user.LastName}، {user.FirstName}",
                };
                result.Add(item);
            }
            return result;
        }

        public async Task<PaginationViewModel<UserViewModel>> ListAsync(UserSearchViewModel searchModel)
        {
            var models = _users as IQueryable<User>;

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Username))
                    models = models.Where(x => EF.Functions.Like(x.Username, searchModel.Username.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.FirstName))
                    models = models.Where(x => EF.Functions.Like(x.FirstName, searchModel.FirstName.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.LastName))
                    models = models.Where(x => EF.Functions.Like(x.LastName, searchModel.LastName.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.Mobile))
                    models = models.Where(x => EF.Functions.Like(x.Mobile, searchModel.Mobile.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.Address))
                    models = models.Where(x => EF.Functions.Like(x.Address, searchModel.Address.LikeExpression()));

                if (searchModel.Role != null)
                    models = models.Where(x => x.Role == searchModel.Role);

                if (!string.IsNullOrEmpty(searchModel.UserId))
                    models = models.Where(x => x.Id == searchModel.UserId);
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<User, UserViewModel>()).ConfigureAwait(false);

            if (result?.Items?.Any() != true)
                return result;

            var superAdmin = result.Items.Find(x => x.Username == "admin" && x.Role == Role.SuperAdmin);
            if (superAdmin?.Logs?.Last() == null)
                return result;

            //            var tempLogs = superAdmin.Logs;
            //            var creationTrack = tempLogs.Create;
            //            if (creationTrack == null)
            //                return result;

            //            tempLogs.Create = null;
            //            superAdmin.Logs = tempLogs;
            return result;
        }

        public async Task<User> EntityAsync(string id, params Role[] allowedRolesshowDeletedItems)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _baseService.QueryByRole(_users, allowedRolesshowDeletedItems).FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return result;
        }

        public async Task<(StatusEnum, User)> UpdateAsync(UserInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, User>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, User>(StatusEnum.IdIsNull, null);

            var entity = await EntityAsync(model.Id, null).ConfigureAwait(false);
            var (updateStatus, updatedUser) = await _baseService.UpdateAsync(entity,
                () =>
                {
                    entity.Address = model.Address;
                    entity.FirstName = model.FirstName;
                    entity.LastName = model.LastName;
                    entity.Mobile = model.Mobile;
                    entity.Password = model.Password.Cipher(CryptologyExtension.CypherMode.Encryption);
                    entity.Phone = model.Phone;
                    entity.Role = entity.Role == Role.SuperAdmin ? Role.SuperAdmin : model.Role;
                }, new[]
                {
                    Role.SuperAdmin
                }, false, StatusEnum.UserIsNull).ConfigureAwait(false);

            await SyncAsync(updatedUser, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedUser, save).ConfigureAwait(false);
        }

        public async Task<StatusEnum> SyncAsync(User user, UserInputViewModel model, bool save)
        {
            var syncItemFeature = await _baseService.SyncAsync(
                user.UserItemCategories,
                model.UserItemCategories,
                itemCategory => new UserItemCategory
                {
                    UserId = user.Id,
                    CategoryId = itemCategory.Id
                },
                (inDb, inModel) => inDb.CategoryId == inModel.Id,
                null,
                false).ConfigureAwait(false);

            var syncPropertyFeature = await _baseService.SyncAsync(
                user.UserPropertyCategories,
                model.UserPropertyCategories,
                propertyCategory => new UserPropertyCategory
                {
                    UserId = user.Id,
                    CategoryId = propertyCategory.Id
                },
                (inDb, inModel) => inDb.CategoryId == inModel.Id,
                null,
                false).ConfigureAwait(false);

            await FixedSalarySyncAsync(model.FixedSalary, user.Id, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);
        }

        public Task<(StatusEnum, User)> CategoryAddOrUpdateAsync(UserInputViewModel model, bool update, bool save)
        {
            return update
                ? UpdateAsync(model, save)
                : AddAsync(model, save);
        }

        public async Task<(StatusEnum, User)> AddAsync(UserInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, User>(StatusEnum.ModelIsNull, null);

            var (userAddStatus, newUser) = await _baseService.AddAsync(new User
            {
                Role = model.Role,
                Address = model.Address,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Phone = model.Phone,
                Password = model.Password.Cipher(CryptologyExtension.CypherMode.Encryption),
                Mobile = model.Mobile
            }, new[]
            {
                Role.SuperAdmin
            }, false).ConfigureAwait(false);

            await SyncAsync(newUser, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newUser, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, FixedSalary)> FixedSalarySyncAsync(double value, string userId, bool save)
        {
            if (value <= 0 || string.IsNullOrEmpty(userId))
                return new ValueTuple<StatusEnum, FixedSalary>(StatusEnum.ParamIsNull, null);

            var findSalary = await _fixedSalaries.OrderByDescending(x => x.DateTime).FirstOrDefaultAsync(x => x.UserId == userId && x.Value == value)
                .ConfigureAwait(false);
            if (findSalary != null)
                return new ValueTuple<StatusEnum, FixedSalary>(StatusEnum.AlreadyExists, findSalary);

            var addStatus = await _baseService.AddAsync(new FixedSalary
            {
                UserId = userId,
                Value = value
            }, null, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<bool> IsUserValidAsync(List<Claim> claims)
        {
            var currentUser = _baseService.CurrentUser(claims);

            var models = _users.Filtered();
            var foundUser = await (from user in models
                                   where user.Id == currentUser.Id
                                   where user.Username == currentUser.Username
                                   where user.Mobile == currentUser.Mobile
                                   where user.Role == currentUser.Role
                                   where user.Password == currentUser.EncryptedPassword
                                   where user.FirstName == currentUser.FirstName
                                   where user.LastName == currentUser.LastName
                                   where user.Phone == currentUser.Phone
                                   select user).FirstOrDefaultAsync().ConfigureAwait(false);
            return foundUser != null;
        }

        public async Task<StatusEnum> RemoveAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return StatusEnum.ParamIsNull;

            var user = await EntityAsync(userId).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task SignOutAsync()
        {
            await HttpContext.SignOutAsync().ConfigureAwait(false);
        }

        public async Task<StatusEnum> SignInAsync(UserLoginViewModel model)
        {
            var userDb = await _users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Username == model.Username)
                .ConfigureAwait(false);
            if (userDb == null) return StatusEnum.UserNotFound;
            try
            {
                var decrypted = userDb.Password.Cipher(CryptologyExtension.CypherMode.Decryption);
                if (model.Password != decrypted)
                    return StatusEnum.WrongPassword;
            }
            catch
            {
                return StatusEnum.WrongPassword;
            }

            if (userDb.LastLog()?.Type == LogTypeEnum.Delete)
                return StatusEnum.Deactivated;

            var itemCategoriesJson = userDb.UserItemCategories.JsonConversion(category => new CategoryJsonViewModel
            {
                Id = category.Id,
                Name = category.Category.Name,
            });
            var propertyCategoriesJson = userDb.UserPropertyCategories.JsonConversion(category => new CategoryJsonViewModel
            {
                Id = category.Id,
                Name = category.Category.Name
            });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDb.Id),
                new Claim(ClaimTypes.Name, userDb.Username),
                new Claim(ClaimTypes.MobilePhone, userDb.Mobile),
                new Claim(ClaimTypes.Role, userDb.Role.ToString()),
                new Claim(ClaimTypes.Hash,userDb.Password),
                new Claim("FirstName",userDb.FirstName),
                new Claim("LastName",userDb.LastName),
                new Claim(ClaimTypes.HomePhone, userDb.Phone),
                new Claim(ClaimTypes.StreetAddress, userDb.Address),
                new Claim("CreationDateTime",userDb.DateTime.ToString("G")),
                new Claim("ItemCategories",itemCategoriesJson),
                new Claim("PropertyCategories",propertyCategoriesJson),
            };

            var identity = new ClaimsIdentity(claims, Extensions.AuthenticationScheme.Scheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IssuedUtc = DateTimeOffset.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(6),
                IsPersistent = true,
            };

            await HttpContext.SignInAsync(Extensions.AuthenticationScheme.Scheme, principal, properties).ConfigureAwait(false);
            HttpContext.User.AddIdentity(identity);

            var identities = HttpContext.User.Identities.Where(x => x.Claims.Any()).ToList();
            if (identities.Count == 0) return StatusEnum.Failed;

            var findIdentity = identities.Find(x => x.Name == identity.Name);
            if (findIdentity == null) return StatusEnum.Failed;

            var isConnected = findIdentity.AuthenticationType == Extensions.AuthenticationScheme.Scheme
                              && findIdentity.IsAuthenticated;
            return isConnected
                ? StatusEnum.SignedIn
                : StatusEnum.Failed;
        }
    }
}