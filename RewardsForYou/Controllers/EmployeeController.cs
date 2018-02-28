﻿using RewardsForYou.Models;
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
            Session["UserID"] = UserID;
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
            int UserID = 3;
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
            UsersRewards userReward = null;
            int newUserPoint = 0;
            Users userUpdated = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                reward = db.Rewards.Where(l => l.RewardsID == RewardsID).FirstOrDefault();
                user = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();

                //check if the points of the user are enough for the selected reward
                if(user.UserPoints >= reward.Points)
                {
                    //sottrazione dei punti allo user
                    newUserPoint = (int)user.UserPoints - reward.Points;
                    userUpdated = new Users();
                    userUpdated.UserPoints = newUserPoint;
                    userUpdated.Serial = user.Serial;
                    userUpdated.Name = user.Name;
                    userUpdated.Surname = user.Surname;
                    userUpdated.UserID = user.UserID;
                    userUpdated.RoleID = user.RoleID;
                    userUpdated.ManagerUserID = user.ManagerUserID;
                    userUpdated.EMail = user.EMail;
                    db.Users.Add(userUpdated);

                    //Inserisco il nuovo reward dell'utente nel db
                    userReward.UserID = user.UserID;
                    userReward.RewardsID = reward.RewardsID;
                    userReward.Note = "";
                    userReward.RewardsDate = new DateTime();
                    db.UsersRewards.Add(userReward);
                    db.SaveChanges();
                    return Json(new { messaggio = $"{reward.Type} aggiunto/a con successo" });
                }

                else
                {
                    return Json(new { messaggio = $"I punti non sono sufficienti" });
                }
            }
                
        }

        
    }

   
}