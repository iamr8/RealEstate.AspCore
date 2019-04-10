using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;

namespace RealEstate.Services.ViewModels
{
    public class FixedSalaryViewModel : BaseLogViewModel<FixedSalary>
    {
        public FixedSalaryViewModel(FixedSalary entity) : base(entity)
        {
            if (entity == null)
                return;

            Value = entity.Value;
            Id = entity.Id;
        }

        public FixedSalaryViewModel()
        {
        }

        public double Value { get; set; }
        public UserViewModel User { get; set; }
    }
}