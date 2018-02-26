using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace RewardsForYou.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index(int? UserID = null)
        {
            ViewModel viewModel = new ViewModel();
            MissionModel missionModel = new MissionModel();
            Users x = null;
            List<Missions> t = null;
            List<Tasks> task = new List<Tasks>();
            UsersRewards u = null;
            Rewards r = null;

            if (!UserID.HasValue)
            {
                string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {

                    //get the user from the db
                    x = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
                    UserID = x.UserID;
                }
            }
            else
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {

                    //get the user from the db
                    x = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();
                }
            }

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                //get tasks of the user
                t = db.Missions.Include(m => m.Tasks).Where(l => l.UserID == UserID).ToList();

                //t = db.Missions.Where(l => l.UserID == UserID).ToList();
                foreach (Missions m in t)
                {
                    // missionModel.Mission = db.Missions.Where(l => l.TaskID == t[i].TaskID).ToList();
                    task.Add(m.Tasks);
                }

                //get rewards of the user
                u = db.UsersRewards.Where(l => l.UserID == UserID).FirstOrDefault();
                r = db.Rewards.Where(l => l.RewardsID == u.RewardsID).FirstOrDefault();

                //save the data in the viewModel class
                viewModel.User = x;
                viewModel.Mission = task;
                viewModel.Reward = r;
            }

            return View(viewModel);
        }

        //public ActionResult _UserDetails()
        //{
        //    Users x = null;
        //    string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
        //    using (RewardsForYouEntities db = new RewardsForYouEntities())
        //    {


        //        x = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();

        //    }
        //    return PartialView(x);
        //}

        //public ActionResult _UserTasks()
        //{
        //    Users x = null;
        //    Missions t = null;
        //    string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
        //    using (RewardsForYouEntities db = new RewardsForYouEntities())
        //    {


        //        x = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();

        //        t = db.Missions.Where(l => l.UserID == x.UserID).FirstOrDefault();
        //    }
        //    return View(t);
        //}

        //public ActionResult _UserRewards()
        //{
        //    Users x = null;
        //    UsersRewards u = null;
        //    Rewards r = null;
        //    string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
        //    using (RewardsForYouEntities db = new RewardsForYouEntities())
        //    {


        //        x = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();

        //        u = db.UsersRewards.Where(l => l.UserID == x.UserID).FirstOrDefault();

        //        r = db.Rewards.Where(l => l.RewardsID == u.RewardsID).FirstOrDefault();

        //    }
        //    return PartialView(r);


        //}




    }
}