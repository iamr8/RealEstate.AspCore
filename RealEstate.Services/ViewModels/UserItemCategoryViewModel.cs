using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;

namespace RealEstate.Services.ViewModels
{
    public class UserItemCategoryViewModel : BaseLogViewModel<UserItemCategory>
    {
        public UserItemCategoryViewModel(UserItemCategory entity) : base(entity)
        {
            Id = entity.Id;
        }

        public UserItemCategoryViewModel()
        {
        }

        public CategoryViewModel Category { get; set; }
    }
}