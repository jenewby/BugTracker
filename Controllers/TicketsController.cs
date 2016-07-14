using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using BugTracker.Helpers;
using System.Threading.Tasks;

namespace BugTracker
{[Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            return View(db.Tickets.ToList());
        }

        public ActionResult MyTickets()
        {
            //var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);

            //return View(db.Tickets.ToList());

            UserRolesHelper helper = new UserRolesHelper(db);

            var usr = db.Users.Find(User.Identity.GetUserId());

            if (helper.IsUserInRole(usr.Id, "Admin"))
            {
                return View(db.Tickets.ToList());
            }
            // developer and proj manager have same access rights
            if (helper.IsUserInRole(usr.Id, "Developer") || helper.IsUserInRole(usr.Id, "Project Manager"))
            {

                return View(usr.Projects.SelectMany(p => p.Tickets));

                //I dont understand this above return
            }


            else {

                return View(db.Tickets.Where(x => x.OwnerUserId == usr.Id).ToList());

            }
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            //ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
                tickets.Created = System.DateTimeOffset.Now;
                tickets.Updated = System.DateTimeOffset.Now;

                //tickets.AssignedToUserId = "";
                tickets.OwnerUserId = User.Identity.GetUserId();
                db.Tickets.Add(tickets);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", tickets.AssignedToUserId);
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            ProjectsHelper pjh = new ProjectsHelper(db);
            var usr = db.Users.Find(User.Identity.GetUserId());
            Tickets tickets = db.Tickets.Find(id);

            //if ( User.IsInRole("Admin") || pjh.IsUserInProject(usr.Id,tickets.ProjectId) || (tickets.OwnerUserId == usr.Id))
            if (User.IsInRole("Admin") || User.IsInRole("Developer"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (tickets == null)
                {
                    return HttpNotFound();
                }
                ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", tickets.AssignedToUserId);
                ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "name", tickets.ProjectId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
                return View(tickets);
            }
            else 
            {                 
                return RedirectToAction("NotAuthorized", "Home");
            }


    }

    // POST: Tickets/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
            
                var oldTic = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == tickets.Id);
                db.Tickets.Attach(tickets);
                db.Entry(tickets).Property("TicketTypeId").IsModified = true;
                //this is the syntax for allowing to modify in a database. I cant change value in database without this syntax
                db.Entry(tickets).Property("Title").IsModified = true;
                db.Entry(tickets).Property("Description").IsModified = true;
                db.Entry(tickets).Property("AssignedToUserId").IsModified = true;
                db.Entry(tickets).Property("TicketPriorityId").IsModified = true;
                db.Entry(tickets).Property("TicketStatusId").IsModified = true;
                db.Entry(tickets).Property("Updated").IsModified = true;

                if (oldTic.Title != tickets.Title) {
                    //Creating a ticket history to populate database
                    TicketHistories ticketHistory1 = new TicketHistories();
                    ticketHistory1.TicketId = tickets.Id;
                    ticketHistory1.Property = "Title";
                    ticketHistory1.UserId = User.Identity.GetUserId();
                    ticketHistory1.OldValue = oldTic.Title;
                    ticketHistory1.NewValue = tickets.Title;
                    ticketHistory1.Changed = DateTimeOffset.Now;
                    db.TicketHistories.Add(ticketHistory1);
                }
                if (oldTic.TicketTypeId != tickets.TicketTypeId)
                {
                    //Creating a ticket history to populate database
                    TicketHistories ticketHistory2 = new TicketHistories();
                    ticketHistory2.TicketId = tickets.Id;
                    ticketHistory2.Property = "TicketTypeId";
                    ticketHistory2.UserId = User.Identity.GetUserId();
                    ticketHistory2.OldValue = oldTic.TicketType.Name;
                    ticketHistory2.NewValue = db.TicketTypes.Find(tickets.TicketTypeId).Name;
                    ticketHistory2.Changed = DateTimeOffset.Now;
                    db.TicketHistories.Add(ticketHistory2);
                }

                if (oldTic.TicketPriorityId != tickets.TicketPriorityId)
                {
                    //Creating a ticket history to populate database
                    TicketHistories ticketHistory3 = new TicketHistories();
                    ticketHistory3.TicketId = tickets.Id;
                    ticketHistory3.Property = "TicketPriorityId";
                    ticketHistory3.UserId = User.Identity.GetUserId();
                    ticketHistory3.OldValue = oldTic.TicketPriority.Name;
                    ticketHistory3.NewValue = db.TicketPriorities.Find(tickets.TicketPriorityId).Name;
                    ticketHistory3.Changed = DateTimeOffset.Now;
                    db.TicketHistories.Add(ticketHistory3);
                }

