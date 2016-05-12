using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize]
        public ActionResult Index()
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            var usrid = db.Users.Find(User.Identity.GetUserId());
            if (helper.IsUserInRole(usrid.Id, "Admin"))
            {

                return View(db.Projects.ToList());
            }
            else {

                return View(usrid.Projects.ToList());

            }
        }


        //GET: EditProject
        public ActionResult AssignUserToProj(int Id)
        {
            ProjectViewModel model = new ProjectViewModel();

            var project = db.Projects.Find(Id);
            // above is single instance of project.  we needed to create that

            ProjectsHelper helper = new ProjectsHelper(db);
            //calls the helper class

            //var notSelectedUsersList = helper.ListOfUsersNotOnAProject(Id).Select(n => n.Id).ToArray();

            var selectedUsersList =  helper.ListOfUsersOnProject(Id).Select(n => n.Id).ToArray();
            //above is list of users based on projectid.  We add the Select to choose only the user id since all we need is the user id and project id to add a person to a project.  Array is select list can take it

            var allUsers = db.Users.ToList();
            //ViewBag.AllUsers = helper.ListOfUsersNotOnAProject(Id).Select(n => n.Id).ToArray();
            //model.Users = new MultiSelectList(notSelectedUsersList, "Id", "DisplayName", notSelectedUsersList);

            model.Users = new MultiSelectList(allUsers, "Id", "DisplayName", selectedUsersList);
            //first paramete is whatever my list is wanting to show  ie here its list of users labeled allUsers. Second item is value being sent back via post method.  Third item is what will display on list. Fourth are selected users

            model.Project = project;
            return View(model);
        }

        //POST: Projects/EditProject
        [HttpPost]
        public ActionResult AssignUserToProj(ProjectViewModel pvm) {
            ProjectsHelper helper = new ProjectsHelper(db);
            var getuserId = User.Identity.GetUserId();

            foreach (var usr in db.Users.ToList())
            {
                if (!pvm.SelectedUsers.Contains(usr.Id))
                {
                    helper.RemoveUserFromProject(usr.Id, pvm.Project.Id);
                }
            }

            foreach (var usr in pvm.SelectedUsers)
            {
                helper.AddUserToProject(usr, pvm.Project.Id);
            }

           
            db.SaveChanges();



            return RedirectToAction("Index", "Projects");
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name, Created, Description, ProjectStatus")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                projects.ProjectStatus = "Open";
                projects.Created = System.DateTimeOffset.Now;
                db.Projects.Add(projects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projects);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            IList<SelectListItem> ps = new List<SelectListItem>
            { new SelectListItem{ Text="Open", Value="Open" },
              new SelectListItem{ Text="Closed", Value="Closed" },
            };
            ViewBag.ProjectStatus = ps;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,name,Description,ProjectStatus" )] Projects projects)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projects);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projects projects = db.Projects.Find(id);
            db.Projects.Remove(projects);
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
    }
}
