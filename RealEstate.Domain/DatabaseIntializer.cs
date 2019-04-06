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
                Mobile = "09166000341",
                Phone = "33379367",
            };
            modelBuilder.Entity<User>().HasData(user);
        }
    }
}