                if (oldTic.TicketStatusId != tickets.TicketStatusId)
                {
                    //Creating a ticket history to populate database
                    TicketHistories ticketHistory4 = new TicketHistories();
                    ticketHistory4.TicketId = tickets.Id;
                    ticketHistory4.Property = "TicketStatusId";
                    ticketHistory4.UserId = User.Identity.GetUserId();
                    ticketHistory4.OldValue = oldTic.TicketStatus.Name;
                    ticketHistory4.NewValue = db.TicketStatuses.Find(tickets.TicketStatusId).Name;
                    ticketHistory4.Changed = DateTimeOffset.Now;
                    db.TicketHistories.Add(ticketHistory4);
                }

                if (oldTic.Description != tickets.Description)
                {
                    //Creating a ticket history to populate database
                    TicketHistories ticketHistory5 = new TicketHistories();
                    ticketHistory5.TicketId = tickets.Id;
                    ticketHistory5.Property = "Description";
                    ticketHistory5.UserId = User.Identity.GetUserId();
                    ticketHistory5.OldValue = oldTic.Description;
                    ticketHistory5.NewValue = tickets.Description;
                    ticketHistory5.Changed = DateTimeOffset.Now;
                    db.TicketHistories.Add(ticketHistory5);
                }

                if (oldTic.AssignedToUserId != tickets.AssignedToUserId)
                {
                    //Creating a ticket history to populate database
                    TicketHistories ticketHistory6 = new TicketHistories();
                    ticketHistory6.TicketId = tickets.Id;
                    ticketHistory6.Property = "AssignedToUserId";
                    ticketHistory6.UserId = User.Identity.GetUserId();
                    if(oldTic.AssignedToUser != null)
                    {
                        ticketHistory6.OldValue = (oldTic.AssignedToUser.FirstName + " " + oldTic.AssignedToUser.LastName);
                    }
                    ticketHistory6.NewValue = db.Users.Find(tickets.AssignedToUserId).FirstName + " " + db.Users.Find(tickets.AssignedToUserId).LastName;
                    ticketHistory6.Changed = DateTimeOffset.Now;
                    db.TicketHistories.Add(ticketHistory6);

                    Projects originalProject = db.Projects.Find(oldTic.Project.Id);

                    var callbackUrl = Url.Action("MyTickets");
                    EmailService Email = new EmailService();
                    IdentityMessage im = new IdentityMessage();
                    im.Destination = db.Users.Find(tickets.AssignedToUserId).Email;
                    im.Subject = "You have been assigned to a Ticket";
                    im.Body = "You have been assigned to a ticket under the " + originalProject.name + " project. Click the link to check the ticket! < a href =\"" + callbackUrl + "\">here</a>";

                    await Email.SendAsync(im);

                }

                //db.Tickets.AsNoTracking().FirstOrDefault(t =>t.Id == tickets.Id);
                //db.Entry(tickets).State = EntityState.Modified;
                // using find creates problems bc using find pulls from the ticket in memory as opposed to the ticket in server so changes will keep changing and wont be able to save an old
                tickets.Updated = System.DateTimeOffset.Now;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", tickets.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            var usr = db.Users.Find(User.Identity.GetUserId());


            if (helper.IsUserInRole(usr.Id, "Admin") || helper.IsUserInRole(usr.Id, "Project Manager"))

            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Tickets tickets = db.Tickets.Find(id);
                if (tickets == null)
                {
                    return HttpNotFound();
                }
                return View(tickets);
            }
            else {
                return RedirectToAction("NotAuthorized", "Home");
            }
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tickets tickets = db.Tickets.Find(id);
            db.Tickets.Remove(tickets);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //POST: Tickets/CreateComment
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment([Bind(Include ="TicketId,Comment")]TicketComments cm, HttpPostedFileBase img)
        {

            //var post = db.TicketComments.Find(comment.TicketId);

            if (ModelState.IsValid)
            {
              //restricting the valid file formats to only images
            if (ImageUploadValidator.IsWebFriendlyImage(img))
                {
                    var fileName = Path.GetFileName(img.FileName);
                    img.SaveAs(Path.Combine(Server.MapPath("~/img/UserComments/"), fileName));
                    cm.FileUrl = "~/img/UserComments/" + fileName;
                }
                cm.UserId = User.Identity.GetUserId();
                cm.Created = DateTimeOffset.Now;
                db.TicketComments.Add(cm);
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Tickets", new { id = cm.TicketId});
        }
    }
}
