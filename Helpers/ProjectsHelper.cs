using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class ProjectsHelper
    {
        private ApplicationDbContext db;

        public ProjectsHelper (ApplicationDbContext context)
        {
            this.db = context;
        }

        // db here is used simply to reference the database ie whatever database will be passed through in the controller.  db will be replaced by the specific instance created during with the controller



        public void AddUserToProject(string userId, int projectId)
        {
            var proj = db.Projects.Find(projectId);
            //finding project by id 
            var usr = db.Users.Find(userId);
            //finding user by id

            if (!IsUserInProject(userId, projectId))
            { proj.Users.Add(usr);
                db.SaveChanges();
            }
        // no need to address if already assigned
        }


        public bool IsUserInProject(string userId, int projectId)
        {
            var proj = db.Projects.Find(projectId);

            var CurrentUser = db.Users.Find(userId);

            if (proj.Users.Contains(CurrentUser))
            { return true; } else
            { return false; }
        }

        //The where statement is looking at all the instances of projects, specifically each instance's collection of Users.
        //The Select statement is then implemented on the collection of users, looking at instances of user
        //within the Users collection. The select statement is looking at the user id of all user instances and seeing
        //if it contains the userId that we passed into the method.
        // Any() checks to see if there is anythign returned by the query



        public void RemoveUserFromProject(string userId, int projectId)
        {
            var proj = db.Projects.Find(projectId);
            var usr = db.Users.Find(userId);

            if (IsUserInProject(userId, projectId)) {

                proj.Users.Remove(usr);
                db.SaveChanges();
            }
        }


        public IList<Projects> ListOfProjectsWithUser(int userId)
        {
            //var usr = db.Users.Find(userId).Projects.ToList();
            //return usr;

            var usr = db.Users.Find(userId);
            var projects = db.Projects.ToList();

            var final = projects.Where(p => p.Users.Contains(usr)).ToList();

            return final;

        }

        public IList<ApplicationUser> ListOfUsersOnProject(int projectId)
        {
            var proj = db.Projects.Find(projectId).Users.ToList() ;
            return proj;

        }

        public IList<ApplicationUser> ListOfUsersNotOnAProject(int projectId)
        {
            var proj = db.Projects.Find(projectId);

            var sfsf = db.Users.Where(u => !u.Projects.Contains(proj)).ToList();

            return sfsf;

        }


    }
}