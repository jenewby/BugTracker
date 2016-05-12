using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // ^ needed whenever i need to access my database

        // GET: Admin
        public ActionResult Index()
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            List<RoleAdminViewModel> model = new List<RoleAdminViewModel>();
            foreach (var user in db.Users.ToList()) {
                RoleAdminViewModel RoleModel = new RoleAdminViewModel();
                RoleModel.Role = helper.ListUserRoles(user.Id).ToList();
                RoleModel.User = user;
                model.Add(RoleModel);
            }

            return View(model);
        }

        public ActionResult EditUser(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);
            var selected = helper.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.Id = user.Id;
            AdminModel.Name = user.DisplayName;


            return View(AdminModel);
        }

        //EditUser POST
        [HttpPost]
        //Where am i posting info?  Which model/viewmodel am i using to pass data through? BC im using the admin view model that will be what im passing through.  Viewmodel is a model itself.  Holds data entered in the view. usr portion can be any name.
        public ActionResult EditUser(AdminUserViewModel usr)
        {
            //var user = db.Users.Find(usr.Id);         

            // instantiate userRolesHelper (function)  
            UserRolesHelper helper = new UserRolesHelper(db);

            foreach (var role in usr.SelectedRoles)
            {
                helper.AddUserToRole(usr.Id, role);
            }

            foreach (var role in db.Roles.ToList())
            {
                if (!usr.SelectedRoles.Contains(role.Name))
                {
                    helper.RemoveUserFromRole(usr.Id, role.Name);
                }
            }
            //var selected = helper.ListUserRoles(usr.Id);
            //AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            //AdminModel.Id = user.Id;
            //AdminModel.Name = user.DisplayName;
            db.SaveChanges();



            return RedirectToAction ("Index", "Admin");
        }
    }
}