using Microsoft.Extensions.Localization;

namespace RealEstate.Resources
{
    public interface ILocalizationService
    {
    }

    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public string this[string index] => _localizer[index];
    }
}