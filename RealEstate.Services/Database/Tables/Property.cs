using GeoAPI.Geometries;
using RealEstate.Base;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;
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

        [NotMapped]
        private string StreetNormalized
        {
            get
            {
                if (string.IsNullOrEmpty(Street))
                    return Street;

                var stPrefixed = new[]
                {
                    "خیابان خ", "خیابان  خ", "خ"
                };
                var prefixes = new[]
                {
                    "خیابان", "اتوبان", "سمت", "کوی", "بلوار", "پاساژ", "نبش", "فاز", "روبروی", "سه راه", "بازارچه"
                };
                if (string.IsNullOrEmpty(Street))
                    return default;

                var street = Street.Trim();
                street = street.Replace(new[]
                {
                    "روبه روی ", "روبه رو ", "روبه رویه "
                }, "روبروی ");
                street = street.Replace(" بارک ", " پارک ");
                street = street.Replace(" سراه ", " سه راه ");

                var concatPrefixes = prefixes.Concat(stPrefixed).ToList();
                if (concatPrefixes?.Any(x => street.StartsWith($"{x} ")) == true)
                {
                    foreach (var prefix in concatPrefixes)
                    {
                        var term = $"{prefix} ";
                        var timmedStreet = street.Trim();
                        if (!timmedStreet.StartsWith(term, StringComparison.CurrentCultureIgnoreCase))
                            continue;

                        var pref = stPrefixed.Any(x => x == prefix)
                            ? "خیابان "
                            : term;
                        var tempStreet = $"{pref}{timmedStreet.Split(term)[1]}";
                        street = tempStreet;
                    }
                }
                else
                {
                    street = $"خیابان {street}";
                }

                street = street.Replace(" خ ", " خیابان ");
                return street.FixPersian();
            }
        }

        [NotMapped]
        public string Address
        {
            get
            {
                var finalString = new StringBuilder();
                finalString.Append(StreetNormalized);

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