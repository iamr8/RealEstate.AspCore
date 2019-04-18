using GeoAPI.Geometries;

namespace RealEstate.Services.ViewModels
{
    public class GeolocationViewModel
    {
        public GeolocationViewModel(IPoint point)
        {
            if (point == null)
                return;

            Point = point;
        }

        public double Latitude => Point.Y;
        public double Longitude => Point.X;

        public IPoint Point { get; }
    }
}