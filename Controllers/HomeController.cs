using BugTracker.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult LoggedIn()
        {

            return View();
        }

        public ActionResult NotAuthorized()
        {

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // POST: /Account/Login
        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult DemoLog (string DemoUser)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        if (DemoUser.Equals("Developer")) {

        //        }
        //        else if (DemoUser.Equals("Project Manager")) {

        //        }
        //        return View(returnUrl);
        //    }
        //}
    }
}