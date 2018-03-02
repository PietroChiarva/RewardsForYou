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

                ViewBag.RoleId = new SelectList(role, "Id", "Name");

            }
            return View();
        }

        public ActionResult _JsonInsertNewUsers(Users DatiUsers)
        {
            if (DatiUsers.UserID != 0 && !string.IsNullOrEmpty(DatiUsers.Serial) && !string.IsNullOrEmpty(DatiUsers.Name) && !string.IsNullOrEmpty(DatiUsers.Surname) && !string.IsNullOrEmpty(DatiUsers.EMail) && DatiUsers.RoleID != 0 && DatiUsers.ManagerUserID != 0)
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {
                    db.Users.Add(DatiUsers);

                    db.SaveChanges();



                }
            }
            return Json(new { messaggio = $"Users {DatiUsers.UserID} aggiunto/a con successo" });
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
            if (DatiRewards.RewardsID != 0 && !string.IsNullOrEmpty(DatiRewards.Type) && !string.IsNullOrEmpty(DatiRewards.Description) && DatiRewards.Points !=0 && DatiRewards.Availability !=0)
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