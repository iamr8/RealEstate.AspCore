using GeoAPI.Geometries;
using RealEstate.Services.Database.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RealEstate.Services.Database.Tables
{
    public class Property : BaseEntity
    {
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

        private string HtmlizeValue(object value)
        {
            return $"<button class=\"btn-sm\">{value}</button>";
        }

        [NotMapped]
        public string AddressHtmlStyled
        {
            get
            {
                var finalString = new StringBuilder();
                finalString.Append("خیابان ").Append(HtmlizeValue(Street));

                if (!string.IsNullOrEmpty(Number))
                    finalString.Append("پلاک ").Append(HtmlizeValue(Number));

                if (!string.IsNullOrEmpty(BuildingName))
                    finalString.Append("ساختمان ").Append(HtmlizeValue(BuildingName));

                if (Floor > 0)
                    finalString.Append("طبقه ").Append(HtmlizeValue(Floor));

                if (Flat > 0)
                    finalString.Append("واحد ").Append(HtmlizeValue(Flat));

                return finalString.ToString();
            }
        }

        [NotMapped]
        public string Address
        {
            get
            {
                var finalString = new StringBuilder();

                Street = Street?.StartsWith("خیابان ", StringComparison.CurrentCultureIgnoreCase) == true ? Street.Split("خیابان ")[1] : Street;
                Street = Street?.StartsWith("خ ", StringComparison.CurrentCultureIgnoreCase) == true ? Street.Split("خ ")[1] : Street;

//                Street = Street?.StartsWith("اتوبان ", StringComparison.CurrentCultureIgnoreCase) == true ? Street.Split("اتوبان ")[1] : Street;

                finalString.Append("خیابان ").Append(Street);

                if (!string.IsNullOrEmpty(Number))
                    finalString.Append("، ").Append("پلاک ").Append(Number);

                if (!string.IsNullOrEmpty(BuildingName))
                    finalString.Append("، ").Append("ساختمان ").Append(BuildingName);

                if (Floor > 0)
                    finalString.Append("، ").Append("طبقه ").Append(Floor);

                if (Flat > 0)
                    finalString.Append("، ").Append("واحد ").Append(Flat);

                return finalString.ToString();
            }
        }

        public virtual District District { get; set; }
        public virtual ICollection<PropertyOwnership> PropertyOwnerships { get; set; }

        [NotMapped]
        public PropertyOwnership CurrentOwnership => PropertyOwnerships?.LastOrDefault();

        public virtual ICollection<Item> Items { get; set; }

        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<PropertyFacility> PropertyFacilities { get; set; }
        public virtual ICollection<PropertyFeature> PropertyFeatures { get; set; }

        public override string ToString()
        {
            return Address;
        }
    }
}