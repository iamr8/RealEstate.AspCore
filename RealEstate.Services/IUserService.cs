using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Extensions;
using RealEstate.Services.Base;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Input;
using RealEstate.ViewModels.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationScheme = RealEstate.Extensions.AuthenticationScheme;
using CryptologyExtension = RealEstate.Extensions.CryptologyExtension;

namespace RealEstate.Services
{
    public interface IUserService
    {
        Task<UserInputViewModel> FindInputAsync(string id);

        Task<List<BeneficiaryJsonViewModel>> ListJsonAsync();

        Task<PaginationViewModel<UserViewModel>> ListAsync(int page, string userName, string userFirst,
            string userLast, string userMobile, string userAddress, string password, string role, string userId);

        Task<bool> IsUserValidAsync(List<Claim> claims);

        Task<StatusEnum> RemoveAsync(string userId);

        Task SignOutAsync();

        Task<StatusEnum> SignInAsync(UserLoginViewModel model);
    }

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IMapService _mapService;
        private readonly DbSet<User> _users;
        private readonly DbSet<Log> _logs;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IMapService mapService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _mapService = mapService;
            _httpContextAccessor = httpContextAccessor;
            _users = _unitOfWork.Set<User>();
            _logs = _unitOfWork.Set<Log>();
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public async Task<UserInputViewModel> FindInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var query = _users.Where(x => x.Id == id);
            var model = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            if (model == null) return null;

