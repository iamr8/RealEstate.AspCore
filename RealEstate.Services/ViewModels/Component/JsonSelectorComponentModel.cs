using Microsoft.AspNetCore.Mvc.Rendering;
using RestSharp.Extensions;
using System.Collections.Generic;
using System.Globalization;

namespace RealEstate.Services.ViewModels.Component
{
    public class JsonSelectorComponentModel
    {
        public string Json { private get; set; }
        public List<SelectListItem> SelectListItems { get; set; }
        public string ScriptKey { get; set; }
        public string IdProperty { get; set; }
        public string NameProperty { get; set; }
        public string ValueProperty { get; set; }

        public string ModelName { private get; set; }
        public string ItemName { private get; set; }
        public string Title { get; set; }

        public string SelectId => $"{ModelName}_{ItemName}_Select";
        public string SubmitId => $"{ModelName}_{ItemName}_Submit";
        public string ValueId => !string.IsNullOrEmpty(ValueProperty) ? $"{ModelName}_{ItemName}_Value" : null;
        public string JsonInputId => $"{ModelName}_{Json}";
        public string WrapperClass => ItemName.ToCamelCase(CultureInfo.CurrentCulture);
        public string ItemSelectorClass => $"{WrapperClass}-item";
    }
}