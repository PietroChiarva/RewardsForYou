using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RewardsForYou.Controllers
{
    public class ManagerController : Controller
    {
        public object Serial { get; private set; }

        // GET: Manager
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaUsers()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                List<Users> users = new List<Users>();
                users = db.Users.Where(x => x.ManagerUserID == (int) Session["UserID"]).ToList();

                return View(users);
            }
        }
    }
}


    //IQueryable<RewardsForYouEntities> x = null;

    //            if (Models.users.UsersID != 0)
    //            {
    //                x = db.RewardsForYouEntities.Where((object l) => l.UsersID == users.UsersID);
    //            }
    //            else
    //            {
    //                x = db.RewardsForYouEntities;
    //            }

    //            if (users.Serial != null)
    //            {
    //                x = x.Where(l => Serial == users.Serial);
    //            }
    //            if (users.Name != null)
    //            {
    //                x = x.Where(p => p.Name == users.Name);
    //            }
    //            if (users.Surname != null)
    //            {
    //                x = x.Where(p => p.Surname == users.Surname);
    //            }
    //            if (users.EMail!= null)
    //            {
    //                x = x.Where(p => p.EMail == users.EMail);
    //            }
    //            if (users.RoleID != null)
    //            {
    //                x = x.Where(p => p.RoleID == users.RoleID);
    //            }
    //            if (users.ManagerUserID != null)
    //            {
    //                x = x.Where(p => p.ManagerUserID == users.ManagerUserID);
    //            }


    //            users.ResultList = x.ToList();
    //        }