using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Projects
    {
        public Projects()
        {
            this.Tickets = new HashSet<Tickets>();
            this.Users = new HashSet<ApplicationUser>();
            
        }
        public int Id { get; set; }
        public string name { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Description { get; set; }
        public string ProjectStatus {get;set;}

        public virtual ICollection<Tickets> Tickets { get; set; }
        // above connects to the ticket Id so it has to be singular ; plural part is bc model is called Tickets

        public virtual ICollection<ApplicationUser> Users{ get; set; }
        //Many to Many Relationship with ApplicationUser(Users) table.  Also see IdentityModel.cs. The result is a Join Table.
        //This creates a "many" relationship to the ApplicationUser table.  This is necessary in order to have SQL server create a join table in the database.

    }
}