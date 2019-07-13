using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IGlobalService
    {
        Task<List<LogDetailViewModel>> Logs(string entityName, string entityId);

        Task FixPricePerMeterAsync();

        Task FixFinalPriceAsync();

        Task CleanComplexDuplicatesAsync();

        Task FixLoanPriceAsync();

        Task FixBuildYearAsync();

        Task FixAddressAsync();

        Task<List<StatisticsViewModel>> StatisticsAsync(StatisticsSearchViewModel searchModel);
    }

    public class GlobalService : IGlobalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IBaseService _baseService;
        private readonly DbSet<Item> _items;
        private readonly DbSet<PropertyFeature> _propertyFeatures;
        private readonly DbSet<Property> _properties;
        private readonly DbSet<ItemFeature> _itemFeatures;
        private readonly DbSet<PropertyOwnership> _propertyOwnerships;
        private readonly DbSet<Ownership> _ownerships;
        private readonly DbSet<PropertyFacility> _propertyFacilities;
        private readonly DbSet<Picture> _pictures;
        private readonly DbSet<Customer> _customers;

        public GlobalService(
            IBaseService baseService,
            IUnitOfWork unitOfWork,
            ApplicationDbContext applicationDbContext
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _applicationDbContext = applicationDbContext;
            _items = _unitOfWork.Set<Item>();
            _propertyOwnerships = _unitOfWork.Set<PropertyOwnership>();
            _ownerships = _unitOfWork.Set<Ownership>();
            _propertyFacilities = _unitOfWork.Set<PropertyFacility>();
            _itemFeatures = _unitOfWork.Set<ItemFeature>();
            _customers = _unitOfWork.Set<Customer>();
            _properties = _unitOfWork.Set<Property>();
            _pictures = _unitOfWork.Set<Picture>();
            _propertyFeatures = _unitOfWork.Set<PropertyFeature>();
        }

        private const string FeaturePricePerMeter = "01cb6a1d-959d-4abb-8488-f10ab09bd8a8";
        private const string FeatureLoadPrice = "736ad605-78ea-41e1-bdeb-8d2811db2dec";
        private const string FeatureBuildYear = "cdb97926-b3b1-48ec-bdd6-389a0431007c";
        private const string FeatureFinalPrice = "54a0b920-c17f-4ff2-9c51-f9551159026a";

        private async Task CleanPropertyDuplicatesAsync()
        {
            var groups = await _properties.IgnoreQueryFilters()
               .GroupBy(x => new
               {
                   x.Street,
                   x.CategoryId,
                   x.DistrictId,
                   x.Flat,
                   x.Alley,
                   x.Floor,
                   x.Number,
                   x.BuildingName
               }).Where(x => x.Count() > 1).ToListAsync();
            if (groups?.Any() == true)
            {
                foreach (var groupedProperty in groups)
                {
                    var cnt = groupedProperty.Count();
                    var propertyBest = groupedProperty.Any(x => x.PropertyFacilities.Any() || x.PropertyFeatures.Any())
                        ? groupedProperty.FirstOrDefault(x => x.PropertyFacilities.Any() || x.PropertyFeatures.Any())
                        : groupedProperty.FirstOrDefault();
                    if (propertyBest == null)
                        continue;

                    var validProperties = groupedProperty.Except(new[]
                    {
                        propertyBest
                    }).ToList();
                    foreach (var property in validProperties)
                    {
                        var currentPropertyId = property.Id;

                        var propertyInDb = await _properties.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == currentPropertyId);
                        if (propertyInDb == null)
                            continue;

                        var items = await _items.IgnoreQueryFilters().Where(x => x.PropertyId == currentPropertyId).ToListAsync();
                        if (items?.Any() == true)
                        {
                            foreach (var item in items)
                            {
                                var itemInDb = await _items.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == item.Id);
                                if (itemInDb == null)
                                    continue;

                                var itemFeatures = await _itemFeatures.IgnoreQueryFilters().Where(x => x.ItemId == itemInDb.Id).ToListAsync();
                                if (itemFeatures?.Any() == true)
                                {
                                    foreach (var itemFeature in itemFeatures)
                                    {
                                        var itemFeatureInDb = await _itemFeatures.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == itemFeature.Id);
                                        await _baseService.RemoveAsync(itemFeatureInDb, null, DeleteEnum.Delete, false);
                                    }
                                }

                                await _baseService.RemoveAsync(itemInDb, null, DeleteEnum.Delete, false);
                            }
                        }

                        var propertyOwnerships = await _propertyOwnerships.IgnoreQueryFilters().Where(x => x.PropertyId == propertyInDb.Id).ToListAsync();
                        if (propertyOwnerships?.Any() == true)
                        {
                            foreach (var propertyOwnership in propertyOwnerships)
                            {
                                var ownerships = await _ownerships.IgnoreQueryFilters().Where(x => x.PropertyOwnershipId == propertyOwnership.Id).ToListAsync();
                                if (ownerships?.Any() == true)
                                    foreach (var ownership in ownerships)
                                    {
                                        var ownershipInDb = await _ownerships.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == ownership.Id);
                                        await _baseService.RemoveAsync(ownershipInDb, null, DeleteEnum.Delete, false);
                                    }

                                var propertyOwnershipInDb = await _propertyOwnerships.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == propertyOwnership.Id);
                                await _baseService.RemoveAsync(propertyOwnershipInDb, null, DeleteEnum.Delete, false);
                            }
                        }

                        var propertyFeatures = await _propertyFeatures.IgnoreQueryFilters().Where(x => x.PropertyId == propertyInDb.Id).ToListAsync();
                        if (propertyFeatures?.Any() == true)
                            foreach (var propertyFeature in propertyFeatures)
                            {
                                var propertyFeatureInDb = await _propertyFeatures.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == propertyFeature.Id);
                                await _baseService.RemoveAsync(propertyFeatureInDb, null, DeleteEnum.Delete, false);
                            }

                        var propertyFacilities = await _propertyFacilities.IgnoreQueryFilters().Where(x => x.PropertyId == propertyInDb.Id).ToListAsync();
                        if (propertyFacilities?.Any() == true)
                            foreach (var propertyFacility in propertyFacilities)
                            {
                                var propertyFacilityInDb = await _propertyFacilities.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == propertyFacility.Id);
                                await _baseService.RemoveAsync(propertyFacilityInDb, null, DeleteEnum.Delete, false);
                            }

                        var pictures = await _pictures.IgnoreQueryFilters().Where(x => x.PropertyId == propertyInDb.Id).ToListAsync();
                        if (pictures?.Any() == true)
                            foreach (var picture in pictures)
                            {
                                var pictureInDb = await _pictures.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == picture.Id);
                                await _baseService.RemoveAsync(pictureInDb, null, DeleteEnum.Delete, false);
                            }

                        await _baseService.RemoveAsync(propertyInDb, null, DeleteEnum.Delete, false);
                    }
                }
            }

            await _baseService.SaveChangesAsync();
        }

        public async Task CleanComplexDuplicatesAsync()
        {
            await CleanPropertyDuplicatesAsync();
            await CleanItemDuplicatesAsync();
            await CleanCustomerDuplicatesAsync();
        }

        private async Task CleanCustomerDuplicatesAsync()
        {
            var groups = await _customers.IgnoreQueryFilters()
                .GroupBy(x => new
                {
                    x.MobileNumber
                }).Where(x => x.Count() > 1).ToListAsync();
            if (groups?.Any() != true)
                return;

            foreach (var groupedCustomer in groups)
            {
                var cnt = groupedCustomer.Count();
                var customerBest = groupedCustomer.Any(x => x.Name != "مالک")
                    ? groupedCustomer.FirstOrDefault(x => x.Name != "مالک")
                    : groupedCustomer.FirstOrDefault();
                if (customerBest == null)
                    continue;

                var customers = groupedCustomer.Except(new[]
                {
                    customerBest
                }).ToList();
                var customerId = customerBest.Id;
                foreach (var customer in customers)
                {
                    var ownerships = customer.Ownerships;
                    if (ownerships?.Any() == true)
                    {
                        foreach (var ownership in ownerships)
                        {
                            var ownershipInDb = await _ownerships.FirstOrDefaultAsync(x => x.Id == ownership.Id);
                            await _baseService.UpdateAsync(ownershipInDb,
                                currentUser =>
                                {
                                    ownershipInDb.CustomerId = customerId;
                                }, null, false, StatusEnum.OwnershipIsNull);
                        }
                    }

                    var customerInDb = await _customers.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == customer.Id);
                    if (customerInDb == null)
                        continue;

                    await _baseService.RemoveAsync(customerInDb, null, DeleteEnum.Delete, false);
                }
            }

            await _baseService.SaveChangesAsync();
        }

        private async Task CleanItemDuplicatesAsync()
        {
            var groups = await _items.IgnoreQueryFilters()
                .Include(x => x.Property)
                .GroupBy(x => new
                {
                    x.CategoryId,
                    x.Property
                }).Where(x => x.Count() > 1).ToListAsync();
            if (groups?.Any() == true)
            {
                foreach (var groupedItem in groups)
                {
                    foreach (var item in groupedItem.Skip(1))
                    {
                        var itemInDb = await _items.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == item.Id);
                        if (itemInDb == null)
                            continue;

                        var itemFeatures = await _itemFeatures.Where(x => x.ItemId == itemInDb.Id).ToListAsync();
                        if (itemFeatures?.Any() == true)
                        {
                            foreach (var itemFeature in itemFeatures)
                            {
                                var itemFeatureInDb = await _itemFeatures.FirstOrDefaultAsync(x => x.Id == itemFeature.Id);
                                await _baseService.RemoveAsync(itemFeatureInDb, null, DeleteEnum.Delete, false);
                            }
                        }

                        await _baseService.RemoveAsync(itemInDb, null, DeleteEnum.Delete, false);
                    }
                }
            }

            await _baseService.SaveChangesAsync();
        }

        private static List<StatisticsDetailViewModel> Map(IReadOnlyCollection<ItemViewModel> item)
        {
            if (item?.Any() != true)
                return default;

            var result = item.Select(x => new StatisticsDetailViewModel
            {
                ItemId = x.Id,
                ItemCategory = x.Category?.Name,
                PropertyCategory = x.Property?.Category?.Name,
                UserFullName = x.Logs.Create.UserFullName,
                UserId = x.Logs.Create.UserId
            }).ToList();
            return result;
        }

        private List<LogJsonEntity> GetAudits(IReflect entityType, string entityId, List<object> model)
        {
            if (model?.Any() != true)
                return default;

            var result = new List<LogJsonEntity>();
            foreach (var row in model)
            {
                var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                if (properties?.Any() != true)
                    continue;

                Applicant testModel;
                var idProperty = row.GetProperty(nameof(testModel.Id));
                if (idProperty == null || !(idProperty.GetValue(row) is string id))
                    continue;

                if (string.IsNullOrEmpty(id) || id != entityId)
                    continue;

                var auditProperty = row.GetProperty(nameof(testModel.Audit));
                if (auditProperty == null || !(auditProperty.GetValue(row) is string audit))
                    continue;

                if (string.IsNullOrEmpty(audit))
                    continue;

                var audits = audit.JsonGetAccessor<LogJsonEntity>();
                if (audits?.Any() != true)
                    continue;

                result = audits;
            }

            return result;
        }

        public static string CleanNumberDividers(string num)
        {
            var featureValue = num.Replace(",", "").Replace("/", "").Replace(".", "").Replace("،", "");
            return featureValue;
        }

        public static string FixLoanPrice(string value)
        {
            value = CleanNumberDividers(value);
            if (!long.TryParse(value, out var loan))
                return value;

            var num = int.Parse(value.Split('0')[0]);
            var loanNormalized = loan > 999999999
                ? $"{num:#########}"
                : loan < 15000
                    ? $"{num:######}"
                    : value;

            return loanNormalized;
        }

        public static string FixPricePerMeter(string value, bool isStore)
        {
            value = value.CleanNumberDividers();
            if (!long.TryParse(value, out var pricePerMeter))
                return value;

            string normalized;
            if (isStore)
            {
                // 550 000 000 : 9
                // 55 000 000 : 8
                // 5 500 000 : 7
                // 5 : <= 2
                if (pricePerMeter <= 10000000)
                    pricePerMeter *= 10;

                if (value.Length == 9)
                    pricePerMeter /= 10;
                else if (value.Length <= 2)
                    pricePerMeter *= 1000000;

                normalized = pricePerMeter.ToString();
            }
            else
            {
                // 550 000 000 : 9
                // 55 000 000 : 8
                // 5 500 000 : 7
                // 5 : <= 2

                if (pricePerMeter >= 20000000)
                    pricePerMeter /= 10;

                value = pricePerMeter.ToString();

                if (value.Length == 9)
                    pricePerMeter /= 10;
                else if (value.Length <= 2)
                    pricePerMeter *= 1000000;

                normalized = pricePerMeter.ToString();
            }

            return normalized;
        }

        public static string FixFinalPrice(string value)
        {
            value = CleanNumberDividers(value);
            if (!long.TryParse(value, out var finalPrice))
                return value;

            var finalPriceNormalized = value.PadRight(finalPrice > 1000000000 ? 10 : 9, '0');
            finalPrice = long.TryParse(finalPriceNormalized, out var finalPriceTemp) && finalPriceTemp > 3000000000
                ? finalPriceTemp / 10
                : finalPriceTemp;
            finalPriceNormalized = finalPrice.ToString();

            return finalPriceNormalized;
        }

        public async Task FixLoanPriceAsync()
        {
            var itemFeatures = await _itemFeatures
                .Include(x => x.Feature)
                .Where(x => x.FeatureId == FeatureLoadPrice)
                .ToListAsync();
            if (itemFeatures?.Any() != true)
                return;

            var updatedIndicator = 0;
            foreach (var itemFeature in itemFeatures)
            {
                var normalized = FixLoanPrice(itemFeature.Value);
                if (normalized.Equals(itemFeature.Value, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                var (updateStatus, updatedPf) = await _baseService.UpdateAsync(itemFeature,
                    currentUser => itemFeature.Value = normalized.ToString(),
                    null, false, StatusEnum.Failed);
                if (updateStatus == StatusEnum.Success)
                    updatedIndicator++;
            }

            if (updatedIndicator > 0)
                await _baseService.SaveChangesAsync();
        }

        public async Task FixPricePerMeterAsync()
        {
            var itemFeatures = await _itemFeatures
                .Include(x => x.Feature)
                .Where(x => x.FeatureId == FeaturePricePerMeter)
                .ToListAsync();
            if (itemFeatures?.Any() != true)
                return;

            var updatedIndicator = 0;
            foreach (var itemFeature in itemFeatures)
            {
                var value = itemFeature.Value.CleanNumberDividers();
                if (!long.TryParse(value, out var num))
                    continue;

                var isStore = itemFeature.Item.Property.CategoryId == "29dfac27-765c-4839-b77b-0afc7b6450e3";
                string normalized;
                var featureId = string.Empty;
                if (isStore)
                {
                    normalized = FixPricePerMeter(value, true);
                }
                else
                {
                    if (num >= 100000000)
                    {
                        normalized = FixFinalPrice(value);
                        featureId = FeatureFinalPrice;
                    }
                    else
                    {
                        normalized = FixPricePerMeter(value, false);
                    }
                }

                if (normalized.Equals(itemFeature.Value, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                var (updateStatus, updatedPf) = await _baseService.UpdateAsync(itemFeature,
                    currentUser =>
                    {
                        if (!string.IsNullOrEmpty(featureId))
                            itemFeature.FeatureId = featureId;

                        itemFeature.Value = normalized;
                    },
                    null, false, StatusEnum.Failed);
                if (updateStatus == StatusEnum.Success)
                    updatedIndicator++;
            }

            if (updatedIndicator > 0)
                await _baseService.SaveChangesAsync();
        }

        public async Task FixFinalPriceAsync()
        {
            var itemFeatures = await _itemFeatures
                .Include(x => x.Feature)
                .Where(x => x.FeatureId == FeatureFinalPrice)
                .ToListAsync();
            if (itemFeatures?.Any() != true)
                return;

            var updatedIndicator = 0;
            foreach (var itemFeature in itemFeatures)
            {
                var normalized = FixFinalPrice(itemFeature.Value);
                if (normalized.Equals(itemFeature.Value, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                var (updateStatus, updatedPf) = await _baseService.UpdateAsync(itemFeature,
                    currentUser => itemFeature.Value = normalized.ToString(),
                    null, false, StatusEnum.Failed);
                if (updateStatus == StatusEnum.Success)
                    updatedIndicator++;
            }

            if (updatedIndicator > 0)
                await _baseService.SaveChangesAsync();
        }

        public async Task FixAddressAsync()
        {
            var properties = await _properties.ToListAsync();
            if (properties?.Any() != true)
                return;

            var updatedIndicator = 0;
            foreach (var property in properties)
            {
                var normalized = FixAddress(property.Street);

                var (updateStatus, updatedPf) = await _baseService.UpdateAsync(property,
                    currentUser => property.Street = normalized,
                    null, false, StatusEnum.PropertyIsNull);
                if (updateStatus == StatusEnum.Success)
                    updatedIndicator++;
            }

            if (updatedIndicator > 0)
                await _baseService.SaveChangesAsync();
        }

        public static string FixAddress(string street)
        {
            if (string.IsNullOrEmpty(street))
                return street;

            var stPrefixed = new[]
            {
                "خیابان خ", "خیابان  خ", "خ"
            };
            var prefixes = new[]
            {
                "خیابان", "اتوبان", "سمت", "کوی", "بلوار", "پاساژ", "نبش", "فاز", "روبروی", "سه راه", "بازارچه"
            };
            if (string.IsNullOrEmpty(street))
                return default;

            var finalStreet = street.Trim();
            finalStreet = finalStreet.Replace(new[]
            {
                "روبه روی ", "روبه رو ", "روبه رویه "
            }, "روبروی ");
            finalStreet = finalStreet.Replace(" بارک ", " پارک ");
            finalStreet = finalStreet.Replace(" سراه ", " سه راه ");

            var concatPrefixes = prefixes.Concat(stPrefixed).ToList();
            if (concatPrefixes?.Any(x => finalStreet.StartsWith($"{x} ")) == true)
            {
                foreach (var prefix in concatPrefixes)
                {
                    var term = $"{prefix} ";
                    var timmedStreet = finalStreet.Trim();
                    if (!timmedStreet.StartsWith(term, StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    var pref = stPrefixed.Any(x => x == prefix)
                        ? "خیابان "
                        : term;
                    var tempStreet = $"{pref}{timmedStreet.Split(term)[1]}";
                    finalStreet = tempStreet;
                }
            }
            else
            {
                finalStreet = $"خیابان {finalStreet}";
            }

            finalStreet = finalStreet.Replace(" خ ", " خیابان ");
            return finalStreet.FixPersian();
        }

        public static string FixBuildYear(string value)
        {
            var featureValue = value;
            try
            {
                if (value.Contains(".", StringComparison.CurrentCulture) && !value.StartsWith(".", StringComparison.CurrentCulture))
                    featureValue = featureValue.Split(".")[0];
            }
            catch
            {
                // ignored
            }

            if (!int.TryParse(featureValue, out var year))
                return value;

            var normalizedYear = year < 100
                ? year <= 30
                    ? new PersianCalendar().GetYear(DateTime.Now) - year
                    : int.Parse($"13{year}")
                : year;

            return normalizedYear.ToString();
        }

        public async Task FixBuildYearAsync()
        {
            var propertyFeatures = await _propertyFeatures
                .Include(x => x.Feature)
                .Where(x => x.FeatureId == FeatureBuildYear
                            && x.Value.Length != 4)
                .ToListAsync();
            if (propertyFeatures?.Any() != true)
                return;

            var updatedIndicator = 0;
            foreach (var propertyFeature in propertyFeatures)
            {
                var normalized = FixBuildYear(propertyFeature.Value);
                if (normalized.Equals(propertyFeature.Value, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                var (updateStatus, updatedPf) = await _baseService.UpdateAsync(propertyFeature,
                    currentUser => propertyFeature.Value = normalized.ToString(),
                    null, false, StatusEnum.Failed);
                if (updateStatus == StatusEnum.Success)
                    updatedIndicator++;
            }

            if (updatedIndicator > 0)
                await _baseService.SaveChangesAsync();
        }

        public async Task<List<LogDetailViewModel>> Logs(string entityName, string entityId)
        {
            if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entityId))
                return default;

            var query = _applicationDbContext.AsQueryable(entityName);
            var type = query.ElementType;
            var model = await query.ToListAsync().ConfigureAwait(false);
            var result = GetAudits(type, entityId, model);
            if (result?.Any() != true)
                return default;

            var audits = result.Select(x => x.MapLog()).Reverse().ToList();
            return audits;
        }

        public async Task<List<StatisticsViewModel>> StatisticsAsync(StatisticsSearchViewModel searchModel)
        {
            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return default;

            var query = _items
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(x => x.Property)
                .ThenInclude(x => x.Category)
                .Include(x => x.Category)
                .AsQueryable();

            if (currentUser.Role != Role.SuperAdmin && currentUser.Role != Role.Admin)
                query = query.Where(entity => CustomDbFunctions.JsonValue(entity.Audit, "$[0].i") == currentUser.Id);

            if (searchModel != null)
                query = _baseService.AdminSeachConditions(query, searchModel);

            var result = new List<StatisticsViewModel>();

            var dateTimeToday = DateTime.Today.Date;
            var dateTimeWeek = DateTime.Today.AddDays(-7);
            var dateTimeMonth = DateTime.Today.AddMonths(-1);
            var dateTimeMonthString = dateTimeMonth.ToString("yyyy/MM/dd", new CultureInfo("en-US"));

            var monthQuery = await query
                .Where(x => CustomDbFunctions.DateDiff("DAY", dateTimeMonthString, CustomDbFunctions.JsonValue(x.Audit, "$[0].d")) <= 0)
                .Cacheable()
                .ToListAsync();
            if (monthQuery?.Any() != true)
                return default;

            var viewModels = monthQuery?.Select(item => item.Map<ItemViewModel>(ent =>
            {
                ent.IncludeAs<Item, Property, PropertyViewModel>(_unitOfWork, x => x.Property,
                    ent2 => ent2.IncludeAs<Property, Category, CategoryViewModel>(_unitOfWork, x => x.Category));
                ent.IncludeAs<Item, Category, CategoryViewModel>(_unitOfWork, x => x.Category);
            })).ToList();
            if (viewModels?.Any() != true)
                return default;

            result.Add(new StatisticsViewModel
            {
                Details = Map(viewModels.Where(x => x.Logs?.Create?.DateTime.Date == dateTimeToday).ToList()),
                From = dateTimeToday,
                To = DateTime.Today.Date,
                Range = StatisticsRangeEnum.Today
            });
            result.Add(new StatisticsViewModel
            {
                Details = Map(viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= dateTimeWeek.Date).ToList()),
                From = dateTimeWeek,
                To = DateTime.Today.Date,
                Range = StatisticsRangeEnum.ThisWeek
            });
            result.Add(new StatisticsViewModel
            {
                Details = Map(viewModels.Where(x => x.Logs?.Create?.DateTime.Date >= dateTimeMonth.Date).ToList()),
                From = dateTimeMonth,
                To = DateTime.Today.Date,
                Range = StatisticsRangeEnum.ThisMonth
            });

            if (result?.Any() != true)
                return default;

            return result;
        }
    }
}