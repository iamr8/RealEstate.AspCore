using RealEstate.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.ViewModels
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