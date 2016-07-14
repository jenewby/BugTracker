using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketNotifications
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string AssignedUserId { get; set; }
        public string Message { get; set; }
        public DateTimeOffset TimeOfChange { get; set; }

        public virtual Tickets Ticket { get; set; }
        // above connects to the ticket Id so it has to be singular ; plural part is bc model is called Tickets

        public virtual ApplicationUser User { get; set; }
        //application user is part of the identity model and is called user to connect to userId
    }
}