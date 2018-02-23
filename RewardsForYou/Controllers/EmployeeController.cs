using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RewardsForYou.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            ViewModel viewModel = new ViewModel();
            MissionModel missionModel = new MissionModel();
            Users x = null;
           List<Missions> t = null;
           List<Tasks> task = null;
            UsersRewards u = null;
            Rewards r = null;
            
            string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                //get the user from the db
                x = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
                //get tasks of the user
                t = db.Missions.Where(l => l.UserID == x.UserID).ToList();
                for(int i=0; i<t.Count; i++)
                {
                    missionModel.Mission = db.Missions.Where(l => l.TaskID == t[i].TaskID).ToList();
                    task = db.Tasks.Where(l => l.TaskID == missionModel.Mission[i].TaskID).ToList();
                }
                
                //get rewards of the user
                u = db.UsersRewards.Where(l => l.UserID == x.UserID).FirstOrDefault();
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