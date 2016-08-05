
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DayCare.Web.Models
{
    public class DayCareContext : DbContext
    {
        public virtual DbSet<Guardian> Guardians { get; set; }
        public virtual DbSet<Child> Children { get; set; }
        public virtual DbSet<ChildGuardianInfo> ChildGuardians { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }

        public DayCareContext(DbContextOptions<DayCareContext> options) : base(options)
        {
            Seed();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChildGuardianInfo>()
                .HasKey(cg => new { cg.ChildId, cg.GuardianId });

            modelBuilder.Entity<ChildGuardianInfo>()
                .HasOne(cg => cg.Guardian)
                .WithMany(g => g.ChildrenInfo)
                .HasForeignKey(cg => cg.GuardianId);

            modelBuilder.Entity<ChildGuardianInfo>()
                .HasOne(cg => cg.Child)
                .WithMany(c => c.GuardianInfo)
                .HasForeignKey(cg => cg.ChildId);

            base.OnModelCreating(modelBuilder);
        }

        private void Seed()
        {
            // Smith Family
            var john = new Guardian { Email = "john@smith.com", FirstName = "John", LastName = "Smith" };
            var jane = new Guardian { Email = "jane@smith.com", FirstName = "Jane", LastName = "Smith" };
            var granny = new Guardian { Email = "granny@smith.com", FirstName = "Granny", LastName = "Smith" };

            // Jones Family
            var david = new Guardian { Email = "david@jones.com", FirstName = "David", LastName = "Jones" };
            var donna = new Guardian { Email = "donna@jones.com", FirstName = "Donna", LastName = "Jones" };
            var uncle = new Guardian { Email = "unc@jones.com", FirstName = "Uncle", LastName = "Jones" };

            var guardians = new List<Guardian>
            {
                john, jane, granny,
                david,donna,uncle
            };

            // Children
            var billy = new Child { FirstName = "Billy", LastName = "Smith", DOB = DateTimeOffset.Now.AddYears(-5) };
            var bonnie = new Child { FirstName = "Bonnie", LastName = "Smith", DOB = DateTimeOffset.Now.AddYears(-2) };

            var ellie = new Child { FirstName = "Ellie", LastName = "Jones", DOB = DateTimeOffset.Now.AddYears(-5) };
            var evan = new Child { FirstName = "Evan", LastName = "Jones", DOB = DateTimeOffset.Now.AddYears(-2) };

            var children = new List<Child>
            {
                billy, bonnie,
                ellie, evan
            };

            // Staff
            var dale = new Staff { FirstName = "Dale", LastName = "King", Email = "david@daycare.edu" };
            var rachel = new Staff { FirstName = "Rachel", LastName = "Turner", Email = "rachel@daycare.edu" };

            var staff = new List<Staff>
            {
                dale,rachel
            };

            // Child info

            var smithChildInfo = new List<ChildGuardianInfo>
            {
                // Smith Connections
               new ChildGuardianInfo
               {
                   Guardian = john,
                   Child = billy,
                   RelationshipType = RelationshipType.Parent
               },
               new ChildGuardianInfo
               {
                   Guardian = john,
                   Child = bonnie,
                   RelationshipType = RelationshipType.Parent
               },
               new ChildGuardianInfo
               {
                   Guardian = jane,
                   Child = billy,
                   RelationshipType = RelationshipType.Parent
               },
                  new ChildGuardianInfo
               {
                   Guardian = jane,
                   Child = bonnie,
                   RelationshipType = RelationshipType.Parent
               },
               // jones Connections
               new ChildGuardianInfo
               {
                   Guardian = david,
                   Child = ellie,
                   RelationshipType = RelationshipType.Parent
               },
               new ChildGuardianInfo
               {
                   Guardian = david,
                   Child = evan,
                   RelationshipType = RelationshipType.Parent
               },
                new ChildGuardianInfo
               {
                   Guardian = donna,
                   Child = ellie,
                   RelationshipType = RelationshipType.Parent
               },
               new ChildGuardianInfo
               {
                   Guardian = donna,
                   Child = evan,
                   RelationshipType = RelationshipType.Parent
               },
                new ChildGuardianInfo
               {
                   Guardian = uncle,
                   Child = evan,
                   RelationshipType = RelationshipType.Other
               },
            };

            Guardians.AddRange(guardians);
            Children.AddRange(children);
            ChildGuardians.AddRange(smithChildInfo);
            Staff.AddRange(staff);

            SaveChanges();
        }
    }
}
