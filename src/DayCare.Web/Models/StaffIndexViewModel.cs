

namespace DayCare.Web.Models
{
    using System.Collections.Generic;

    public class StaffIndexViewModel
    {
        public Staff StaffMember { get; set; }
        public IEnumerable<Child> Children { get; set; }
    }
}
