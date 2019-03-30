using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class ContactViewModel : BaseTrackViewModel
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        //        public int Properties { get; set; }
        //        public int Deals { get; set; }
    }
}