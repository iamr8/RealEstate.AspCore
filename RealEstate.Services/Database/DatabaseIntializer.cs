using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Database
{
    public static class DatabaseIntializer
    {
        public static void SeedDatabase(this ModelBuilder modelBuilder)
        {
            var audits = new List<LogJsonEntity>
                {
                    new LogJsonEntity
                    {
                        Type = LogTypeEnum.Create,
                        UserId = null,
                        UserFullName = "آرش شبه",
                        DateTime = DateTime.Now,
                        UserMobile = "09364091209"
                    }
                };

            var districts = new List<District>
            {
                new District
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "زیتون کارمندی",
                },
                new District
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "زیتون کارگری",
                },
                new District
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "باهنر",
                },
                new District
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "کیان آباد",
                },
                new District
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "کیانپارس",
                },
                new District
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "ملیراه",
                },
            };

            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "خرید و فروش",
                    Type = CategoryTypeEnum.Item,
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "زمین",
                    Type = CategoryTypeEnum.Property,
                    Audits = audits,
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "ویلایی",
                    Type = CategoryTypeEnum.Property,
                    Audits = audits,
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "مشارکت در ساخت",
                    Type = CategoryTypeEnum.Item,
                    Audits = audits,
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "رهن و اجاره",
                    Type = CategoryTypeEnum.Item,
                    Audits = audits,
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "رهن کامل",
                    Type = CategoryTypeEnum.Item,
                    Audits = audits,
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "آپارتمان",
                    Type = CategoryTypeEnum.Property,
                    Audits = audits,
                },
                new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "مغازه",
                    Type = CategoryTypeEnum.Property,
                    Audits = audits,
                },
            };
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "املاک",
                },
                new Division
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "کارواش",
                },
            };

            var features = new List<Feature>
            {
                new Feature
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "بر زمین",
                    Type = FeatureTypeEnum.Property,
                },
                new Feature
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "قیمت نهایی",
                    Type = FeatureTypeEnum.Item,
                },
                new Feature
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "بودجه",
                    Type = FeatureTypeEnum.Applicant,
                },
                new Feature
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "متراژ",
                    Type = FeatureTypeEnum.Property,
                },
                new Feature
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "پیش پرداخت",
                    Type = FeatureTypeEnum.Applicant,
                },
                new Feature
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "قیمت هر متر",
                    Type = FeatureTypeEnum.Item,
                },
                new Feature
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "تعداد خواب",
                    Type = FeatureTypeEnum.Property,
                },
                new Feature
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "کرایه",
                    Type = FeatureTypeEnum.Applicant,
                },
            };

            var facilities = new List<Facility>
            {
                new Facility
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "سالن بدنسازی"
                },
                new Facility
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "پارکینگ"
                },
                new Facility
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "آسانسور"
                },
                new Facility
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "سالن همایش"
                },
                new Facility
                {
                    Id = Guid.NewGuid().ToString(),
                    Audits = audits,
                    Name = "آنتن مرکزی"
                },
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid().ToString(),
                Audits = audits,
                Address = "باهنر",
                FirstName = "هانی",
                LastName = "موسی زاده",
                Mobile = "09166000341",
                Phone = "33379367",
            };

            var employeeDivision = new EmployeeDivision
            {
                Id = Guid.NewGuid().ToString(),
                EmployeeId = employee.Id,
                DivisionId = divisions.FirstOrDefault(x => x.Name == "املاک").Id,
                Audits = audits
            };

            var employeeStatus = new EmployeeStatus
            {
                Id = Guid.NewGuid().ToString(),
                EmployeeId = employee.Id,
                Status = EmployeeStatusEnum.Start,
                Audits = audits
            };

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "admin",
                Password = "123456".Cipher(CryptologyExtension.CypherMode.Encryption),
                EmployeeId = employee.Id,
                Role = Role.SuperAdmin,
                Audits = audits
            };
            modelBuilder.Entity<Division>().HasData(divisions);
            modelBuilder.Entity<Employee>().HasData(employee);
            modelBuilder.Entity<EmployeeDivision>().HasData(employeeDivision);
            modelBuilder.Entity<EmployeeStatus>().HasData(employeeStatus);
            modelBuilder.Entity<User>().HasData(user);
            modelBuilder.Entity<Category>().HasData(categories);
            modelBuilder.Entity<District>().HasData(districts);
            modelBuilder.Entity<Facility>().HasData(facilities);
            modelBuilder.Entity<Feature>().HasData(features);
        }
    }
}