            var result = _baseService.Map(model,
                new UserInputViewModel
                {
                    Role = model.Role,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Mobile = model.Mobile,
                    Password = model.Password.Cipher(CryptologyExtension.CypherMode.Decryption),
                    Username = model.Username,
                    Address = model.Address,
                    Phone = model.Phone,
                });
            return result;
        }

        public async Task<List<BeneficiaryJsonViewModel>> ListJsonAsync()
        {
            var users = await _users.Filtered().ToListAsync().ConfigureAwait(false);
            var result = _baseService.Map(users, x => new BeneficiaryJsonViewModel
            {
                UserId = x.Id,
                UserFullName = $"{x.LastName}، {x.FirstName}",
            });
            return result;
        }

        public async Task<PaginationViewModel<UserViewModel>> ListAsync(int page, string userName, string userFirst,
            string userLast, string userMobile, string userAddress, string password, string role, string userId)
        {
            var models = _users as IQueryable<User>;

            if (!string.IsNullOrEmpty(userName))
                models = models.Where(x => EF.Functions.Like(x.Username, userName.LikeExpression()));

            if (!string.IsNullOrEmpty(userFirst))
                models = models.Where(x => EF.Functions.Like(x.FirstName, userFirst.LikeExpression()));

            if (!string.IsNullOrEmpty(userLast))
                models = models.Where(x => EF.Functions.Like(x.LastName, userLast.LikeExpression()));

            if (!string.IsNullOrEmpty(userMobile))
                models = models.Where(x => EF.Functions.Like(x.Mobile, userMobile.LikeExpression()));

            if (!string.IsNullOrEmpty(userAddress))
                models = models.Where(x => EF.Functions.Like(x.Address, userAddress.LikeExpression()));

            if (!string.IsNullOrEmpty(role))
                models = models.Where(x => x.Role.ToString() == role);

            if (!string.IsNullOrEmpty(userId))
                models = models.Where(x => x.Id == userId);

            var result = await _baseService.PaginateAsync(models, page, _mapService.Map,
                new[]
                {
                    Role.SuperAdmin
                }, null).ConfigureAwait(false);

            if (result?.Items?.Any() != true)
                return default;

            var superAdmin = result.Items.Find(x => x.Username == "admin" && x.Role == Role.SuperAdmin);
            if (superAdmin?.Tracks?.Any() == true)
            {
                var tempTracks = superAdmin.Tracks;
                var creationTrack = tempTracks.Find(x => x.Type == TrackTypeEnum.Create);
                if (creationTrack != null)
                {
                    var isRemoved = tempTracks.Remove(creationTrack);
                    if (isRemoved)
                        superAdmin.Tracks = tempTracks;
                }
            }

            return result;
        }

        //public async Task<(StatusEnum, string)> AddAsync(UserInputViewModel model)
        //{
        //    if (model == null)
        //        return new ValueTuple<StatusEnum, string>(StatusEnum.ModelIsNull, null);

        //    StatusEnum finalStatus;
        //    User finalEntity;

        //    if (model?.Id != null)
        //    {
        //        var models = _users.Where(x => x.Id == model.Id);

        //        var (status, entity) = await _baseService.UpdateTrackedAsync(models,
        //            true,
        //            user => user.Username == model.Username,
        //            (user, currentUser) =>
        //            {
        //                user.FirstName = model.FirstName;
        //                user.LastName = model.LastName;
        //                user.Mobile = model.Mobile;
        //                user.Password = model.Password.Cipher(CryptologyExtension.CypherMode.Encryption);
        //                user.Username = model.Username;
        //                user.Role = model.Role == Role.SuperAdmin ? Role.Admin : model.Role;
        //                user.Address = model.Address;
        //                user.Phone = model.Phone;
        //            },
        //            oldEntity =>
        //            {
        //                var message = $"کاربر \"{oldEntity.Username}\" ویرایش شد.";
        //                _baseService.AddNotification(message, Role.Admin, Role.SuperAdmin);
        //            }, Role.SuperAdmin).ConfigureAwait(false);

        //        finalEntity = entity;
        //        finalStatus = status;
        //    }
        //    else
        //    {
        //        var (status, entity) = await _baseService.AddAsync(_users,
        //            (userEntity, user) => userEntity.Username == model.Username,
        //            new User
        //            {
        //                FirstName = model.FirstName,
        //                LastName = model.LastName,
        //                Mobile = model.Mobile,
        //                Password = model.Password.Cipher(CryptologyExtension.CypherMode.Encryption),
        //                Username = model.Username,
        //                Role = model.Role,
        //                Address = model.Address,
        //                Phone = model.Phone
        //            },
        //            new[]
        //            {
        //                Role.SuperAdmin
        //            },
        //            new[]
        //            {
        //                Role.Admin, Role.SuperAdmin
        //            }).ConfigureAwait(false);

        //        finalEntity = entity;
        //        finalStatus = status;
        //    }

        //    return new ValueTuple<StatusEnum, string>(finalStatus, finalEntity?.Id);
        //}

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

            var user = await _users.FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin
                    },
                    true,
                    true,
                    null)
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

            var lastTrack = userDb.LastLog();
            if (lastTrack?.Type == TrackTypeEnum.Delete)
                return StatusEnum.Deactivated;

            var itemCategoriesJson = userDb.UserItemCategories.JsonConversion(itemCategory => new ItemCategoryViewModel
            {
                Id = itemCategory.Id,
                Name = itemCategory.ItemCategory.Name
            });
            var propertyCategoriesJson = userDb.UserPropertyCategories.JsonConversion(itemCategory => new PropertyCategoryViewModel
            {
                Id = itemCategory.Id,
                Name = itemCategory.PropertyCategory.Name
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

            var identity = new ClaimsIdentity(claims, AuthenticationScheme.Scheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IssuedUtc = DateTimeOffset.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(6),
                IsPersistent = true,
            };

            await HttpContext.SignInAsync(AuthenticationScheme.Scheme, principal, properties).ConfigureAwait(false);
            HttpContext.User.AddIdentity(identity);

            var identities = HttpContext.User.Identities.Where(x => x.Claims.Any()).ToList();
            if (identities.Count == 0) return StatusEnum.Failed;

            var findIdentity = identities.Find(x => x.Name == identity.Name);
            if (findIdentity == null) return StatusEnum.Failed;

            var isConnected = findIdentity.AuthenticationType == AuthenticationScheme.Scheme
                              && findIdentity.IsAuthenticated;
            return isConnected
                ? StatusEnum.SignedIn
                : StatusEnum.Failed;
        }
    }
}