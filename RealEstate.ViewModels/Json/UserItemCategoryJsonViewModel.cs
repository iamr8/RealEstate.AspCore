using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.ViewModels.Json
{
    public class UserItemCategoryJsonViewModel : BaseViewModel
    {
        private readonly UserItemCategory _model;

        public UserItemCategoryJsonViewModel(UserItemCategory model)
        {
            if (model == null)
                return;

            _model = model;
            Id = _model.CategoryId;
            Name = _model.Category.Name;
        }

        public UserItemCategoryJsonViewModel()
        {
        }

        [JsonProperty("n")]
        public string Name { get; set; }
    }
}