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
                manager = db.Users.Where(l => l.UserID == x.ManagerUserID).FirstOrDefault();
                managerUser = manager.Name+ " "+ manager.Surname;
                
              

                //save the data in the viewModel class
                viewModel.User = x;
                viewModel.Mission = task;
                viewModel.Reward = r;
                viewModel.ManagerName = managerUser;
            }

            return View(viewModel);
        }

        public ActionResult _ChooseRewards()
        {
            int UserID = 0;
            if (Session["UserID"] != null)
            {
                UserID = (int)Session["UserID"];
            }
            //take all the rewards
            UserRewardModel userReward = new UserRewardModel();
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                userReward.rewards = db.Rewards.ToList();
                
            }
            userReward.UserID = UserID;
            return PartialView(userReward);
        }

        public ActionResult _PartialTakeReward(int RewardsID, int UserID)
        {
            Rewards reward = null;
            Users user = null;
            UsersRewards userReward = new UsersRewards();
            Rewards availabilityReward = new Rewards();           
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                Users userUpdated = db.Users.Find(UserID);
                availabilityReward = db.Rewards.Find(RewardsID);
                reward = db.Rewards.Where(l => l.RewardsID == RewardsID).FirstOrDefault();
                user = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();

                //check if the points of the user are enough for the selected reward
                if(user.UserPoints >= reward.Points)
                {
                    //sottrazione dei punti allo user
                    userUpdated.UserPoints = user.UserPoints - reward.Points;

                    //diminuzione dell'availability del reward
                    availabilityReward.Availability = availabilityReward.Availability - 1;

                    //Inserisco il nuovo reward dell'utente nel db
                    userReward.UserID = user.UserID;
                    userReward.RewardsID = reward.RewardsID;
                    userReward.Note = "";
                    userReward.RewardsDate = DateTime.Now;
                    db.UsersRewards.Add(userReward);
                    db.SaveChanges();
                    return Json(new { messaggio = $"{reward.Type} aggiunto/a con successo", flag = true });
                }

                else
                {
                    return Json(new { messaggio = $"I punti non sono sufficienti", flag = false });
                }
            }
                
        }

        
    }

   
}