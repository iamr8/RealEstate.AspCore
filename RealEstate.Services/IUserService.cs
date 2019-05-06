using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
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

        Task<(StatusEnum, User)> AddOrUpdateAsync(UserInputViewModel model, bool update, bool save);

        Task<List<BeneficiaryJsonViewModel>> ListJsonAsync(bool includeDeleted = false, bool exceptAdmin = true);

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
        private readonly IPaymentService _paymentService;
        private readonly IEmployeeService _employeeService;
        private readonly DbSet<User> _users;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IPaymentService paymentService,
            IEmployeeService employeeService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _httpContextAccessor = httpContextAccessor;
            _users = _unitOfWork.Set<User>();
            _employeeService = employeeService;
            _paymentService = paymentService;
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public async Task<List<BeneficiaryJsonViewModel>> ListJsonAsync(bool includeDeleted = false, bool exceptAdmin = true)
        {
            var query = _users.AsQueryable();
            if (includeDeleted)
                query = query.IgnoreQueryFilters();

            if (exceptAdmin)
                query = query.Where(x => !x.Username.Equals("admin", StringComparison.CurrentCultureIgnoreCase));

            var models = await query.ToListAsync().ConfigureAwait(false);
            if (models?.Any() != true)
                return default;

            var list = new List<UserViewModel>();
            foreach (var user in models)
            {
                var item = user.Into<User, UserViewModel>(false, act => act.GetEmployee());
                list.Add(item);
            }
            if (list?.Any() != true)
                return default;

            var result = list.Select(x => new BeneficiaryJsonViewModel
            {
                UserId = x.Id,
                UserFullName = $"{x.Employee?.LastName}، {x.Employee?.FirstName}",
            }).ToList();
            return result;
        }

        public async Task<UserInputViewModel> FindInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var model = await EntityAsync(id, null).ConfigureAwait(false);
            var viewModel = model?.Into<User, UserViewModel>(false, act =>
            {
                act.GetUserItemCategories(false, act2 => act2.GetCategory());
                act.GetUserPropertyCategories(false, act2 => act2.GetCategory());
                act.GetEmployee();
                //                act.GetFixedSalaries();
            });
            if (viewModel == null)
                return default;

            var result = new UserInputViewModel
            {
                Role = viewModel.Role,
                Password = viewModel.EncryptedPassword.Cipher(CryptologyExtension.CypherMode.Decryption),
                Username = viewModel.Username,
                UserItemCategories = viewModel.UserItemCategories?.Select(x => new UserItemCategoryJsonViewModel
                {
                    CategoryId = x.Category?.Id,
                    Name = x.Category?.Name
                }).ToList(),
                UserPropertyCategories = viewModel.UserPropertyCategories?.Select(x => new UserPropertyCategoryJsonViewModel
                {
                    CategoryId = x.Category?.Id,
                    Name = x.Category?.Name
                }).ToList(),
                //                FixedSalary = viewModel.FixedSalaries?.OrderByDescending(x => x.Logs.Create).FirstOrDefault()?.Value ?? 0,
                Id = viewModel.Id,
                EmployeeId = viewModel.Employee?.Id
            };
            return result;
        }

        public async Task<PaginationViewModel<UserViewModel>> ListAsync(UserSearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_users, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<UserViewModel>();

            if (searchModel != null)
            {
                if (searchModel.Role != null)
                    query = query.Where(x => x.Role == searchModel.Role);

                if (!string.IsNullOrEmpty(searchModel.UserId))
                    query = query.Where(x => x.Id == searchModel.UserId);

                if (!string.IsNullOrEmpty(searchModel.Username))
                    query = query.Where(x => EF.Functions.Like(x.Username, searchModel.Username.Like()));

                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            var result = await _baseService.PaginateAsync(query, searchModel?.PageNo ?? 1,
                item => item.Into<User, UserViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetEmployee(true);
                    act.GetUserItemCategories(false, act2 => act2.GetCategory());
                    act.GetUserPropertyCategories(false, act2 => act2.GetCategory());
                })).ConfigureAwait(false);
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

            var user = _baseService.CurrentUser();
            if (user == null)
                return new ValueTuple<StatusEnum, User>(StatusEnum.UserIsNull, null);

            var entity = await EntityAsync(model.Id, null).ConfigureAwait(false);
            var (updateStatus, updatedUser) = await _baseService.UpdateAsync(entity,
                currentUser =>
                {
                    if (!string.IsNullOrEmpty(model.Password))
                        entity.Password = model.Password.Cipher(CryptologyExtension.CypherMode.Encryption);

                    if (user.Role == Role.SuperAdmin && currentUser.Id != entity.Id)
                        entity.Role = model.Role;
                },
                null,
                false,
                StatusEnum.UserIsNull).ConfigureAwait(false);

            if (user.Role == Role.SuperAdmin)
                await SyncAsync(updatedUser, model, false).ConfigureAwait(false);

            return await _baseService.SaveChangesAsync(updatedUser, save).ConfigureAwait(false);
        }

        public async Task<StatusEnum> SyncAsync(User user, UserInputViewModel model, bool save)
        {
            var syncItemFeature = await _baseService.SyncAsync(
                user.UserItemCategories,
                model.UserItemCategories,
                (itemCategory, currentUser) => new UserItemCategory
                {
                    UserId = user.Id,
                    CategoryId = itemCategory.CategoryId
                },
                (inDb, inModel) => inDb.CategoryId == inModel.CategoryId,
                null,
                false).ConfigureAwait(false);

            var syncPropertyFeature = await _baseService.SyncAsync(
                user.UserPropertyCategories,
                model.UserPropertyCategories,
                (propertyCategory, currentUser) => new UserPropertyCategory
                {
                    UserId = user.Id,
                    CategoryId = propertyCategory.CategoryId
                },
                (inDb, inModel) => inDb.CategoryId == inModel.CategoryId,
                null,
                false).ConfigureAwait(false);

            //            await _paymentService.FixedSalarySyncAsync(model.FixedSalary, user.Id, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);
        }

        public Task<(StatusEnum, User)> AddOrUpdateAsync(UserInputViewModel model, bool update, bool save)
        {
            return update
                ? UpdateAsync(model, save)
                : AddAsync(model, save);
        }

        public async Task<(StatusEnum, User)> AddAsync(UserInputViewModel model, bool save)
        {
            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return new ValueTuple<StatusEnum, User>(StatusEnum.UserIsNull, null);

            if (currentUser.Role != Role.SuperAdmin)
                return new ValueTuple<StatusEnum, User>(StatusEnum.Forbidden, null);

            if (model == null)
                return new ValueTuple<StatusEnum, User>(StatusEnum.ModelIsNull, null);

            var existing = await _users.AnyAsync(x => x.Username.Equals(model.Username, StringComparison.CurrentCultureIgnoreCase)).ConfigureAwait(false);
            if (existing)
                return new ValueTuple<StatusEnum, User>(StatusEnum.AlreadyExists, null);

            var (userAddStatus, newUser) = await _baseService.AddAsync(new User
            {
                Username = model.Username,
                Password = model.Password.Cipher(CryptologyExtension.CypherMode.Encryption),
                EmployeeId = model.EmployeeId,
                Role = model.Role
            }, new[]
            {
                Role.SuperAdmin
            }, false).ConfigureAwait(false);

            await SyncAsync(newUser, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newUser, save).ConfigureAwait(false);
        }

        public async Task<bool> IsUserValidAsync(List<Claim> claims)
        {
            var currentUser = _baseService.CurrentUser(claims);

            var models = _users.WhereNotDeleted();
            var foundUser = await (from user in models
                                   where user.Id == currentUser.Id
                                   where user.Username == currentUser.Username
                                   where user.Password == currentUser.EncryptedPassword
                                   where user.EmployeeId == currentUser.EmployeeId
                                   where user.Role == currentUser.Role
                                   select user).FirstOrDefaultAsync().ConfigureAwait(false);
            return foundUser != null;
        }

        public async Task<StatusEnum> RemoveAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return StatusEnum.ParamIsNull;

            var entity = await _users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(entity,
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
                .FirstOrDefaultAsync(x => x.Username.Equals(model.Username, StringComparison.CurrentCultureIgnoreCase))
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

            if (userDb.IsDeleted)
                return StatusEnum.Deactivated;

            var employee = userDb.Employee;
            if (employee == null)
                return StatusEnum.EmployeeIsNull;

            var itemCategoriesJson = userDb.UserItemCategories?.WhereNotDeleted()?.JsonConversion(category => new CategoryJsonViewModel
            {
                CategoryId = category.Category?.Id,
                Name = category.Category?.Name,
            });
            var propertyCategoriesJson = userDb.UserPropertyCategories?.WhereNotDeleted()?.JsonConversion(category => new CategoryJsonViewModel
            {
                CategoryId = category.Category?.Id,
                Name = category.Category?.Name
            });
            var employeeDivisionsJson = userDb.Employee?.EmployeeDivisions?.JsonConversion(division => new DivisionJsonViewModel
            {
                DivisionId = division.Division?.Id,
                Name = division.Division?.Name
            });
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDb.Id),
                new Claim(ClaimTypes.Name, userDb.Username),
                new Claim(ClaimTypes.MobilePhone, userDb.Employee?.Mobile),
                new Claim(ClaimTypes.Hash,userDb.Password),
                new Claim("FirstName",userDb.Employee?.FirstName),
                new Claim("LastName",userDb.Employee?.LastName),
                new Claim("EmployeeId", userDb.Employee?.Id),
                new Claim("ItemCategories",itemCategoriesJson),
                new Claim("PropertyCategories",propertyCategoriesJson),
                new Claim(ClaimTypes.Role, userDb.Role.ToString()),
                new Claim("EmployeeDivisions", employeeDivisionsJson)
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