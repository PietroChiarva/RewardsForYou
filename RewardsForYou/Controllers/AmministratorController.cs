using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Data.Entity;
using Microsoft.Ajax.Utilities;
using System.Web.Routing;




namespace RewardsForYou.Controllers
{
    [Authorize]
    public class AmministratorController : Controller
    {
        // GET: Amministrator
        public ActionResult Index()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                int userID = (int)Session["UserID"];
                return View();
            }
        }

        public ActionResult InsertNewUsers()
        {
            List<Users> user = new List<Users>();
            List<Roles> role = new List<Roles>();
            List<Users> usermanager = new List<Users>();
            //Users managerName = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                usermanager = db.Users.Where(l => l.RoleID == 2).ToList();

                //var a = new SelectList()   new SelectListItem() { Text = "A", Value = "1" } );
                //var b = new SelectListItem() { Text = "A", Value = "1" };

                // ViewBag.RoleList = new SelectList();

                //ViewBag.RoleList = new SelectList(db.Roles.Select(r=> new { Value = r.RoleID.ToString(), Text = r.Role }).ToList(), null);
                ViewBag.RoleList = db.Roles.Select(r => new SelectListItem() { Value = r.RoleID.ToString(), Text = r.Role }).ToList();

                ViewBag.ManagerList = usermanager.Select(r => new SelectListItem() { Value = r.UserID.ToString(), Text = r.Name + " " + r.Surname }).ToList();

            }
            return View();
        }

        public ActionResult _JsonInsertNewUsers(Users data)
        {

            if (!string.IsNullOrEmpty(data.Serial) && !string.IsNullOrEmpty(data.Name) && !string.IsNullOrEmpty(data.Surname) && !string.IsNullOrEmpty(data.EMail) && data.RoleID != 0 && data.ManagerUserID != 0)
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {
                    db.Users.Add(data);

                    db.SaveChanges();



                }
                return Json(new { messaggio = $"Users {data.UserID} aggiunto/a con successo", flag = true });
            }
            else
            {
                return Json(new { messaggio = $"Dati mancanti o non validi", flag = false });
            }
        }

        public ActionResult AddRewards()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                return View();
            }
        }

        public ActionResult _JsonAddRewards(Rewards data)
        {



            if (!string.IsNullOrEmpty(data.Type) && !string.IsNullOrEmpty(data.Description) && data.Points != 0 && data.Availability != 0)
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {
                    db.Rewards.Add(data);

                    db.SaveChanges();



                }
                return Json(new { messaggio = $"Rewards {data.RewardsID} aggiunto/a con successo", flag = true });
            }
            else
            {
                return Json(new { messaggio = $"Dati mancanti o non validi", flag = false });
            }
        }

        //Search and Delete Users
        public ActionResult SearchDeleteUser(SearchDeleteUser data)
        {
            //ViewModel viewModel = new ViewModel();
            //List<String> manager = new List<String>();
            //String managerUser = null;

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                IQueryable<Users> x = null;
                if (data.Serial != null)
                {
                    x = db.Users.Where(l => l.Serial == data.Serial);
                }
                else
                {
                    x = db.Users;
                }

                if (data.EMail != null)
                {
                    x = db.Users.Where(l => l.EMail == data.EMail);
                }

                //get the name of the manager
                //manager = db.Users.Where(l => l.UserID == l.ManagerUserID).ToList().Select(l=> string.Format("{0} {1}",l.Name,l.Surname)).ToList();




                data.Lista = x.ToList();

                //controllo se lo user è stato eliminato
                foreach (Users item in x)
                {
                    if (item.FiredDate != null)
                    {
                        data.Lista.Remove(item);


                    }
                }




                return View("SearchDeleteUser", data);
            }
        }

        public ActionResult _PartialDelete(string Serial, string EMail)
        {
            Users userDelete = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                userDelete = db.Users.Where(l => l.Serial == Serial && l.EMail == EMail).FirstOrDefault();

            }
            return PartialView(userDelete);
        }

        public ActionResult DoDelete(string Serial, string EMail)
        {

            Users deletedUser = null;

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                deletedUser = db.Users.Where(l => l.Serial == Serial && l.EMail == EMail).FirstOrDefault();

                //elimino(si contrassegna come licenziato) lo user selezionato
                if (deletedUser != null)
                {

                    deletedUser.FiredDate = DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("SearchDeleteUser", new SearchDeleteUser());
        }
        public ActionResult _PartialUpdateUser(string Serial, string EMail)
        {
            List<Roles> role = new List<Roles>();
            List<Users> usermanager = new List<Users>();
            Users updateUser = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                updateUser = db.Users.Where(l => l.Serial == Serial && l.EMail == EMail).FirstOrDefault();
                usermanager = db.Users.Where(l => l.RoleID == 2).ToList();

                
                ViewBag.RoleList = db.Roles.Select(r => new SelectListItem() { Value = r.RoleID.ToString(), Text = r.Role }).ToList();

                ViewBag.ManagerList = usermanager.Select(r => new SelectListItem() { Value = r.UserID.ToString(), Text = r.Name + " " + r.Surname }).ToList();
            }
                return PartialView(updateUser);
        }

        public ActionResult DoUpdateUser(Users data)
        {
            Users userUpdate = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                userUpdate = db.Users.Where(l => l.UserID == data.UserID).FirstOrDefault();
                userUpdate.Serial = data.Serial;
                userUpdate.Name = data.Name;
                userUpdate.Surname = data.Surname;
                userUpdate.UserPoints = data.UserPoints;
               // userUpdate.ManagerUserID = data.ManagerUserID;
                userUpdate.EMail = data.EMail;
                //userUpdate.RoleID = data.RoleID;
                db.SaveChanges();
            }
                return RedirectToAction("SearchDeleteUser", new SearchDeleteUser());
        }

        //Search and Delete Rewards
        public ActionResult SearchDeleteRewards(SearchDeleteReward data)
        {

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                IQueryable<Rewards> x = null;
                if (data.Description != null)
                {
                    x = db.Rewards.Where(l => l.Description == data.Description);
                }
                else
                {
                    x = db.Rewards;
                }




                data.Lista = x.ToList();



                return View("SearchDeleteRewards", data);
            }
        }

        public ActionResult _PartialDeleteRewards(string Description)
        {
            Rewards rewardDelete = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                rewardDelete = db.Rewards.Where(l => l.Description == Description).FirstOrDefault();

            }
            return PartialView(rewardDelete);
        }

        public ActionResult DoDeleteReward(string Description)
        {

            Rewards deletedReward = null;
            UsersRewards userReward = null;
            NoticeRewardsTakes noticeReward = null;

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                deletedReward = db.Rewards.Where(l => l.Description == Description).FirstOrDefault();
                userReward = db.UsersRewards.Where(l => l.RewardsID == deletedReward.RewardsID).FirstOrDefault();
                noticeReward = db.NoticeRewardsTakes.Where(l => l.RewardsID == deletedReward.RewardsID).FirstOrDefault();


                if ((deletedReward != null && userReward != null) || (noticeReward != null) || (userReward != null))
                {
                    TempData["msg"] = "<script>alert('Il Reward non può esere cancellato perchè un impiegato ne è in possesso o è in attesa di essere ricevuto');</script>";
                   
                }
                else
                {
                    db.Rewards.Remove(deletedReward);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("SearchDeleteRewards", new SearchDeleteReward());
        }
        public ActionResult _PartialUpdateRewards(string Description)
        {
            Rewards updateReward = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                updateReward = db.Rewards.Where(l => l.Description == Description).FirstOrDefault();
            }
                return PartialView(updateReward);
        }

        public ActionResult DoUpdateReward(Rewards data)
        {
            Rewards updateReward = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                updateReward = db.Rewards.Where(l => l.RewardsID == data.RewardsID).FirstOrDefault();
                updateReward.Type = data.Type;
                updateReward.Description = data.Description;
                updateReward.Availability = data.Availability;
                updateReward.Points = data.Points;
                db.SaveChanges();
            }
            return RedirectToAction("SearchDeleteRewards", new SearchDeleteReward());
        }
        //Search and Delete Tasks
        public ActionResult SearchDeleteTasks(SearchDeleteTask data)
        {

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                IQueryable<Tasks> x = null;
                if (data.Description != null)
                {
                    x = db.Tasks.Where(l => l.Description == data.Description);
                }
                else
                {
                    x = db.Tasks;
                }
                if (data.Type != null)
                {
                    x = db.Tasks.Where(l => l.Type == data.Type);
                }




                data.Lista = x.ToList();

                return View("SearchDeleteTasks", data);
            }
        }

        public ActionResult _PartialDeleteTasks(string Description, string Type)
        {
            Tasks taskDelete = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                taskDelete = db.Tasks.Where(l => l.Description == Description && l.Type == Type).FirstOrDefault();

            }
            return PartialView(taskDelete);
        }
        public ActionResult DoDeleteTask(string Description, string Type)
        {

            Tasks deletedTask = null;
            Missions deletedMission = null;
            NoticeMissionEnded noticeMission = null;

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                deletedTask = db.Tasks.Where(l => l.Description == Description && l.Type == Type).FirstOrDefault();
                deletedMission = db.Missions.Where(l => l.TaskID == deletedTask.TaskID).FirstOrDefault();
                noticeMission = db.NoticeMissionEnded.Where(l => l.MissionID == deletedMission.MissionID).FirstOrDefault();

                if ((deletedTask != null && deletedMission != null) || (noticeMission != null))
                {
                    TempData["msg"] = "<script>alert('Il Task non può esere cancellato perchè è stato eseguito o è in fase di esecuzione');</script>";
                }
                else
                {
                    db.Tasks.Remove(deletedTask);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("SearchDeleteTasks", new SearchDeleteTask());
        }
         public ActionResult _PartialUpdateView(string Description, string Type)
        {
            Tasks updateTask = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                updateTask = db.Tasks.Where(l => l.Description == Description && l.Type == Type).FirstOrDefault();
            }

                return PartialView(updateTask);
        }

        public ActionResult DoUpdate(Tasks data)
        {
            Tasks updateTask = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                updateTask = db.Tasks.Where(l => l.Description == data.Description && l.Type == data.Type).FirstOrDefault();

                updateTask.Description = data.Description;
                updateTask.Type = data.Type;
                updateTask.TimeSpan = data.TimeSpan;
                updateTask.Points = data.Points;
                updateTask.ExpiryDate = data.ExpiryDate;
                db.SaveChanges();
            }



            return RedirectToAction("SearchDeleteTasks", new SearchDeleteTask());
        }

        public ActionResult TakeJsonUsers(string Serial, string EMail)
        {
            List<Users> users = null;

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                users = db.Users.Where(l => l.FiredDate != null).ToList();


                foreach (Users u in users)
                {
                    if ((u.Serial == Serial || u.EMail == EMail) || (u.Serial == Serial && u.EMail == EMail))
                    {

                        return Json(new { message = $"L'utente è stato licenziato in data: {u.FiredDate}" });
                    }

                }

            }

            return SearchDeleteUser(new SearchDeleteUser());
        }

        public ActionResult AddTask()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                return View();
            }
        }

        public ActionResult JsonAddTask(Tasks data)
        {

            if (!string.IsNullOrEmpty(data.Type) && !string.IsNullOrEmpty(data.Description) && data.ExpiryDate.HasValue && data.TimeSpan != 0 && data.Points != 0)
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {
                    db.Tasks.Add(data);

                    db.SaveChanges();


                }

                return Json(new { messaggio = $"Task {data.TaskID} aggiunto/a con successo", flag = true });
            }


            else
            {
                return Json(new { messaggio = $"Dati mancanti o non validi", flag = false });
            }

        }



    }
}