using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class InsuranceViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Insurance Entity { get; }

        public InsuranceViewModel(Insurance entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public double Price => Entity?.Price ?? 0;

        public EmployeeViewModel Employee { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}