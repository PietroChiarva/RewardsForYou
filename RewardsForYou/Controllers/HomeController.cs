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
        return View();
    }

      

    }
}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult Login(Users objUser)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            using (RewardsForYouEntities db = new RewardsForYouEntities())
    //            {
    //                var obj = db.Users.Where(a => a.Name.Equals(objUser.Name) && a.Serial.Equals(objUser.Serial)).FirstOrDefault();
    //                if (obj != null)
    //                {
    //                    Session["UserID"] = obj.UserID.ToString();
    //                    Session["UserName"] = obj.Name.ToString();
    //                    return RedirectToAction("UserDashBoard");
    //                }
    //            }
    //        }
    //        return View(objUser);
    //    }

//    public ActionResult UserDashBoard()
//    {
//        if (Session["UserID"] != null)
//        {
//            return View();
//        }
//        else
//        {
//            return RedirectToAction("Login");
//        }
//    }
//}


//public ActionResult About()
//{
//    ViewBag.Message = "Your application description page.";

//    return View();
//}


//public ActionResult Contact()
//{
//    ViewBag.Message = "Your contact page.";

//    return View();
//}

