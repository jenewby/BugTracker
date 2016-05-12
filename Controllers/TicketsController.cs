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
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tickets).State = EntityState.Modified;
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
