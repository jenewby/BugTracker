using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //public virtual Tickets Ticket { get; set; }
        // above connects to the ticket Id so it has to be singular ; plural part is bc model is called Tickets
        
    }
}