using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class JsonSelectorViewModel
    {
        public string Json { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }
        public string WrapperClass { get; set; }
        public string LocalizerName { get; set; }
        public string LocalizerButtonName { get; set; }
        public string SelectId { get; set; }
        public string ButtonId { get; set; }

        public string ScriptKey { get; set; }
        public string ItemSelectorClass { get; set; }
        public string IdProperty { get; set; }
        public string NameProperty { get; set; }
    }
}