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
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                //var a = new SelectList()   new SelectListItem() { Text = "A", Value = "1" } );
                //var b = new SelectListItem() { Text = "A", Value = "1" };

                // ViewBag.RoleList = new SelectList();

                //ViewBag.RoleList = new SelectList(db.Roles.Select(r=> new { Value = r.RoleID.ToString(), Text = r.Role }).ToList(), null);
                ViewBag.RoleList = db.Roles.Select(r => new SelectListItem() { Value = r.RoleID.ToString(), Text = r.Role }).ToList();
            }
            return View();
        }

        public ActionResult _JsonInsertNewUsers(Users data)
        {
            //Users userID = null;

            //using (RewardsForYouEntities db = new RewardsForYouEntities())
            //{
            //    userID = db.Users.Find(Session["UserID"]);
            //    data.UserID = userID.UserID;


            //}
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

        public ActionResult SearchDeleteUser(SearchDeleteUser data)
        {
            
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
              
                    IQueryable<Users> x = null;
                    if (data.Serial != null)
                    {
                    x = x.Where(l => l.Serial == data.Serial); 
                    }
                    else
                    {
                        x = db.Users;
                    }

                    if (data.EMail!= null)
                    {
                        x = x.Where(l => l.EMail == data.EMail);
                    }
                    

                data.Lista = x.ToList();

                

                return View("SearchDeleteUser", data);
            }
        }




    }
}