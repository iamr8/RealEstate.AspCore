using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

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
            var division = new Division
            {
                Id = Guid.NewGuid().ToString(),
                Subject = "املاک",
                Audits = audits
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid().ToString(),
                Address = "باهنر",
                FirstName = "هانی",
                LastName = "موسی زاده",
                Mobile = "09166000341",
                Phone = "33379367",
                Audits = audits
            };

            var employeeDivision = new EmployeeDivision
            {
                Id = Guid.NewGuid().ToString(),
                EmployeeId = employee.Id,
                DivisionId = division.Id,
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
                Audits = audits
            };
            modelBuilder.Entity<Division>().HasData(division);
            modelBuilder.Entity<Employee>().HasData(employee);
            modelBuilder.Entity<EmployeeDivision>().HasData(employeeDivision);
            modelBuilder.Entity<EmployeeStatus>().HasData(employeeStatus);
            modelBuilder.Entity<User>().HasData(user);
        }
    }
}