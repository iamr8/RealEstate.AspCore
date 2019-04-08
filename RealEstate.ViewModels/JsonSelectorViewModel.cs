using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class JsonSelectorViewModel
    {
        public string Json { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }
        public string ScriptKey { get; set; }
        public string IdProperty { get; set; }
        public string NameProperty { get; set; }
        public string ValueProperty { get; set; }

        public string ModelName { get; set; }
        public string ItemName { get; set; }
        public string Title { get; set; }
    }
}