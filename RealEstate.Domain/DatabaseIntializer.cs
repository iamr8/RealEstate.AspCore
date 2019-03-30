using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain.Tables;
using System;

namespace RealEstate.Domain
{
    public static class DatabaseIntializer
    {
        public static void SeedDatabase(this ModelBuilder modelBuilder)
        {
            //            using (var context = new ApplicationDbContext())
            //            {
            //                context.Database.EnsureCreated();

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                DateTime = DateTime.Now,
                Address = "باهنر",
                FirstName = "هانی",
                LastName = "موسی زاده",
                Username = "admin",
                Role = Role.SuperAdmin,
                Password = "123456".Cipher(CryptologyExtension.CypherMode.Encryption),
                Mobile = "09166000341"
            };
            //            }

            //            var districts = new List<District>
            //            {
            //                new District
            //                {
            //                    Id = Guid.NewGuid().ToString(),
            //                    DateTime = DateTime.UtcNow,
            //                    Name = "کیانپارس",
            //                },
            //                new District
            //                {
            //                    Id = Guid.NewGuid().ToString(),
            //                    DateTime = DateTime.UtcNow,
            //                    Name = "زیتون کارمندی",
            //                }
            //            };
            //            var features = new List<Feature>
            //            {
            //                new Feature
            //                {
            //                    Id = Guid.NewGuid().ToString(),
            //                    DateTime = DateTime.UtcNow,
            //                    Name = "قیمت نهایی",
            //                    Type = FeatureTypeEnum.Deal
            //                },
            //                new Feature
            //                {
            //                    Id = Guid.NewGuid().ToString(),
            //                    DateTime = DateTime.UtcNow,
            //                    Name = "بر زمین",
            //                    Type = FeatureTypeEnum.Property
            //                },
            //                new Feature
            //                {
            //                    Id = Guid.NewGuid().ToString(),
            //                    DateTime = DateTime.UtcNow,
            //                    Name = "قیمت هر متر",
            //                    Type = FeatureTypeEnum.Deal
            //                }
            //            };
            //            var facilities = new List<Facility>
            //            {
            //                new Facility
            //                {
            //                    Id = Guid.NewGuid().ToString(),
            //                    DateTime = DateTime.UtcNow,
            //                    Name = "آسانسور",
            //                },
            //                new Facility
            //                {
            //                    Id = Guid.NewGuid().ToString(),
            //                    DateTime = DateTime.UtcNow,
            //                    Name = "سالن همایش"
            //                }
            //            };
            modelBuilder.Entity<User>().HasData(user);
            //            modelBuilder.Entity<District>().HasData(districts);
            //            modelBuilder.Entity<Feature>().HasData(features);
            //            modelBuilder.Entity<Facility>().HasData(facilities);
        }
    }
}