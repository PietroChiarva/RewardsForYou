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

        public ActionResult _JsonInsertNewUsers(Users DatiUsers)
        {
            Users userID = null;
            
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                userID = db.Users.Find(Session["UserID"]);
                DatiUsers.UserID = userID.UserID;
                

            }
            if (DatiUsers.UserID != 0 && !string.IsNullOrEmpty(DatiUsers.Serial) && !string.IsNullOrEmpty(DatiUsers.Name) && !string.IsNullOrEmpty(DatiUsers.Surname) && !string.IsNullOrEmpty(DatiUsers.EMail) && DatiUsers.RoleID != 0 && DatiUsers.ManagerUserID != 0)
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {
                    db.Users.Add(DatiUsers);

                    db.SaveChanges();



                }
                
               
            }
           
            return View("Index");
         
        }


        public ActionResult AddRewards()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                return View();
            }
        }

        public ActionResult _JsonAddRewards(Rewards DatiRewards)
        {
            if (DatiRewards.RewardsID != 0 && !string.IsNullOrEmpty(DatiRewards.Type) && !string.IsNullOrEmpty(DatiRewards.Description) && DatiRewards.Points != 0 && DatiRewards.Availability != 0)
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {
                    db.Rewards.Add(DatiRewards);

                    db.SaveChanges();



                }
            }
            return Json(new { messaggio = $"Rewards {DatiRewards.RewardsID} aggiunto/a con successo" });
        }




    }
}