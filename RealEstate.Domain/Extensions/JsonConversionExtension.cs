using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.Domain.Extensions
{
    public static class JsonConversionExtension
    {
        public static void HasJsonConversion<TProperty>(
            this PropertyBuilder<TProperty> builder) where TProperty : class
        {
            var jsonConversion = new ValueConverter<TProperty, string>(
                value => JsonConvert.SerializeObject(value, JsonExtensions.JsonNetSetting),
                value => JsonConvert.DeserializeObject<TProperty>(value, JsonExtensions.JsonNetSetting));

            builder.HasConversion(jsonConversion);
        }
    }
}