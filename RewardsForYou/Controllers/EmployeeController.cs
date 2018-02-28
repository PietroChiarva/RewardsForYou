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
            List<UsersRewards> u = null;
            List<Rewards> r = new List<Rewards>();

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
                
                foreach (Missions m in t)
                {
                    
                    task.Add(m.Tasks);
                }

                //get rewards of the user
                u = db.UsersRewards.Include(m => m.Rewards).Where(l => l.UserID == UserID).ToList();
                foreach (UsersRewards re in u)
                {
                    r.Add(re.Rewards);
                }
                //save the data in the viewModel class
                viewModel.User = x;
                viewModel.Mission = task;
                viewModel.Reward = r;
            }

            return View(viewModel);
        }

        public ActionResult _ChooseRewards()
        {
            //take all the rewards
            List<Rewards> rew = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                rew = db.Rewards.ToList();
                
            }
            return PartialView(rew);
        }

        public ActionResult _PartialTakeReward(int RewardsID)
        {
            Rewards reward = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                reward = db.Rewards.Where(l => l.RewardsID == RewardsID).FirstOrDefault();
            }
                return View(reward);
        }

        public ActionResult Take(int RewardID)
        {
            return View("Index");
        }
    }

   
}