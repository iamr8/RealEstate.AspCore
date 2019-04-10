using GeoAPI.Geometries;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels
{
    public class GeolocationViewModel : BaseViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IPoint Point { get; set; }
    }
}