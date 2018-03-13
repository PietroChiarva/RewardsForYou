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
               
                ViewBag.ManagerList = usermanager.Select(r=> new SelectListItem() { Value = r.UserID.ToString(), Text = r.Name + " "+ r.Surname }).ToList();
                
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

            
            
            if (!string.IsNullOrEmpty(data.Type) && !string.IsNullOrEmpty(data.Description)  && data.Points != 0 && data.Availability != 0)
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

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                deletedUser = db.Users.Where(l => l.Serial == Serial && l.EMail == EMail).FirstOrDefault();
                
                //elimino(si contrassegna come licenziato) lo user selezionato
                if(deletedUser != null)
                {
                    deletedUser.FiredDate = DateTime.Now;
                    db.SaveChanges();
                }
            }
                return SearchDeleteUser(new SearchDeleteUser());
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

        public ActionResult AddTask()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                return View();
            }
        }

        public ActionResult JsonAddTask(Tasks data)
        {

            if (!string.IsNullOrEmpty(data.Type) && !string.IsNullOrEmpty(data.Description) && data.ExpiryDate.HasValue && data.Points != 0 )
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