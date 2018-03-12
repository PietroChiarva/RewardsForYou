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
               
                ViewBag.ManagerList = usermanager.Select(r=> new SelectListItem() { Value = r.UserID.ToString(), Text = r.Name }).ToList();
                
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
                return Json(new { messaggio = $"Dati mancanti o non validi" , flag = false});
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
                return Json(new { messaggio = $"Rewards {data.RewardsID} aggiunto/a con successo",flag = true });
            }
            else
            {
                return Json(new { messaggio = $"Dati mancanti o non validi", flag = false });
            }
        }

        public ActionResult SearchDeleteUser(SearchDeleteUser data)
        {
            
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

                    if (data.EMail!= null)
                    {
                        x = db.Users.Where(l => l.EMail == data.EMail);
                    }
                    

                data.Lista = x.ToList();

                //controllo se lo user è stato eliminato
                foreach(Users item in x)
                {
                    if(item.FiredDate != null)  
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
            SearchDeleteUser users = new SearchDeleteUser();
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                deletedUser = db.Users.Where(l => l.Serial == Serial && l.EMail == EMail).FirstOrDefault();
                
                //elimino(si contrassegna come licenziato) lo user selezionato
                if(deletedUser != null)
                {
                    deletedUser.FiredDate = DateTime.Now;
                    db.SaveChanges();
                }
                users.Lista = db.Users.ToList();
            }
            return RedirectToAction("SearchDeleteUser", users);
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

                        return Json(new { message = $"L'utente è stato licenziato in data: {u.FiredDate}"});
                    }

                }

            }
            
            return SearchDeleteUser(new SearchDeleteUser());
        }

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

                
            }
            return View("SearchDeleteRewards", data);
        }

       public ActionResult _PartialDeleteRewards(string Description)
        {
            Rewards reward = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                reward = db.Rewards.Where(l => l.Description == Description).FirstOrDefault();
            }
                return PartialView(reward);
        }

        public ActionResult DoDeleteReward(string Description)
        {
            Rewards rewardDeleteted = null;
            List<UsersRewards> usersRewards = null;
            SearchDeleteReward rewards = new SearchDeleteReward();
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                rewardDeleteted = db.Rewards.Where(l => l.Description == Description).FirstOrDefault();
                usersRewards = db.UsersRewards.Where(l => l.RewardsID == rewardDeleteted.RewardsID).ToList();
               
                if(rewardDeleteted != null && usersRewards != null)
                {
                    db.Rewards.Remove(rewardDeleteted);
                    foreach (UsersRewards item in usersRewards)
                    {
                        db.UsersRewards.Remove(item);
                    }
                    db.SaveChanges();
                }
                rewards.Lista = db.Rewards.ToList();
            }
                return RedirectToAction("SearchDeleteRewards",rewards);
        }


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
                if(data.Type != null)
                {
                    x = db.Tasks.Where(l => l.Type == data.Type);
                }
                data.Lista = x.ToList();


            }
            return View("SearchDeleteTasks", data);
        }

        public ActionResult _PartialDeleteTasks(string Description, string Type)
        {
            Tasks task = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                task = db.Tasks.Where(l => l.Description == Description && l.Type == Type).FirstOrDefault();
            }
            return PartialView(task);
        }

        public ActionResult DoDeleteTask(string Description, string Type)
        {
            Tasks taskDeleted = null;
            List<Missions> mission = null;
            SearchDeleteTask task = new SearchDeleteTask();
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                taskDeleted = db.Tasks.Where(l => l.Description == Description && l.Type == Type).FirstOrDefault();
                mission = db.Missions.Where(l => l.TaskID == taskDeleted.TaskID).ToList();

                if (taskDeleted != null && mission != null)
                {
                    db.Tasks.Remove(taskDeleted);
                    foreach (Missions item in mission)
                    {
                        db.Missions.Remove(item);
                    }
                    db.SaveChanges();
                }
                task.Lista = db.Tasks.ToList();
            }
            return RedirectToAction("SearchDeleteTasks", task);
        }

    }
}