using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.ViewModels.Json
{
    public class UserPropertyCategoryJsonViewModel : BaseViewModel
    {
        private readonly UserPropertyCategory _model;

        public UserPropertyCategoryJsonViewModel(UserPropertyCategory model)
        {
            if (model == null)
                return;

            _model = model;
            Id = _model.CategoryId;
            Name = _model.Category.Name;
        }

        public UserPropertyCategoryJsonViewModel()
        {
        }

        [JsonProperty("n")]
        public string Name { get; set; }
    }
}