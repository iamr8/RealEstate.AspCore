using Newtonsoft.Json;
using RealEstate.Base;
using System;

namespace RealEstate.Services.Extensions
{
    public static class PassModelExtensions
    {
        public static string Serialize<TModel>(this TModel model) where TModel : BaseInputViewModel
        {
            var json = JsonConvert.SerializeObject(model);
            return json;
        }

        public static TModel Deserialize<TModel>(this TModel model, string identifier, string serializedModel) where TModel : BaseInputViewModel
        {
            if (string.IsNullOrEmpty(serializedModel))
                return model;

            var passModel = JsonConvert.DeserializeObject<TModel>(serializedModel);
            if (passModel == null)
                return model;

            TModel result;
            if (!string.IsNullOrEmpty(identifier))
            {
                result = model != null
                    ? passModel?.Id == model.Id
                        ? passModel
                        : model
                    : default;
            }
            else
            {
                result = passModel;
            }

            return result;
        }
    }
}