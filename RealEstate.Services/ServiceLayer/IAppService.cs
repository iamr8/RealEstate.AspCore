using EFSecondLevelCache.Core;
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
using RealEstate.Services.ViewModels.ModelBind;
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

        Task<ResponseWrapper<ConfigResponse>> ConfigAsync(RequestWrapper model);

        Task<ResponseWrapper<ItemResponse>> ItemListAsync(ItemRequest model);
    }

    public class AppService : IAppService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IUserService _userService;
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly DbSet<User> _users;
        private readonly DbSet<Item> _items;

        public AppService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IUserService userService,
            IItemService itemService,
            IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _itemService = itemService;
            _userService = userService;
            _localizer = localizer;
            _users = _unitOfWork.Set<User>();
            _items = _unitOfWork.Set<Item>();
        }

        public async Task<ResponseWrapper<ItemResponse>> ItemListAsync(ItemRequest model)
        {
            var tokenState = await TokenCheckAsync(model);
            if (!tokenState.Success)
                return TokenCheckToResponse<ItemResponse>(tokenState);

            if (string.IsNullOrEmpty(model.ItemCategory))
                return new ResponseWrapper<ItemResponse>
                {
                    Success = false,
                    Message = "دسته بندی مورد را پر کنید",
                };

            if (string.IsNullOrEmpty(model.PropertyCategory))
                return new ResponseWrapper<ItemResponse>
                {
                    Success = false,
                    Message = "دسته بندی ملک را پر کنید",
                };

            var query = _items.AsQueryable();
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.Category);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.District);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.Pictures);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.PropertyFacilities)
                .ThenInclude(x => x.Facility);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.PropertyFeatures)
                .ThenInclude(x => x.Feature);
            query = query.Include(x => x.Property)
                .ThenInclude(x => x.PropertyOwnerships)
                .ThenInclude(x => x.Ownerships)
                .ThenInclude(x => x.Customer);
            query = query.Include(x => x.Category);
            query = query.Include(x => x.DealRequests);
            query = query.Include(x => x.ItemFeatures)
                .ThenInclude(x => x.Feature);
            query = query.Include(x => x.Applicants)
                .ThenInclude(x => x.Customer);

            query = query.Where(x => x.Category.Name == model.ItemCategory);
            query = query.Where(x => x.Property.Category.Name == model.PropertyCategory);
            query = query.Where(x => x.Category.UserItemCategories.Any(c => c.UserId == tokenState.User.UserId));
            query = query.Where(x => x.Property.Category.UserPropertyCategories.Any(c => c.UserId == tokenState.User.UserId));

            var items = await query.Cacheable().ToListAsync();
            if (items?.Any() != true)
                return new ResponseWrapper<ItemResponse>
                {
                    Message = _localizer[SharedResource.NoItemToShow],
                    Success = true,
                };

            var result = from item in items
                         let converted = item.Map<ItemViewModel>(ent =>
                         {
                             ent.IncludeAs<Item, Property, PropertyViewModel>(_unitOfWork, x => x.Property, ent2 =>
                             {
                                 ent2.IncludeAs<Property, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                                 ent2.IncludeAs<Property, District, DistrictViewModel>(_unitOfWork, x => x.District);
                                 ent2.IncludeAs<Property, PropertyFacility, PropertyFacilityViewModel>(_unitOfWork, x => x.PropertyFacilities,
                                     ent3 => ent3.IncludeAs<PropertyFacility, Facility, FacilityViewModel>(_unitOfWork, x => x.Facility));
                                 ent2.IncludeAs<Property, PropertyFeature, PropertyFeatureViewModel>(_unitOfWork, x => x.PropertyFeatures,
                                     ent3 => ent3.IncludeAs<PropertyFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                                 ent2.IncludeAs<Property, PropertyOwnership, PropertyOwnershipViewModel>(_unitOfWork, x => x.PropertyOwnerships,
                                     ent3 => ent3.IncludeAs<PropertyOwnership, Ownership, OwnershipViewModel>(_unitOfWork, x => x.Ownerships,
                                         ent4 => ent4.IncludeAs<Ownership, Customer, CustomerViewModel>(_unitOfWork, x => x.Customer)));
                             });
                             ent.IncludeAs<Item, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
                             ent.IncludeAs<Item, ItemFeature, ItemFeatureViewModel>(_unitOfWork, x => x.ItemFeatures,
                                 ent2 => ent2.IncludeAs<ItemFeature, Feature, FeatureViewModel>(_unitOfWork, x => x.Feature));
                         })
                         where converted != null
                         let property = converted.Property
                         where property != null
                         select new ItemResponse
                         {
                             Property = new PropertyResponse
                             {
                                 Address = property.Address,
                                 Category = property.Category?.Name,
                                 District = property.District?.Name,
                                 Facilities = property.PropertyFacilities?.Select(x => x.Facility?.Name).ToList(),
                                 Ownership = property.CurrentPropertyOwnership?.Ownership != null ?
                             new OwnershipResponse
                             {
                                 Name = property.CurrentPropertyOwnership?.Ownership.Customer?.Name,
                                 Id = property.CurrentPropertyOwnership?.Ownership.Customer?.Id,
                                 Mobile = property.CurrentPropertyOwnership?.Ownership.Customer?.Mobile
                             } :
                             default,
                                 Features = property.PropertyFeatures?.ToDictionary(x => x.Feature?.Name, x => x.Value),
                             },
                             Category = converted.Category?.Name,
                             Features = converted.ItemFeatures?.ToDictionary(x => x.Feature?.Name, x => x.Value.CurrencyToWords())
                         };

            return new ResponseWrapper<ItemResponse>
            {
                Message = StatusEnum.Success.GetDisplayName(),
                Success = true,
                Result = result.ToList(),
            };
        }

        private ResponseWrapper<T> TokenCheckToResponse<T>(ResponseWrapper model) where T : class
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

        public async Task<ResponseWrapper<ConfigResponse>> ConfigAsync(RequestWrapper model)
        {
            var tokenState = await TokenCheckAsync(model);
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
                Result = new List<ConfigResponse>
                {
                    new ConfigResponse
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
                }
            };
        }

        private async Task<TokenCheck> TokenCheckAsync<T>(T model) where T : RequestWrapper
        {
            if (model == null)
                return new TokenCheck
                {
                    Success = false,
                    Message = StatusEnum.Failed.GetDisplayName(),
                };

            var result = new TokenCheck
            {
                Message = StatusEnum.Forbidden.GetDisplayName(),
                Success = false
            };

            var token = model.Token;
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
            var propertyCategories = userDb.UserPropertyCategories.Select(x => x.Name).ToList();
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
                    Result = new List<SignInResponse>
                    {
                        new SignInResponse
                        {
                            Token = token
                        }
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