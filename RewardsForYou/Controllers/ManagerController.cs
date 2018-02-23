using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RewardsForYou.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        private Tasks t;

        public object Serial { get; private set; }
        public int MissionID { get; private set; }
        public int TaskID { get; private set; }

        // GET: Manager
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaUsers()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                int userID = (int)Session["UserID"];
                List<Users> users = new List<Users>();
                users = db.Users.Where(x => x.ManagerUserID == userID).ToList();

                return View(users);
            }
        }

        public ActionResult _PartialMission(int UserID, string Name, string Surname, string EMail, int ManagerUserID)
        {
            ViewModel viewModel = new ViewModel();
            Missions x = null;
            IQueryable<Tasks> t = null;
                  
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                //get tasks of the user
                x = db.Missions.Where(l => l.UserID == UserID).FirstOrDefault();
                t = db.Tasks.Where(l => l.TaskID == TaskID);

                viewModel.Mission = t.ToList();
                

                
            }

            return PartialView(viewModel);
        }

        public ActionResult AddTask()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                return View();
            }
        }

        public ActionResult DoAddTask(int TaskID, string Type, string Description, DateTime ExpiryDate, int Points, string Finished )
        {

            if (TaskID != 0 && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Description) && !DateTime.ExpiryDate && Points != 0 && !string.IsNullOrEmpty(Finished))
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {
                    //db.Tasks.Add();

                    db.SaveChanges();

                    
                }
            }

            return View("Index");
        }

        public ActionResult DetailEmployee()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                return View();
            }
        }
   
}
   
        


    