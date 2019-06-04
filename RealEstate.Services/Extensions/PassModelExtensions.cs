using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Services.Extensions
{
    public static class PassModelExtensions
    {
        public static string SerializePassModel<T>(this T model)
        {
            var json = JsonConvert.SerializeObject(model);
            return json;
        }

        public static T UsePassModelForAdd<T>(this T model, string passJson) where T : BaseInputViewModel
        {
            if (string.IsNullOrEmpty(passJson))
                return model;

            var passModel = JsonConvert.DeserializeObject<T>(passJson);
            if (passModel == null)
                return model;

            return passModel;
        }

        public static T UsePassModelForEdit<T>(this T model, string passJson) where T : BaseInputViewModel
        {
            if (string.IsNullOrEmpty(passJson))
                return model;

            var passModel = JsonConvert.DeserializeObject<T>(passJson);
            if (passModel == null)
                return model;

            var finalModel = model != null
                ? passModel?.Id == model.Id
                    ? passModel
                    : model
                : default;

            return finalModel;
        }
    }
}