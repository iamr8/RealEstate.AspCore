using EFSecondLevelCache.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IUserService
    {
        Task<UserInputViewModel> FindInputAsync(string id);

        Task<MethodStatus<User>> AddOrUpdateAsync(UserInputViewModel model, bool update, bool save);

        Task<List<BeneficiaryJsonViewModel>> ListJsonAsync(bool includeDeleted = false, bool exceptAdmin = true);

        string BaseUrl { get; }

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
        private readonly DbSet<UserItemCategory> _userItemCategories;
        private readonly DbSet<UserPropertyCategory> _userPropertyCategories;
        private readonly DbSet<EmployeeDivision> _employeeDivisions;
        private readonly DbSet<Employee> _employees;
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
            _employeeDivisions = _unitOfWork.Set<EmployeeDivision>();
            _userItemCategories = _unitOfWork.Set<UserItemCategory>();
            _userPropertyCategories = _unitOfWork.Set<UserPropertyCategory>();
            _employees = _unitOfWork.Set<Employee>();
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public async Task<List<BeneficiaryJsonViewModel>> ListJsonAsync(bool includeDeleted = false, bool exceptAdmin = true)
        {
            var query = _users.AsQueryable();
            if (includeDeleted)
                query = query.IgnoreQueryFilters();

            query = query.Include(x => x.Employee);

            if (exceptAdmin)
                query = query.Where(x => !x.Username.Equals("admin", StringComparison.CurrentCultureIgnoreCase));

            var models = await query.Cacheable().ToListAsync();
            if (models?.Any() != true)
                return default;

            var list = new List<UserViewModel>();
            foreach (var user in models)
            {
                var item = user.Map<UserViewModel>(act => act.IncludeAs<User, Employee, EmployeeViewModel>(_unitOfWork, x => x.Employee));
                list.Add(item);
            }
            if (list?.Any() != true)
                return default;

            var result = from user in list
                         let employee = user.Employee
                         let isDeleted = employee.IsDeleted ? " - کاربر غیرفعال" : ""
                         select new BeneficiaryJsonViewModel
                         {
                             UserId = user.Id,
                             UserFullName = $"{employee.LastName}، {employee.FirstName}{isDeleted}"
                         };

            return result.ToList();
        }

        public async Task<UserInputViewModel> FindInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var model = await _baseService.QueryByRole(_users)
                .AsNoTracking()
                .Include(x => x.UserItemCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.UserPropertyCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);
            var viewModel = model?.Map<UserViewModel>(ent =>
            {
                ent.IncludeAs<User, UserItemCategory, UserItemCategoryViewModel>(_unitOfWork, x => x.UserItemCategories,
                    ent2 => ent2.IncludeAs<UserItemCategory, Category, CategoryViewModel>(_unitOfWork, x => x.Category));
                ent.IncludeAs<User, UserPropertyCategory, UserPropertyCategoryViewModel>(_unitOfWork, x => x.UserPropertyCategories,
                    ent2 => ent2.IncludeAs<UserPropertyCategory, Category, CategoryViewModel>(_unitOfWork, x => x.Category));
                ent.IncludeAs<User, Employee, EmployeeViewModel>(_unitOfWork, x => x.Employee);
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

            query = query.AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.UserItemCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.UserPropertyCategories)
                .ThenInclude(x => x.Category);

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

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<UserViewModel>(ent =>
                {
                    ent.IncludeAs<User, Employee, EmployeeViewModel>(_unitOfWork, x => x.Employee);
                    ent.IncludeAs<User, UserItemCategory, UserItemCategoryViewModel>(_unitOfWork, x => x.UserItemCategories,
                        ent2 => ent2.IncludeAs<UserItemCategory, Category, CategoryViewModel>(_unitOfWork, x => x.Category));
                    ent.IncludeAs<User, UserPropertyCategory, UserPropertyCategoryViewModel>(_unitOfWork, x => x.UserPropertyCategories,
                        ent2 => ent2.IncludeAs<UserPropertyCategory, Category, CategoryViewModel>(_unitOfWork, x => x.Category));
                }));
            return result;
        }

        public async Task<User> EntityAsync(string id, params Role[] allowedRolesshowDeletedItems)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _baseService.QueryByRole(_users, allowedRolesshowDeletedItems).FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<MethodStatus<User>> UpdateAsync(UserInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<User>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<User>(StatusEnum.IdIsNull, null);

            var user = _baseService.CurrentUser();
            if (user == null)
                return new MethodStatus<User>(StatusEnum.UserIsNull, null);

            var entity = await EntityAsync(model.Id, null);
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
                StatusEnum.UserIsNull);

            if (user.Role == Role.SuperAdmin)
                await SyncAsync(updatedUser, model, false);

            return await _baseService.SaveChangesAsync(updatedUser);
        }

        public async Task<StatusEnum> SyncAsync(User user, UserInputViewModel model, bool save)
        {
            await _baseService.SyncAsync(
                user.UserItemCategories,
                model.UserItemCategories,
                (itemCategory, currentUser) => new UserItemCategory
                {
                    UserId = user.Id,
                    CategoryId = itemCategory.CategoryId
                },
                inDb => inDb.CategoryId,
                (inDb, inModel) => inDb.CategoryId == inModel.CategoryId,
                null,
                null,
                null);

            await _baseService.SyncAsync(
                user.UserPropertyCategories,
                model.UserPropertyCategories,
                (propertyCategory, currentUser) => new UserPropertyCategory
                {
                    UserId = user.Id,
                    CategoryId = propertyCategory.CategoryId
                },
                inDb => inDb.CategoryId,
                (inDb, inModel) => inDb.CategoryId == inModel.CategoryId,
                null,
                null,
                null);

            //            await _paymentService.FixedSalarySyncAsync(model.FixedSalary, user.Id, false);
            return await _baseService.SaveChangesAsync();
        }

        public Task<MethodStatus<User>> AddOrUpdateAsync(UserInputViewModel model, bool update, bool save)
        {
            return update
                ? UpdateAsync(model, save)
                : AddAsync(model, save);
        }

        public async Task<MethodStatus<User>> AddAsync(UserInputViewModel model, bool save)
        {
            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return new MethodStatus<User>(StatusEnum.UserIsNull, null);

            if (currentUser.Role != Role.SuperAdmin)
                return new MethodStatus<User>(StatusEnum.Forbidden, null);

            if (model == null)
                return new MethodStatus<User>(StatusEnum.ModelIsNull, null);

            var existing = await _users.AnyAsync(x => x.Username.Equals(model.Username, StringComparison.CurrentCultureIgnoreCase));
            if (existing)
                return new MethodStatus<User>(StatusEnum.AlreadyExists, null);

            var (userAddStatus, newUser) = await _baseService.AddAsync(new User
            {
                Username = model.Username,
                Password = model.Password.Cipher(CryptologyExtension.CypherMode.Encryption),
                EmployeeId = model.EmployeeId,
                Role = model.Role
            }, new[]
            {
                Role.SuperAdmin
            }, false);

            await SyncAsync(newUser, model, false);
            return await _baseService.SaveChangesAsync(newUser);
        }

        public async Task<bool> IsUserValidAsync(List<Claim> claims)
        {
            var currentUser = _baseService.CurrentUser(claims);

            var query = from user in _users
                        where user.Id == currentUser.Id
                        where user.Username == currentUser.Username
                        where user.Password == currentUser.EncryptedPassword
                        where user.EmployeeId == currentUser.EmployeeId
                        where user.Role == currentUser.Role
                        select user;

            var foundUser = await query.Cacheable().AnyAsync();
            return foundUser;
        }

        public async Task<StatusEnum> RemoveAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return StatusEnum.ParamIsNull;

            var entity = await _users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin
                    })
                ;

            return result;
        }

        public async Task SignOutAsync()
        {
            await HttpContext.SignOutAsync();
        }

        public string BaseUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Host}:{HttpContext.Request.Host.Port}/";

        public async Task<StatusEnum> SignInAsync(UserLoginViewModel model)
        {
            var lowerUsername = model.Username.ToLower(CultureInfo.CurrentCulture);
            var query = _users.IgnoreQueryFilters()
                .AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.UserItemCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.UserPropertyCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.Employee.EmployeeDivisions)
                .ThenInclude(x => x.Division)
                .AsQueryable();

            var query2 = from user in query
                         let employee = user.Employee
                         let userItemCategories = from uic in user.UserItemCategories
                                                  select new
                                                  {
                                                      uic.CategoryId,
                                                      uic.Category.Name
                                                  }
                         let userPropertyCategories = from upc in user.UserPropertyCategories
                                                      select new
                                                      {
                                                          upc.CategoryId,
                                                          upc.Category.Name
                                                      }
                         let employeeDivisions = from ed in employee.EmployeeDivisions
                                                 select new
                                                 {
                                                     ed.DivisionId,
                                                     ed.Division.Name
                                                 }
                         where user.Username.ToLower() == lowerUsername
                         select new
                         {
                             Password = user.Password,
                             UserAudits = user.Audits,
                             EmployeeAudits = employee.Audits,
                             UserItemCategories = userItemCategories.ToList(),
                             UserPropertyCategories = userPropertyCategories.ToList(),
                             EmployeeDivisions = employeeDivisions.ToList(),
                             Id = user.Id,
                             Username = user.Username,
                             Mobile = employee.Mobile,
                             FirstName = employee.FirstName,
                             LastName = employee.LastName,
                             EmployeeId = employee.Id,
                             Role = user.Role
                         };

            var userDb = await query2.Cacheable().FirstOrDefaultAsync();
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

            if (userDb.UserAudits.Render().Last.Type == LogTypeEnum.Delete)
                return StatusEnum.Deactivated;

            if (userDb.EmployeeAudits.Render().Last.Type == LogTypeEnum.Delete)
                return StatusEnum.EmployeeIsNull;

            var itemCategoriesJson = userDb.UserItemCategories.JsonConversion(category => new CategoryJsonViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
            });
            var propertyCategoriesJson = userDb.UserPropertyCategories.JsonConversion(category => new CategoryJsonViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
            });
            var employeeDivisionsJson = userDb.EmployeeDivisions?.JsonConversion(division => new DivisionJsonViewModel
            {
                DivisionId = division.DivisionId,
                Name = division.Name
            });

            var id = userDb.Id;
            var username = userDb.Username;
            var mobilePhone = userDb.Mobile;
            var password = userDb.Password;
            var firstname = userDb.FirstName;
            var lastname = userDb.LastName;
            var employeeId = userDb.EmployeeId;
            var role = userDb.Role.ToString();
            itemCategoriesJson = string.IsNullOrEmpty(itemCategoriesJson) ? "[]" : itemCategoriesJson;
            propertyCategoriesJson = string.IsNullOrEmpty(propertyCategoriesJson) ? "[]" : propertyCategoriesJson;
            employeeDivisionsJson = string.IsNullOrEmpty(employeeDivisionsJson) ? "[]" : employeeDivisionsJson;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.MobilePhone, mobilePhone),
                new Claim(ClaimTypes.Hash, password),
                new Claim("FirstName", firstname),
                new Claim("LastName", lastname),
                new Claim("EmployeeId", employeeId),
                new Claim("ItemCategories", itemCategoriesJson),
                new Claim("PropertyCategories", propertyCategoriesJson),
                new Claim(ClaimTypes.Role, role),
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

            await HttpContext.SignInAsync(Extensions.AuthenticationScheme.Scheme, principal, properties);
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