using EFSecondLevelCache.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Base;
using RealEstate.Base.Api;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels.Api.Request;
using RealEstate.Services.ViewModels.Api.Response;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IAppService
    {
        Task<ResponseWrapper<SignInResponse>> SignInAsync(SignInRequest model);

        Task<ResponseWrapper<ConfigResponse>> ConfigAsync(string userId);

        Task<ResponseWrapper<PaginatedResponse<ReminderResponse>>> RemindersAsync(ReminderRequest model, string userId);

        Task<ResponsesWrapper<ZoonkanResponse>> ZoonkansAsync();

        Task<UserResponse> FindUserAsync(string id, string userName, string password);

        Task<ResponseWrapper<PaginatedResponse<ItemResponse>>> ItemListAsync(ItemRequest model, string userId);
    }

    public class AppService : IAppService
    {
        private readonly IReminderService _reminderService;
        private readonly IItemService _itemService;

        private readonly IStringLocalizer<SharedResource> _localizer;

        private readonly DbSet<User> _users;

        private readonly string _baseUrl;

        public AppService(
            IUnitOfWork unitOfWork,
            IReminderService reminderService,
            IItemService itemService,
            IHttpContextAccessor accessor,
            IStringLocalizer<SharedResource> localizer)
        {
            _itemService = itemService;
            _reminderService = reminderService;

            _localizer = localizer;

            _users = unitOfWork.Set<User>();

            var httpContext = accessor.HttpContext;
            _baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Host}:{httpContext.Request.Host.Port}/";
        }

        public async Task<ResponsesWrapper<ZoonkanResponse>> ZoonkansAsync()
        {
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

        public async Task<ResponseWrapper<PaginatedResponse<ReminderResponse>>> RemindersAsync(ReminderRequest model, string userId)
        {
            var searchModel = new ReminderSearchViewModel
            {
                PageNo = model.Page,
            };

            var paginatedResult = await _reminderService.ReminderListAsync(searchModel, userId);
            var result = (from reminder in paginatedResult.Items
                          select new ReminderResponse
                          {
                              CheckBank = reminder.CheckBank,
                              CheckNumber = reminder.CheckNumber,
                              Date = reminder.Date.ToUnixTimestamp(),
                              Price = (long)reminder.Price,
                              Subject = reminder.Description,
                              Pictures = reminder.Pictures?.Select(x => Path.Combine(_baseUrl, $"img/{x.File}")).ToList()
                          }).ToList();

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

        public async Task<ResponseWrapper<PaginatedResponse<ItemResponse>>> ItemListAsync(ItemRequest model, string userId)
        {
            var searchModel = new ItemSearchViewModel
            {
                PageNo = model.Page
            };
            var paginatedResult = await _itemService.ItemListAsync(searchModel, null, userId);
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
                                  Pictures = property.Pictures?.Select(x => Path.Combine(_baseUrl, $"img/{x.File}")).ToList()
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

        public async Task<ResponseWrapper<ConfigResponse>> ConfigAsync(string userId)
        {
            var user = await _users
                .AsNoTracking()
                .Include(x => x.Employee.EmployeeDivisions).ThenInclude(x => x.Division)
                .Include(x => x.UserItemCategories).ThenInclude(x => x.Category)
                .Include(x => x.UserPropertyCategories).ThenInclude(x => x.Category)
                .Where(x => x.Id == userId)
                .Select(x => new
                {
                    x.Role,
                    x.Employee.FirstName,
                    x.Employee.LastName,
                    x.Employee.Mobile,
                    x.Username,
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
                    Role = user.Role.GetDisplayName(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MobileNumber = user.Mobile,
                    UserId = userId,
                    Username = user.Username,
                    UserItemCategories = user.UserItemCategories.Select(x => x.Name).ToList(),
                    UserPropertyCategories = user.UserPropertyCategories.Select(x => x.Name).ToList(),
                    EmployeeDivisions = user.EmployeeDivisions.Select(x => x.Name).ToList()
                }
            };
        }

        public async Task<UserResponse> FindUserAsync(string id, string userName, string password)
        {
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
                return default;

            var userModel = new UserResponse
            {
                Role = user.Role.GetDisplayName(),
                LastName = user.LastName,
                FirstName = user.FirstName,
                MobileNumber = user.Mobile,
                UserId = user.Id,
                Username = user.Username
            };
            return userModel;
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
                    x.Id,
                    x.Password,
                    x.Role,
                    x.Audits,
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

            var id = userDb.Id;
            var encryptedPassword = userDb.Password;

            UserViewModel template;
            var claims = new List<Claim>
            {
                new Claim(nameof(template.Id), id),
                new Claim(nameof(template.Username), model.Username),
                new Claim(nameof(template.Password), encryptedPassword),
            };
            var identity = new ClaimsIdentity(claims, AuthenticationScheme.Scheme);

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