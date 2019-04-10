using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;

namespace RealEstate.Services.ViewModels
{
    public class PictureViewModel : BaseLogViewModel<Picture>
    {
        public PictureViewModel(Picture model, bool showDeleted) : base(model)
        {
            if (model == null)
                return;

            File = model.File;
            Id = model.Id;
            Text = model.Text;
        }

        public PictureViewModel()
        {
        }

        public string File { get; set; }
        public string Text { get; set; }
    }
}