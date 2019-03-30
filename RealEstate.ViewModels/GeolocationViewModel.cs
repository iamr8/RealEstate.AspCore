using GeoAPI.Geometries;
using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class GeolocationViewModel : BaseTrackViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IPoint Point { get; set; }
    }
}