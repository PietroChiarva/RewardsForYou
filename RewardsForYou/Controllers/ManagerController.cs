using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace RewardsForYou.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {

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

      

        public ActionResult ViewTask(int? UserID = null)
        {

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                List<Tasks> task = new List<Tasks>();
                List<Missions> t = null;

                //task = db.Tasks.Where(l => l.TaskID == UserID).ToList();

                //t = db.Missions.Include(m => m.Tasks).Where(l => l.UserID == UserID).ToList();

                t = db.Missions.Include(m => m.Tasks).Where(l => l.UserID == UserID).ToList();

                foreach (Missions m in t)
                {
                    task.Add(m.Tasks);
                }

                return View(task);
            }

        }

        //View The Task
        public ActionResult AssegnaTask(int UserID)
        {

            TaskUserView tasksUsers = new TaskUserView();



            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                tasksUsers.task = db.Tasks.ToList();

            }
            tasksUsers.UsersID = UserID;

            return View(tasksUsers);

        }

        //Add the Task at Employee
        public ActionResult _DoAddTaskJson(Tasks DatiTask, int TaskID, int UserID)
        {

            TaskUserView tasksUsers = new TaskUserView();

            Tasks task = null;
            Users user = null;
            List<Missions> missions = null;
            Missions m = null;


            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                task = db.Tasks.Where(l => l.TaskID == TaskID).FirstOrDefault();
                user = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();
                missions = db.Missions.ToList();

                //controllo se il task è stato gia assegnato
                m = db.Missions.Where(l => l.TaskID == task.TaskID && l.UserID == user.UserID && l.Status == 0).FirstOrDefault();

                if (m != null)
                {
                    return Json(new { messaggio = $"Il task è stato già assegnato" });
                }
                else
                {
                    DateTime start = DateTime.Now.Date;
                    Missions mission = new Missions
                    {

                        Tasks = task,
                        Users = user,
                        UserID = user.UserID,
                        TaskID = task.TaskID,
                        StartDate = DateTime.Now,
                        EndDate = task.ExpiryDate,
                        Note = "",
                        Status = 0,
                        DesiredEndDate= start.AddMonths(task.TimeSpan)
                    };

                    db.Missions.Add(mission);
                    db.SaveChanges();
                }
            }
            return Json(new { messaggio = $"Task : {DatiTask.TaskID} assegnato con successo" });
        }

        //View The Task and Rewards
        public ActionResult ManagerTaskandReward()
        {
            ViewModel viewModel = new ViewModel();
            List<Tasks> t = null;
            List<Rewards> r = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                t = db.Tasks.ToList();
                r = db.Rewards.ToList();

               
            }
            viewModel.Task = t;
            viewModel.Reward = r;
            return View(viewModel);
        }

       public ActionResult ManagerProfile(int? UserID)
        {
            if (UserID.HasValue)
            {
                Session["UserID"] = UserID;
            }
            else
            {
                UserID = (int)Session["UserID"];
            }
            ViewModel viewModel = new ViewModel();
            Users x = null;

            {

                if (UserID.HasValue)
                {
                    string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
                    using (RewardsForYouEntities db = new RewardsForYouEntities())
                    {

                        //get the user from the db
                        x = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
                        UserID = x.UserID;
                    }
                }
                else
                {
                    using (RewardsForYouEntities db = new RewardsForYouEntities())
                    {

                        //get the user from the db
                        x = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();
                    }

                    
                }
                viewModel.User = x;
            }
            return View(viewModel);
        }


        //Details of the employee
        public ActionResult Details(int? UserID)
        {
            if (UserID.HasValue)
            {
                Session["UserID"] = UserID;
            }
            else
            {
                UserID = (int)Session["UserID"];
            }
            ViewModel viewModel = new ViewModel();
            Users x = null;
            List<Missions> t = null;
            List<Missions> g = null;
            List<Missions> mission = new List<Missions>();
            List<MissionExtended> task = new List<MissionExtended>();
            List<UsersRewards> u = null;
            List<Rewards> r = new List<Rewards>();
            Users manager = null;
            String managerUser = null;
            if (!UserID.HasValue)
            {
                string EMail = ((System.Security.Claims.ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity).Name;
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {

                    //get the user from the db
                    x = db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
                    UserID = x.UserID;
                }
            }
            else
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {

                    //get the user from the db
                    x = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();
                }
            }

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                //get tasks of the user
                task = db.Missions.Include(m => m.Tasks).Where(l => l.UserID == UserID).Select(l => new MissionExtended()
                {
                    TaskID = l.TaskID,
                    Type = l.Tasks.Type,
                    Description = l.Tasks.Description,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    DesiredEndDate = l.DesiredEndDate,
                    IsFinished = l.Tasks.Finished,
                    Points = l.Tasks.Points,
                    Note = l.Note
                }).ToList();
                g = db.Missions.Where(k => k.UserID == UserID).ToList();

                //foreach(Missions m in g)
                //{
                //    mission.Add(m);
                //}

                //get rewards of the user
                u = db.UsersRewards.Include(m => m.Rewards).Where(l => l.UserID == UserID).ToList();
                foreach (UsersRewards re in u)
                {
                    r.Add(re.Rewards);
                }

                //get the name of the manager
                manager = db.Users.Where(l => l.UserID == x.ManagerUserID).FirstOrDefault();
                managerUser = manager.Name + " " + manager.Surname;



                //save the data in the viewModel class
                viewModel.User = x;
                viewModel.Mission = task;
                viewModel.Reward = r;
                viewModel.ManagerName = managerUser;
                //viewModel.MissionDesiredDate = mission;

            }
            return View(viewModel);
        }
    }
}






