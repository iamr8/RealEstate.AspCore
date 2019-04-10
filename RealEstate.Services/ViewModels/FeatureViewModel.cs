using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.ViewModels
{
    public class FeatureViewModel : BaseLogViewModel<Feature>
    {
        public FeatureViewModel()
        {
        }

        public FeatureViewModel(Feature model) : base(model)
        {
            if (model == null)
                return;

            Name = model.Name;
            Type = model.Type;
            Id = model.Id;
            Properties = model.PropertyFeatures.Count;
        }

        public string Name { get; set; }

        public FeatureTypeEnum Type { get; set; }

        public int Properties { get; set; }
    }
}