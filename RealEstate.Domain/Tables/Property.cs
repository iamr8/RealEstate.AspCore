using GeoAPI.Geometries;
using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class Property : BaseEntity
    {
        public Property()
        {
            Pictures = new HashSet<Picture>();
            PropertyFacilities = new HashSet<PropertyFacility>();
            Items = new HashSet<Item>();
            PropertyFeatures = new HashSet<PropertyFeature>();
            PropertyOwnerships = new HashSet<PropertyOwnership>();
            Logs = new HashSet<Log>();
        }

        [Required]
        public string Street { get; set; }

        public string Alley { get; set; }

        public string BuildingName { get; set; }
        public string Number { get; set; }
        public int Floor { get; set; }
        public int Flat { get; set; }
        public string Description { get; set; }

        public string DistrictId { get; set; }
        public string CategoryId { get; set; }
        public IPoint Geolocation { get; set; }
        public virtual Category Category { get; set; }

        public virtual District District { get; set; }
        public virtual ICollection<PropertyOwnership> PropertyOwnerships { get; set; }
        public virtual ICollection<Item> Items { get; set; }

        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<PropertyFacility> PropertyFacilities { get; set; }
        public virtual ICollection<PropertyFeature> PropertyFeatures { get; set; }
    }
}