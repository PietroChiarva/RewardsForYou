using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RewardsForYou.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            int userId = 0;
            int roleId = 0;
            string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                var d = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
                if(d != null)
                {
                    Session["UserID"] = d.UserID;
                    roleId = d.RoleID;
                    userId = d.UserID;
                }

            }
            ViewData["Employee"] = false;
            ViewData["Manager"] = false;
            ViewData["Amministrator"] = false;
            if (roleId == 1)
            {
                ViewData["Employee"] = true;
                ViewData["Manager"] = true;

            }
            else if(roleId == 2)
            {
                ViewData["Employee"] = true;
                ViewData["Amministrator"] = true;
            }
            else if(roleId == 3)
            {
                ViewData["Manager"] = true;
                ViewData["Amministrator"] = true;
            }


            return View();
        }

     
      
    }
}



