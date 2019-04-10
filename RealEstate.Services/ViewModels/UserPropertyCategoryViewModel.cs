using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;

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