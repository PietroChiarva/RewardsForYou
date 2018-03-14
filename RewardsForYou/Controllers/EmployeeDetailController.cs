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
    public class EmployeeDetailController : Controller
    {
        // GET: Employee
        public ActionResult Index(int? UserID)
        {
            if (UserID.HasValue)
            {
                Session["UserID"] = UserID;
            }
            else
            {
                UserID = (int)Session["UserID"];
            }
            ViewModel viewModel = new ViewModel();
            MissionModel missionModel = new MissionModel();
            Users x = null;
            List<Missions> t = null;
            List<Tasks> task = new List<Tasks>();
            List<UsersRewards> u = null;
            List<Rewards> r = new List<Rewards>();
            Users manager = null;
            String managerUser = null;

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

                //get the name of the manager
                manager = db.Users.Where(l => l.UserID == x.ManagerUserID).FirstOrDefault();
                managerUser = manager.Name + " " + manager.Surname;

                //save the data in the viewModel class
                viewModel.User = x;
                viewModel.Mission = task;
                viewModel.Reward = r;
                viewModel.ManagerName = managerUser;
            }

            return View(viewModel);
        }

      


    }


}