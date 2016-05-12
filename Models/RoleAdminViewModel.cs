using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class RoleAdminViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string>Role { get; set; }

    }
}