using RewardsForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using System.Data.Services.Client;
using System.IO;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RewardsForYou.Domain;
using RewardsForYou.Controllers;

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
                        DesiredEndDate = start.AddMonths(task.TimeSpan)
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

            List<MissionExtended> mission = new List<MissionExtended>();




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
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {


                // status 0 = accettato, 1= rifiutato, 2= in sospeso
                mission = db.NoticeMissionEnded.Include(n => n.Missions)
                    .Include(n => n.Users).Where(l => l.ManagerID == UserID && l.Status == 2)
                    .Select(l => new MissionExtended()
                    {
                        TaskID = l.Missions.Tasks.TaskID,
                        Type = l.Missions.Tasks.Type,
                        Description = l.Missions.Tasks.Description,
                        StartDate = l.Missions.StartDate,
                        EndDate = l.Missions.EndDate,
                        DesiredEndDate = l.Missions.DesiredEndDate,
                        IsFinished = l.Missions.Tasks.Finished,
                        Points = l.Missions.Tasks.Points,
                        Note = l.Missions.Note,
                        UserName = l.Users.Name + " " + l.Users.Surname,
                        UserID = l.Users.UserID
                    })
                    .ToList();


            }
            viewModel.User = x;
            viewModel.Mission = mission;

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

        // status 0 = accettato, 1= rifiutato, 2= in sospeso
        public ActionResult AcceptMission(int TaskID, int UserID)
        {
            Missions mission = null;
            NoticeMissionEnded noticeMission = null;
            Tasks task = null;
            Users user = null;

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                mission = db.Missions.Where(l => l.TaskID == TaskID && l.UserID == UserID).FirstOrDefault();
                noticeMission = db.NoticeMissionEnded.Where(l => l.MissionID == mission.MissionID && l.UserID == UserID).FirstOrDefault();
                task = db.Tasks.Where(l => l.TaskID == TaskID).FirstOrDefault();
                user = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();
                if (mission != null && noticeMission != null)
                {
                    user.UserPoints = user.UserPoints + task.Points;
                    mission.Status = 1;
                    task.Finished = true;
                    noticeMission.Status = 0;
                    db.SaveChanges();
                    return Json(new { message = $"Missione accettata con successo", flag = true });

                }
            }

            return Json(new { message = $"Missione non accettata per qualche problema", flag = false });
        }

        public ActionResult RefuseNote(int TaskID, int UserID)
        {
            NoticeMissionEnded notice = null;
            Missions mission = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                mission = db.Missions.Where(l => l.TaskID == TaskID && l.UserID == UserID).FirstOrDefault();
                notice = db.NoticeMissionEnded.Where(l => l.UserID == UserID && l.MissionID == mission.MissionID).FirstOrDefault();
            }
                return PartialView(notice);
        }

        public ActionResult DoRefuse(int MissionID, int UserID)
        {
            NoticeMissionEnded notice = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                notice = db.NoticeMissionEnded.Where(l => l.MissionID == MissionID && l.UserID == UserID).FirstOrDefault();
                notice.Status = 1;
                db.SaveChanges();
            }

            return RedirectToAction("ManagerProfile", notice.ManagerID);
        }




        //UserImage
        [HttpPost]
        public async Task<JsonResult> GetAADUserImageAsync()
        {
            JsonResult ret = null;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            try
            {
                string graphResourceID = "https://graph.windows.net/";
                UserProfileController userProfileController = new UserProfileController();

                Uri servicePointUri = new Uri(graphResourceID);
                Uri serviceRoot = new Uri(servicePointUri, tenantID);
                ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                      async () => await userProfileController.GetTokenForApplication());

                // use the token for querying the graph to get the user details

                var result = await activeDirectoryClient.Users
                    .Where(u => u.ObjectId.Equals(userObjectID))
                    .ExecuteAsync();
                IUser user = result.CurrentPage.ToList().First();

                DataServiceStreamResponse photo = await user.ThumbnailPhoto.DownloadAsync();
                using (MemoryStream s = new MemoryStream())
                {
                    photo.Stream.CopyTo(s);
                    var encodedImage = Convert.ToBase64String(s.ToArray());
                    ret = Json(new
                    {
                        Success = true,
                        Base64StringImage = String.Format("data:image/gif;base64,{0}", encodedImage)
                    });
                }
            }
            catch (AdalException e)
            {
                ret = Json(new
                {
                    Success = false,
                    Message = e.Message
                });
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (Exception e)
            {
                ret = Json(new
                {
                    Success = false,
                    Message = e.Message
                });
            }
            return ret;
        }


    }
}



