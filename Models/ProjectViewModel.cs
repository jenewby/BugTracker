using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjectViewModel
    {
        public Projects Project { get; set; }
        // this property allows me to hold a single instance of projects

        public MultiSelectList Users { get; set; }
        //because we will handle multiple users

        //public MultiSelectList UsersNotOn { get; set; }

        public string[] SelectedUsers { get; set; }
    }
}