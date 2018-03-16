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
    public class EmployeeDetails : Controller
    {
        // GET: EmployeeDetails
        public ActionResult Details(int? UserID)
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
            Users x = null;
            List<Missions> t = null;
            List<Missions> g = null;
            List<Missions> mission = new List<Missions>();
            List<MissionExtended> task = new List<MissionExtended>();
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
                task = db.Missions.Include(m => m.Tasks).Where(l => l.UserID == UserID).Select(l => new MissionExtended()
                {
                    TaskID = l.TaskID,
                    Type = l.Tasks.Type,
                    Description = l.Tasks.Description,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    DesiredEndDate = l.DesiredEndDate,
                    IsFinished = l.Tasks.Finished,
                    Points = l.Tasks.Points,
                    Note = l.Note
                }).ToList();
                g = db.Missions.Where(k => k.UserID == UserID).ToList();

                //foreach(Missions m in g)
                //{
                //    mission.Add(m);
                //}

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
                //viewModel.MissionDesiredDate = mission;

            }
            return View(viewModel);
        }
    }
}