using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.ViewModels
{
    public class UserPropertyCategoryViewModel : BaseLogViewModel<UserPropertyCategory>
    {
        public UserPropertyCategoryViewModel(UserPropertyCategory entity) : base(entity)
        {
            Id = entity.Id;
        }

        public UserPropertyCategoryViewModel()
        {
        }

        public CategoryViewModel Category { get; set; }
    }
}