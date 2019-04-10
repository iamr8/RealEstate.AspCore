using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.ViewModels
{
    public class PictureViewModel : BaseLogViewModel<Picture>
    {
        public PictureViewModel(Picture model) : base(model)
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