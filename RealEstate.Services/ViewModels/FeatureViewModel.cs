using RealEstate.Base.Enums;
using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;

namespace RealEstate.Services.ViewModels
{
    public class FeatureViewModel : BaseLogViewModel<Feature>
    {
        public FeatureViewModel()
        {
        }

        public FeatureViewModel(Feature model, bool showDeleted) : base(model)
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