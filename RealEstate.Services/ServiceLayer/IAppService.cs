using EFSecondLevelCache.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Api;
using RealEstate.Services.ViewModels.Api.Request;
using RealEstate.Services.ViewModels.Api.Response;
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
    public interface IAppService
    {
        Task<ResponseWrapper<SignInResponse>> SignInAsync(SignInRequest model);

        Task<ResponseWrapper<ConfigResponse>> ConfigAsync();

        Task<ResponseWrapper<PaginatedResponse<ReminderResponse>>> RemindersAsync(ReminderRequest model);

        Task<ResponsesWrapper<ZoonkanResponse>> ZoonkansAsync();

        Task<ResponseWrapper<PaginatedResponse<ItemResponse>>> ItemListAsync(ItemRequest model);
    }

    public class AppService : IAppService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IUserService _userService;
        private readonly IReminderService _reminderService;
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IHttpContextAccessor _accessor;

        private readonly DbSet<User> _users;
        private readonly DbSet<Item> _items;
        private HttpContext HttpContext => _accessor.HttpContext;
        private string Token => HttpContext.Request.Headers["RealEstate-Token"];

        private VersionCheck Version
        {
            get
            {
                try
                {
                    var vrs = HttpContext.Request.Headers["RealEstate-Version"].ToString();
                    if (string.IsNullOrEmpty(vrs))
                        return new VersionCheck(0, 0, 0);

                    var version = vrs.Split(".");
                    return new VersionCheck(int.Parse(version[0]), int.Parse(version[1]), int.Parse(version[2]));
                }
                catch
                {
                    return new VersionCheck(0, 0, 0);
                }
            }
        }

        private string Os => HttpContext.Request.Headers["RealEstate-OS"];
        private string Device => HttpContext.Request.Headers["RealEstate-Device"];

        public AppService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IUserService userService,
            IReminderService reminderService,
            IItemService itemService,
            IHttpContextAccessor accessor,
            IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _itemService = itemService;
            _userService = userService;
            _reminderService = reminderService;
            _localizer = localizer;
            _accessor = accessor;
            _users = _unitOfWork.Set<User>();
            _items = _unitOfWork.Set<Item>();
        }

        public async Task<ResponsesWrapper<ZoonkanResponse>> ZoonkansAsync()
        {
            var tokenState = await TokenCheckAsync(Token);
            if (!tokenState.Success)
                return TokenCheckToResponses<ZoonkanResponse>(tokenState);

            var zoos = await _itemService.ZoonkansAsync();
            if (zoos?.Any() != true)
                return new ResponsesWrapper<ZoonkanResponse>
                {
                    Success = true,
                    Message = _localizer[SharedResource.NoItemToShow],
                };

            var result = zoos.Select(x => new ZoonkanResponse
            {
                PropertyCategory = x.PropertyCategory,
                Picture = x.Picture,
                Count = x.Count,
                ItemCategory = x.ItemCategory
            }).ToList();
            return new ResponsesWrapper<ZoonkanResponse>
            {
                Message = StatusEnum.Success.GetDisplayName(),
                Success = true,
                Result = result.ToList(),
            };
        }

        public async Task<ResponseWrapper<PaginatedResponse<ReminderResponse>>> RemindersAsync(ReminderRequest model)
        {
            var tokenState = await TokenCheckAsync(Token);
            if (!tokenState.Success)
                return TokenCheckToResponse<PaginatedResponse<ReminderResponse>>(tokenState);

            var searchModel = new ReminderSearchViewModel
            {
                PageNo = model.Page,
            };

            var paginatedResult = await _reminderService.ReminderListAsync(searchModel, tokenState.User.UserId);
            var result = (from reminder in paginatedResult.Items
                          select new ReminderResponse()).ToList();

            var response = new PaginatedResponse<ReminderResponse>
            {
                Items = result,
                Pages = paginatedResult.Pages,
                PageNumber = paginatedResult.CurrentPage
            };
            return new ResponseWrapper<PaginatedResponse<ReminderResponse>>
            {
                Message = StatusEnum.Success.GetDisplayName(),
                Success = true,
                Result = response,
            };
        }

        public async Task<ResponseWrapper<PaginatedResponse<ItemResponse>>> ItemListAsync(ItemRequest model)
        {
            var (major, minor) = Version;
            switch (major)
            {
                case 0:
                    return new ResponseWrapper<PaginatedResponse<ItemResponse>>
                    {
                        Success = false,
                        Message = "برای استفاده از این بخش، اپلیکیشن خود را بروزرسانی کنید",
                    };

                case 1 when minor == 0:
                default:
                    break;
            }

            var tokenState = await TokenCheckAsync(Token);
            if (!tokenState.Success)
                return TokenCheckToResponse<PaginatedResponse<ItemResponse>>(tokenState);

            var searchModel = new ItemSearchViewModel
            {
                PageNo = model.Page
            };
            var paginatedResult = await _itemService.ItemListAsync(searchModel, null, tokenState.User.UserId);
            if (paginatedResult?.Items?.Any() != true)
                return new ResponseWrapper<PaginatedResponse<ItemResponse>>
                {
                    Message = _localizer[SharedResource.NoItemToShow],
                    Success = true,
                };

            var result = (from item in paginatedResult.Items
                          let property = item.Property
                          where property != null
                          select new ItemResponse
                          {
                              Property = new PropertyResponse
                              {
                                  Address = property.Address,
                                  Category = property.Category?.Name,
                                  District = property.District?.Name,
                                  Facilities = property.PropertyFacilities?.Select(x => x.Facility?.Name).ToList(),
                                  Ownership = property.CurrentPropertyOwnership?.Ownership != null
                                      ? new OwnershipResponse
                                      {
                                          Name = property.CurrentPropertyOwnership?.Ownership.Customer?.Name,
                                          Mobile = property.CurrentPropertyOwnership?.Ownership.Customer?.Mobile,
                                      }
                                      : default,
                                  Features = property.PropertyFeatures?.Select(x => new FeatureResponse
                                  {
                                      Name = x.Feature?.Name,
                                      Value = x.Value
                                  }).ToList(),
                                  Id = property.Id,
                                  Pictures = property.Pictures?.Select(x => x.File).ToList()
                              },
                              Category = item.Category?.Name,
                              Features = item.ItemFeatures?.Select(x => new FeatureResponse
                              {
                                  Name = x.Feature?.Name,
                                  Value = x.Value
                              }).ToList(),
                              Id = item.Id,
                              Description = item.Description,
                              IsNegotiable = item.IsNegotiable
                          }).ToList();

            var response = new PaginatedResponse<ItemResponse>
            {
                Items = result,
                Pages = paginatedResult.Pages,
                PageNumber = paginatedResult.CurrentPage
            };
            return new ResponseWrapper<PaginatedResponse<ItemResponse>>
            {
                Message = StatusEnum.Success.GetDisplayName(),
                Success = true,
                Result = response,
            };
        }

        private ResponsesWrapper<T> TokenCheckToResponses<T>(ResponseStatus model) where T : class
        {
            if (model == null)
                return new ResponsesWrapper<T>
                {
                    Success = false,
                    Message = StatusEnum.Failed.GetDisplayName()
                };

            var result = new ResponsesWrapper<T>
            {
                Success = model.Success,
                Message = model.Message
            };
            return result;
        }

        private ResponseWrapper<T> TokenCheckToResponse<T>(ResponseStatus model) where T : class
        {
            if (model == null)
                return new ResponseWrapper<T>
                {
                    Success = false,
                    Message = StatusEnum.Failed.GetDisplayName()
                };

            var result = new ResponseWrapper<T>
            {
                Success = model.Success,
                Message = model.Message
            };
            return result;
        }

        public async Task<ResponseWrapper<ConfigResponse>> ConfigAsync()
        {
            var tokenState = await TokenCheckAsync(Token);
            if (!tokenState.Success)
                return TokenCheckToResponse<ConfigResponse>(tokenState);

            var user = await _users
                .AsNoTracking()
                .Include(x => x.Employee.EmployeeDivisions).ThenInclude(x => x.Division)
                .Include(x => x.UserItemCategories).ThenInclude(x => x.Category)
                .Include(x => x.UserPropertyCategories).ThenInclude(x => x.Category)
                .Where(x => x.Id == tokenState.User.UserId)
                .Select(x => new
                {
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
                }).FirstOrDefaultAsync();

            return new ResponseWrapper<ConfigResponse>
            {
                Success = true,
                Message = StatusEnum.Success.GetDisplayName(),
                Result = new ConfigResponse
                {
                    Role = tokenState.User.Role,
                    FirstName = tokenState.User.FirstName,
                    LastName = tokenState.User.LastName,
                    MobileNumber = tokenState.User.MobileNumber,
                    UserId = tokenState.User.UserId,
                    Username = tokenState.User.Username,
                    UserItemCategories = user.UserItemCategories.Select(x => x.Name).ToList(),
                    UserPropertyCategories = user.UserPropertyCategories.Select(x => x.Name).ToList(),
                    EmployeeDivisions = user.EmployeeDivisions.Select(x => x.Name).ToList()
                }
            };
        }

        private async Task<TokenCheck> TokenCheckAsync(string token)
        {
            var result = new TokenCheck
            {
                Message = StatusEnum.Forbidden.GetDisplayName(),
                Success = false
            };

            if (string.IsNullOrEmpty(token))
                return result;

            List<Claim> claims;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!(handler.ReadToken(token) is JwtSecurityToken jwtSecurityToken))
                    return result;
                claims = jwtSecurityToken.Claims.ToList();
                if (claims?.Any() != true)
                    return result;
            }
            catch
            {
                return result;
            }

            var id = claims.FirstOrDefault(x => x.Type.Equals("Id", StringComparison.CurrentCulture))?.Value;
            var userName = claims.FirstOrDefault(x => x.Type.Equals("Username", StringComparison.CurrentCulture))?.Value;
            var password = claims.FirstOrDefault(x => x.Type.Equals("Password", StringComparison.CurrentCulture))?.Value;

            if (string.IsNullOrEmpty(id)
                || string.IsNullOrEmpty(userName)
                || string.IsNullOrEmpty(password))
                return result;

            var user = await _users
                .AsNoTracking()
                .Include(x => x.Employee)
                .Where(x =>
                    x.Id == id
                    && x.Username == userName
                    && x.Password == password)
                .Cacheable()
                .Select(x => new
                {
                    x.Id,
                    x.Username,
                    x.Password,
                    x.Employee.FirstName,
                    x.Employee.LastName,
                    x.Role,
                    x.Employee.Mobile
                })
                .FirstOrDefaultAsync();
            if (user == null)
                return new TokenCheck
                {
                    Message = StatusEnum.UserNotFound.GetDisplayName(),
                    Success = false,
                };

            return new TokenCheck
            {
                Message = StatusEnum.Success.GetDisplayName(),
                Success = true,
                User = new UserResponse
                {
                    Role = user.Role.GetDisplayName(),
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    MobileNumber = user.Mobile,
                    UserId = user.Id,
                    Username = user.Username
                }
            };
        }

        public async Task<ResponseWrapper<SignInResponse>> SignInAsync(SignInRequest model)
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
                return new ResponseWrapper<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.UserNotFound.GetDisplayName(),
                };
            try
            {
                var decryptedPasswordInDb = userDb.Password.Cipher(CryptologyExtension.CypherMode.Decryption);
                if (decryptedPasswordInDb != model.Password)
                    return new ResponseWrapper<SignInResponse>
                    {
                        Success = false,
                        Message = StatusEnum.WrongPassword.GetDisplayName(),
                    };
            }
            catch
            {
                return new ResponseWrapper<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.WrongPassword.GetDisplayName(),
                };
            }

            if (userDb.Audits.LastOrDefault()?.Type == LogTypeEnum.Delete)
                return new ResponseWrapper<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.Deactivated.GetDisplayName(),
                };

            var itemCategories = userDb.UserItemCategories.Select(x => x.Name).ToList();
            var employeeDivisions = userDb.EmployeeDivisions.Select(x => x.Name).ToList();

            var id = userDb.Id;
            var mobileNumber = userDb.MobileNumber;
            var encryptedPassword = userDb.Password;
            var firstname = userDb.FirstName;
            var lastname = userDb.LastName;
            var employeeId = userDb.Id;
            var role = userDb.Role.GetDisplayName();

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
                return new ResponseWrapper<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.TokenIsNull.GetDisplayName(),
                };

            try
            {
                return new ResponseWrapper<SignInResponse>
                {
                    Success = true,
                    Message = StatusEnum.SignedIn.GetDisplayName(),
                    Result = new SignInResponse
                    {
                        Token = token
                    }
                };
            }
            catch
            {
                return new ResponseWrapper<SignInResponse>
                {
                    Success = false,
                    Message = StatusEnum.Failed.GetDisplayName()
                };
            }
        }
    }
}