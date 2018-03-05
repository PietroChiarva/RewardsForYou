using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.Ajax.Utilities;

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
          
            if (!string.IsNullOrEmpty(data.Serial) && !string.IsNullOrEmpty(data.Name) && !string.IsNullOrEmpty(data.Surname) && !string.IsNullOrEmpty(data.EMail) && data.RoleID != 0 && data.ManagerUserID != 0)
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {
                    db.Users.Add(data);

                    db.SaveChanges();
                    


                }
                return Json(new { messaggio = $"Users {data.UserID} aggiunto/a con successo" });
            }
            else
            {
                return Json(new { messaggio = $"Dati mancanti o non validi" });
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
                return Json(new { messaggio = $"Rewards {data.RewardsID} aggiunto/a con successo" });
            }
            else
            {
                return Json(new { messaggio = $"Dati mancanti o non validi" });
            }
        }




    }
}