using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RewardsForYou.Models;

namespace RewardsForYou.Controllers
{
    public class ManagerController : Controller
    {
        private RewardsForYouEntities db = new RewardsForYouEntities();

        // GET: Manager
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Roles).Include(u => u.Users1).Include(u => u.Users2);
            return View(users.ToList());
        }
     } 
}

//        // GET: Manager/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Users users = db.Users.Find(id);
//            if (users == null)
//            {
//                return HttpNotFound();
//            }
//            return View(users);
//        }

//        // GET: Manager/Create
//        public ActionResult Create()
//        {
//            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Role");
//            ViewBag.UserID = new SelectList(db.Users, "UserID", "Serial");
//            ViewBag.UserID = new SelectList(db.Users, "UserID", "Serial");
//            return View();
//        }

//        // POST: Manager/Create
//        // Per proteggere da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
//        // Per ulteriori dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "UserID,Serial,Name,Surname,EMail,RoleID,ManagerUserID")] Users users)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Users.Add(users);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Role", users.RoleID);
//            ViewBag.UserID = new SelectList(db.Users, "UserID", "Serial", users.UserID);
//            ViewBag.UserID = new SelectList(db.Users, "UserID", "Serial", users.UserID);
//            return View(users);
//        }

//        // GET: Manager/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Users users = db.Users.Find(id);
//            if (users == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Role", users.RoleID);
//            ViewBag.UserID = new SelectList(db.Users, "UserID", "Serial", users.UserID);
//            ViewBag.UserID = new SelectList(db.Users, "UserID", "Serial", users.UserID);
//            return View(users);
//        }

//        // POST: Manager/Edit/5
//        // Per proteggere da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
//        // Per ulteriori dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "UserID,Serial,Name,Surname,EMail,RoleID,ManagerUserID")] Users users)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(users).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Role", users.RoleID);
//            ViewBag.UserID = new SelectList(db.Users, "UserID", "Serial", users.UserID);
//            ViewBag.UserID = new SelectList(db.Users, "UserID", "Serial", users.UserID);
//            return View(users);
//        }

//        // GET: Manager/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Users users = db.Users.Find(id);
//            if (users == null)
//            {
//                return HttpNotFound();
//            }
//            return View(users);
//        }

//        // POST: Manager/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            Users users = db.Users.Find(id);
//            db.Users.Remove(users);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
