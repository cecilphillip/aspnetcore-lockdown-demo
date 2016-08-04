
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DayCare.Web.Models
{
    public class DayCareContext : DbContext
    {
        public virtual DbSet<Guardian> Guardians { get; set; }
        public virtual DbSet<Child> Children { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }

        public DayCareContext(DbContextOptions<DayCareContext> options) : base(options)
        {
            Seed();
        }

        private void Seed()
        {
            var guardians = new List<Guardian>
            {
                new Guardian { Email = "john@smith.com", FirstName = "John", LastName = "Smith"},
                new Guardian { Email = "jane@smith.com", FirstName = "Jane", LastName = "Smith"},
                new Guardian { Email = "granny@smith.com", FirstName = "Granny", LastName = "Smith"},

                new Guardian { Email = "david@jones.com", FirstName = "David", LastName = "Jones"},
                new Guardian { Email = "donna@jones.com", FirstName = "Donna", LastName = "Jones"},
                new Guardian { Email = "unc@jones.com", FirstName = "Uncle", LastName = "Jones"},
            };

            var children = new List<Child>
            {
                new Child { FirstName = "Billy", LastName = "Smith", DOB = DateTimeOffset.Now.AddYears(-5)},
                new Child { FirstName = "Bonnie", LastName = "Smith", DOB = DateTimeOffset.Now.AddYears(-2)},

                new Child { FirstName = "Ellie", LastName = "Jones", DOB = DateTimeOffset.Now.AddYears(-5)},
                new Child { FirstName = "Evan", LastName = "Jones", DOB = DateTimeOffset.Now.AddYears(-2)}
            };

            var staff = new List<Staff>
            {
                new Staff {FirstName = "David", LastName = "King", Email = "david@daycare.edu"},
                new Staff {FirstName = "Rachel", LastName = "Turner", Email = "rachel@daycare.edu"}
            };

            Guardians.AddRange(guardians);
            Staff.AddRange(staff);
            Children.AddRange(children);

            SaveChanges();
        }
    }
}
