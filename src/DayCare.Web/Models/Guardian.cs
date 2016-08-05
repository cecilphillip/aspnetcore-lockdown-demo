using System;
using System.Collections.Generic;

namespace DayCare.Web.Models
{
    public enum RelationshipType
    {
        Parent,
        Sibling,
        GrandParent,
        Other
    }

    public class Guardian
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<ChildGuardianInfo> ChildrenInfo { get; set; }

    }

    public class Child
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DOB { get; set; }
        public List<ChildGuardianInfo> GuardianInfo { get; set; }
        public List<ChildActivity> Activities { get; set; }
    }

    public class ChildGuardianInfo
    {
        public int GuardianId { get; set; }
        public Guardian Guardian { get; set; }

        public int ChildId { get; set; }
        public Child Child { get; set; }

        public RelationshipType RelationshipType { get; set; }
    }

    public class ChildActivity
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public Child Child { get; set; }

        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTimeOffset Ocurred { get; set; }
    }

    public class Staff
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
