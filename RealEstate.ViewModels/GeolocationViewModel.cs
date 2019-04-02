using GeoAPI.Geometries;
using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class GeolocationViewModel : BaseLogViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IPoint Point { get; set; }
    }
}