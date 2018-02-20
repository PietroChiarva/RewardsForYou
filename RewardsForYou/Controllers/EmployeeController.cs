using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RewardsForYou.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            Users x = null;
            string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                
                x = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
                
               
                
                

            }
                return View(x);
        }


    }
}