using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketComments
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset Created { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string FileUrl { get; set; }
        public virtual Tickets Ticket { get; set; }
        // above connects to the ticket Id so it has to be singular ; plural part is bc model is called Tickets
        public virtual ApplicationUser User { get; set; }
    }
}