//public ActionResult DetailEmployee()
//{
//    using (RewardsForYouEntities db = new RewardsForYouEntities())
//    {

//        return View();
//    }
//}




//public ActionResult _PartialTaskDetails(int UserID)
//{
//    ViewModel viewModel = new ViewModel();
//    Missions x = null;
//    Users u = null;
//    IQueryable<Tasks> t = null;

//    using (RewardsForYouEntities db = new RewardsForYouEntities())
//    {
//        //get tasks of the user
//        u= db.Users.Where(l => l.EMail == EMail).FirstOrDefault();
//        x = db.Missions.Where(l => l.UserID == UserID).FirstOrDefault();
//        t = db.Tasks.Where(l => l.TaskID == x.TaskID);

//        viewModel.User = u;
//        viewModel.Mission = t.ToList();



//    }

//    return PartialView(viewModel);
//}



/*
 * @using (Html.BeginForm("ListaUsers", "Manager", null, FormMethod.Post))
 * 
 
     <a class="btn btn-primary" onclick="ShowMissionModal ('@item.UserID', '@item.Name', '@item.Surname', '@item.EMail','@item.ManagerUserID')">Details Employee</a>
                @*<a class="btn btn-primary" onclick="ShowAddlModal ('@item.TaskID', '@item.Type','@item.Description', '@item.ExpiryDate', '@item.Points', '@item.Finished')">+</a>*@
            </td>
        </tr>
    }

</table>

<script type="text/javascript">
    function ShowMissionModal(UserID, Name, Surname, EMail, ManagerUserID) {

        $('#showModal .modal-body').load(Router.action('Manager', '_PartialMission', { UserID: UserID, Name: Name, Surname: Surname, EMail: EMail, ManagerUserID: ManagerUserID }));
        $('#showModal').modal("show");


    }

    function ShowAddModal(TaskID, Type, Description, ExpiryDate, Points, Finished) {

        $('#showModal .modal-body').load(Router.action('Manager', 'DoAddTask', { TaskID: TaskID, Type: Type, Description: Description, ExpiryDate: ExpiryDate, Points: Points, Finished: Finished}));
        $('#showModal').modal("show");


    }



</script>
     
     
     */

