namespace BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var roleManager = new RoleManager<IdentityRole>(
                   new RoleStore<IdentityRole>(context));

            //if (!context.Roles.Any(r => r.Name == "Admin"))
            //{
            //    roleManager.Create(new IdentityRole { Name = "Admin" });
            //}


            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            //if (!context.Roles.Any(r => r.Name == "Moderator"))
            //{
            //    roleManager.Create(new IdentityRole { Name = "Moderator" });
            //}
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "ProjectManager@demoEmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ProjectManager@demoEmail.com",
                    Email = "ProjectManager@demoEmail.com",
                    FirstName = "PM.Demo",
                    LastName = "UserPM",
                    DisplayName = "ProjectManagerDemoUser"
                }, "Password-4");
            }
            if (!context.Users.Any(u => u.Email == "Developer@demoEmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Developer@demoEmail.com",
                    FirstName = "DV.Demo",
                    LastName = "UserDV",
                    Email = "Developer@demoEmail.com",
                    DisplayName = "DeveloperDemoUser"
                }, "Password-3");
            }
            var userId3 = userManager.FindByEmail("ProjectManager@demoEmail.com").Id;
            userManager.AddToRole(userId3, "Project Manager");

            var userId4 = userManager.FindByEmail("Developer@demoEmail.com").Id;
            userManager.AddToRole(userId4, "Developer");

            //if (!context.Users.Any(u => u.Email == "jenewby54n@yahoo.com"))
            //{
            //    userManager.Create(new ApplicationUser
            //    {
            //        UserName = "jenewby54n@yahoo.com",
            //        Email = "jenewby54n@yahoo.com",
            //        FirstName = "James",
            //        LastName = "Newby",
            //        DisplayName = "jenewby"
            //    }, "password");
            //}
            //if (!context.Users.Any(u => u.Email == "moderator@coderfoundry"))
            //{
            //    userManager.Create(new ApplicationUser
            //    {
            //        UserName = "moderator@coderfoundry",
            //        Email = "moderator@coderfoundry",
            //        DisplayName = "Moderator"
            //    }, "Password-1");
            //}
            //var userId = userManager.FindByEmail("jenewby54n@yahoo.com").Id;
            //userManager.AddToRole(userId, "Admin");

            //var userId2 = userManager.FindByEmail("moderator@coderfoundry").Id;
            //userManager.AddToRole(userId2, "Moderator");



        }
    }
}
