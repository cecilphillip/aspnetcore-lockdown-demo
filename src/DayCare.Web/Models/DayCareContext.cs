
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DayCare.Web.Models
{
    public class DayCareContext : DbContext
    {
        public virtual DbSet<Guardian> Guardians { get; set; }
        public virtual DbSet<Child> Children { get; set; }
        public virtual DbSet<ChildActivity> ChildrenActivities { get; set; }
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

            modelBuilder.Entity<ChildActivity>()
                .HasOne(ca => ca.Child)
                .WithMany(c => c.Activities)
                .HasForeignKey(ca => ca.ChildId);

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

            #region Child info

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
            #endregion

            #region Activity

            var activities = new List<ChildActivity>
            {
                // Jones Kids
                new ChildActivity { Child = billy, Title = "Nap Time", Notes = "Slept for 2 hours.", Ocurred = DateTimeOffset.Now.AddDays(-2)},
                new ChildActivity { Child = billy, Title = "Arrived", Notes = "Jane dropped him off.", Ocurred = DateTimeOffset.Now.AddDays(-3)},
                new ChildActivity { Child = billy, Title = "Ate", Notes = "Didn't eat any of his food today.", Ocurred = DateTimeOffset.Now.AddDays(-3)},

                new ChildActivity { Child = bonnie, Title = "Nap Time", Notes = "Slept for 3 hours.", Ocurred = DateTimeOffset.Now.AddDays(-3)},
                new ChildActivity { Child = bonnie, Title = "Arrived", Notes = "John dropped her off", Ocurred = DateTimeOffset.Now.AddDays(-3)},
                new ChildActivity { Child = bonnie, Title = "Ate", Notes = "Didn't eat any of her food today.", Ocurred = DateTimeOffset.Now.AddDays(-3)},
                
                // Smith Kids
                new ChildActivity { Child = ellie, Title = "Nap Time", Notes = "Didn't sleep at all today", Ocurred = DateTimeOffset.Now.AddDays(-3)},
                new ChildActivity { Child = ellie, Title = "Arrived", Notes = "Donna dropped her off", Ocurred = DateTimeOffset.Now.AddDays(-3)},
                new ChildActivity { Child = ellie, Title = "Ate", Notes = "She was really hungry today. She asked for seconds", Ocurred = DateTimeOffset.Now.AddDays(-3)},

                new ChildActivity { Child = ellie, Title = "Nap Time", Notes = "Didn't sleep at all today", Ocurred = DateTimeOffset.Now.AddDays(-5)},
                new ChildActivity { Child = ellie, Title = "Left", Notes = "His uncle picked him up today early.", Ocurred = DateTimeOffset.Now.AddDays(-3)},
                new ChildActivity { Child = ellie, Title = "Ate", Notes = "She was really hungry today. She asked for seconds", Ocurred = DateTimeOffset.Now.AddDays(-4)},
            };
            #endregion

            Guardians.AddRange(guardians);
            Children.AddRange(children);
            ChildGuardians.AddRange(smithChildInfo);
            ChildrenActivities.AddRange(activities);
            Staff.AddRange(staff);

            SaveChanges();
        }
    }
}
