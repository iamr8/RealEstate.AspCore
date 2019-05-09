using System.Linq;

namespace RealEstate.Base
{
    public class BaseInputViewModel : BaseViewModel
    {
        public BaseInputViewModel()
        {
            GetType().GetPublicProperties().Where(x => x.PropertyType == typeof(string)).ToList()?.ForEach(property =>
            {
                var value = property.GetValue(this) as string;
                if (string.IsNullOrEmpty(value))
                    return;

                value = value.FixPersian();
                property.SetValue(this, value);
            });
        }

        public bool IsNew => string.IsNullOrEmpty(Id);
    }
}