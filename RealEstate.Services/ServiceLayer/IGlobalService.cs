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

        Task FixFinalPriceAsync();

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
            _properties = _unitOfWork.Set<Property>();
            _propertyFeatures = _unitOfWork.Set<PropertyFeature>();
        }

        private List<StatisticsDetailViewModel> Map(List<ItemViewModel> item)
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
            Applicant testModel;
            foreach (var row in model)
            {
                var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                if (properties?.Any() != true)
                    continue;

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

        public static string FixFinalPrice(string value)
        {
            value = CleanNumberDividers(value);
            if (!long.TryParse(value, out var finalPrice))
                return value;

            var finalPriceLength = finalPrice > 1000000000 ? 10 : 9;
            var finalPriceNormalized = value.PadRight(finalPriceLength, '0');
            finalPrice = long.TryParse(finalPriceNormalized, out var finalPriceTemp) && finalPriceTemp > 3000000000 ? finalPriceTemp / 10 : finalPriceTemp;
            finalPriceNormalized = finalPrice.ToString();

            return finalPriceNormalized;
        }

        public async Task FixLoanPriceAsync()
        {
            var query = _propertyFeatures
                .Include(x => x.Feature)
                .Where(x => x.FeatureId == "736ad605-78ea-41e1-bdeb-8d2811db2dec");

            var propertyFeatures = await query.ToListAsync();
            if (propertyFeatures?.Any() != true)
                return;

            var updatedIndicator = 0;
            foreach (var propertyFeature in propertyFeatures)
            {
                var normalized = FixLoanPrice(propertyFeature.Value);
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

        public async Task FixFinalPriceAsync()
        {
            var query = _propertyFeatures
                .Include(x => x.Feature)
                .Where(x => x.FeatureId == "54a0b920-c17f-4ff2-9c51-f9551159026a");

            var propertyFeatures = await query.ToListAsync();
            if (propertyFeatures?.Any() != true)
                return;

            var updatedIndicator = 0;
            foreach (var propertyFeature in propertyFeatures)
            {
                var normalized = FixFinalPrice(propertyFeature.Value);
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

        public async Task FixAddressAsync()
        {
            var query = _properties.AsQueryable();

            var properties = await query.ToListAsync();
            if (properties?.Any() != true)
                return;

            var updatedIndicator = 0;
            foreach (var property in properties)
            {
                var street = property.Street;
                var normalized = FixAddress(street);

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
            var query = _propertyFeatures
                .Include(x => x.Feature)
                .Where(x => x.FeatureId == "cdb97926-b3b1-48ec-bdd6-389a0431007c" && x.Value.Length == 2);

            var propertyFeatures = await query.ToListAsync();
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
                ent.IncludeAs<Item, Property, PropertyViewModel>(_unitOfWork, x => x.Property, ent2 => ent2.IncludeAs<Property, Category, CategoryViewModel>(_unitOfWork, x => x.Category));
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