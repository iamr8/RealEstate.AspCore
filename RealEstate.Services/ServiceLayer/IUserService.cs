using EFSecondLevelCache.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Api;
using RealEstate.Services.ViewModels.Api.Request;
using RealEstate.Services.ViewModels.Api.Response;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IUserService
    {
        Task<UserInputViewModel> FindInputAsync(string id);

        Task<TokenValidation> ValidateToken(string token);

        Task<MethodStatus<User>> AddOrUpdateAsync(UserInputViewModel model, bool update, bool save);

        Task<List<BeneficiaryJsonViewModel>> ListJsonAsync(bool includeDeleted = false, bool exceptAdmin = true);

        Task<Response<SignInResponse>> SignInAsync(SignInRequest model);

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

            var models = await query.Cacheable().ToListAsync().ConfigureAwait(false);
            if (models?.Any() != true)
                return default;

            var list = new List<UserViewModel>();
            foreach (var user in models)
            {
                var item = user.Map<UserViewModel>(act => act.IncludeAs<Employee, EmployeeViewModel>(act.Entity.Employee));
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

        public async Task<TokenValidation> ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new TokenValidation
                {
                    Message = StatusEnum.Forbidden.GetDisplayName(),
                    Success = false
                };
            }

            var handler = new JwtSecurityTokenHandler();
            if (!(handler.ReadToken(token) is JwtSecurityToken jwtSecurityToken))
                return default;

            var claims = jwtSecurityToken.Claims.ToList();
            if (claims?.Any() != true)
                return new TokenValidation
                {
                    Message = StatusEnum.Forbidden.GetDisplayName(),
                    Success = false
                };

            var id = claims.FirstOrDefault(x => x.Type.Equals("Id", StringComparison.CurrentCulture))?.Value;
            var userName = claims.FirstOrDefault(x => x.Type.Equals("Username", StringComparison.CurrentCulture))?.Value;
            var password = claims.FirstOrDefault(x => x.Type.Equals("Password", StringComparison.CurrentCulture))?.Value;

            if (string.IsNullOrEmpty(id)
                || string.IsNullOrEmpty(userName)
                || string.IsNullOrEmpty(password))
                return new TokenValidation
                {
                    Message = StatusEnum.Forbidden.GetDisplayName(),
                    Success = false
                };

            var isUserValid = await _users.AnyAsync(x =>
                x.Id == id
                && x.Username == userName
                && x.Password == password);
            if (!isUserValid)
                return new TokenValidation
                {
                    Message = StatusEnum.UserNotFound.GetDisplayName(),
                    Success = false,
                };

            return new TokenValidation
            {
                Message = StatusEnum.Success.GetDisplayName(),
                Success = true,
                UserId = id
            };
        }

        public async Task<UserInputViewModel> FindInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var model = await EntityAsync(id, null).ConfigureAwait(false);
            var viewModel = model?.Map<UserViewModel>(ent =>
            {
                ent.IncludeAs<UserItemCategory, UserItemCategoryViewModel>(model.UserItemCategories,
                    ent2 => ent2.IncludeAs<Category, CategoryViewModel>(ent2.Entity.Category));
                ent.IncludeAs<UserPropertyCategory, UserPropertyCategoryViewModel>(model.UserPropertyCategories,
                    ent2 => ent2.IncludeAs<Category, CategoryViewModel>(ent2.Entity.Category));
                ent.IncludeAs<Employee, EmployeeViewModel>(model.Employee);
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
                    ent.IncludeAs<Employee, EmployeeViewModel>(item.Employee);
                    ent.IncludeAs<UserItemCategory, UserItemCategoryViewModel>(item.UserItemCategories,
                        ent2 => ent2.IncludeAs<Category, CategoryViewModel>(ent2.Entity.Category));
                    ent.IncludeAs<UserPropertyCategory, UserPropertyCategoryViewModel>(item.UserPropertyCategories,
                        ent2 => ent2.IncludeAs<Category, CategoryViewModel>(ent2.Entity.Category));
                }), Task.FromResult(false), currentUser);
            return result;
        }

        public async Task<User> EntityAsync(string id, params Role[] allowedRolesshowDeletedItems)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _baseService.QueryByRole(_users, allowedRolesshowDeletedItems).FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
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
            return await _baseService.SaveChangesAsync().ConfigureAwait(false);
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

            var existing = await _users.AnyAsync(x => x.Username.Equals(model.Username, StringComparison.CurrentCultureIgnoreCase)).ConfigureAwait(false);
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
            }, false).ConfigureAwait(false);

            await SyncAsync(newUser, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newUser, save).ConfigureAwait(false);
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

            var entity = await _users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin
                    })
                .ConfigureAwait(false);

            return result;
        }

        public async Task SignOutAsync()
        {
            await HttpContext.SignOutAsync().ConfigureAwait(false);
        }

        public async Task<Response<SignInResponse>> SignInAsync(SignInRequest model)
        {
            var query = _users.IgnoreQueryFilters()
                .Include(x => x.Employee)
                .ThenInclude(x => x.EmployeeDivisions)
                .ThenInclude(x => x.Division)
                .Include(x => x.UserItemCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.UserPropertyCategories)
                .ThenInclude(x => x.Category)
                .Where(x => x.Username.Equals(model.Username, StringComparison.CurrentCultureIgnoreCase))
                .Select(x => new
                {
                    Id = x.Id,
                    MobileNumber = x.Employee.Mobile,
                    Password = x.Password,
                    Role = x.Role,
                    EmployeeId = x.EmployeeId,
                    FirstName = x.Employee.FirstName,
                    LastName = x.Employee.LastName,
                    Audits = x.Audits,
                    UserItemCategories = x.UserItemCategories.Select(c => new
                    {
                        c.Category.Name
                    }).ToList(),
                    UserPropertyCategories = x.UserPropertyCategories.Select(c => new
                    {
                        c.Category.Name
                    }).ToList(),
                    EmployeeDivisions = x.Employee.EmployeeDivisions.Select(c => new
                    {
                        c.Division.Name
                    }).ToList()
                });

            var sql = query.ToSql();
            var userDb = await query.Cacheable().FirstOrDefaultAsync();
            if (userDb == null)
                return new Response<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.UserNotFound.GetDisplayName()
                };
            try
            {
                var decryptedPasswordInDb = userDb.Password.Cipher(CryptologyExtension.CypherMode.Decryption);
                if (decryptedPasswordInDb != model.Password)
                    return new Response<SignInResponse>
                    {
                        Success = false,
                        Message = StatusEnum.WrongPassword.GetDisplayName()
                    };
            }
            catch
            {
                return new Response<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.WrongPassword.GetDisplayName()
                };
            }

            if (userDb.Audits.LastOrDefault()?.Type == LogTypeEnum.Delete)
                return new Response<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.Deactivated.GetDisplayName()
                };

            var itemCategories = userDb.UserItemCategories.Select(x => x.Name).ToList();
            var propertyCategories = userDb.UserPropertyCategories.Select(x => x.Name).ToList();
            var employeeDivisions = userDb.EmployeeDivisions.Select(x => x.Name).ToList();

            var id = userDb.Id;
            var mobileNumber = userDb.MobileNumber;
            var encryptedPassword = userDb.Password;
            var firstname = userDb.FirstName;
            var lastname = userDb.LastName;
            var employeeId = userDb.Id;
            var role = userDb.Role.ToString();

            var claims = new List<Claim>
            {
                new Claim("Id", id),
                new Claim("Username", model.Username),
                new Claim("Password", encryptedPassword),
            };
            var identity = new ClaimsIdentity(claims, Extensions.AuthenticationScheme.Scheme);

            const string privateKey = PrivateKeyConstant.PrivateKey;
            var now = DateTime.UtcNow;
            var symmetricKey = Encoding.UTF8.GetBytes(privateKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey),
                    SecurityAlgorithms.HmacSha256Signature),
                Expires = now.AddDays(1),
                Issuer = "http://localhost/",
                Audience = "Any",
                NotBefore = now,
            };
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            if (string.IsNullOrEmpty(token))
                return new Response<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.TokenIsNull.GetDisplayName()
                };

            try
            {
                return new Response<SignInResponse>
                {
                    Success = true,
                    Message = StatusEnum.SignedIn.GetDisplayName(),
                    Result = new List<SignInResponse>
                    {
                        new SignInResponse
                        {
                            FirstName = firstname,
                            LastName = lastname,
                            UserItemCategories = itemCategories,
                            UserPropertyCategories = propertyCategories,
                            EmployeeDivisions = employeeDivisions,
                            EmployeeId = employeeId,
                            Role = role,
                            MobileNumber = mobileNumber,
                            EncryptedPassword = encryptedPassword,
                            UserId = id,
                            Token = token
                        }
                    }
                };
            }
            catch
            {
                return new Response<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.Failed.GetDisplayName()
                };
            }
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

            var id = userDb.Id;
            var username = userDb.Username;
            var mobilePhone = userDb.Employee?.Mobile;
            var password = userDb.Password;
            var firstname = userDb.Employee?.FirstName;
            var lastname = userDb.Employee?.LastName;
            var employeeId = userDb.Employee?.Id;
            var role = userDb.Role.ToString();
            itemCategoriesJson = string.IsNullOrEmpty(itemCategoriesJson) ? "[]" : itemCategoriesJson;
            propertyCategoriesJson = string.IsNullOrEmpty(propertyCategoriesJson) ? "[]" : propertyCategoriesJson;
            employeeDivisionsJson = string.IsNullOrEmpty(employeeDivisionsJson) ? "[]" : employeeDivisionsJson;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.MobilePhone, mobilePhone),
                new Claim(ClaimTypes.Hash,password),
                new Claim("FirstName", firstname),
                new Claim("LastName", lastname),
                new Claim("EmployeeId", employeeId),
                new Claim("ItemCategories",itemCategoriesJson),
                new Claim("PropertyCategories",propertyCategoriesJson),
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