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

                t = db.Missions.Include(m => m.Tasks).Where(l => l.UserID == UserID && l.Tasks.Finished == false).ToList();
                
                foreach (Missions m in t)
                {
                    if (m.Status == 0)
                    {
                        task.Add(m.Tasks);
                    }
                }

                return PartialView(task);
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

            return PartialView(tasksUsers);

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
            List<Missions> g = null;
            List<UsersRewards> u = null;
            List<Rewards> r = new List<Rewards>();
            Users manager = null;
            String managerUser = null;
            List<MissionExtended> task = new List<MissionExtended>();
            List<MissionExtended> mission = new List<MissionExtended>();
            List<UsersRewardsExtended> userRewards = new List<UsersRewardsExtended>();




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

            //get the task and the rewards of user manager
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
                viewModel.Tasks = task;
                viewModel.Reward = r;
                viewModel.ManagerName = managerUser;
            }


                //get the missions and the rewards of the employees
                using (RewardsForYouEntities db = new RewardsForYouEntities())
            {



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
            
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                userRewards = db.NoticeRewardsTakes.Include(n => n.Rewards)
                   .Include(n => n.Users).Where(l => l.ManagerID == UserID && l.Status == 2)
                   .Select(l => new UsersRewardsExtended()
                   {
                       RewardsID = l.Rewards.RewardsID,
                       Type = l.Rewards.Type,
                       Description = l.Rewards.Description,
                       Points= l.Rewards.Points,
                       Availability = l.Rewards.Availability,
                       UserName = l.Users.Name + " " + l.Users.Surname,
                       UserID = l.Users.UserID
                       
                   })
                   .ToList();

            }

            viewModel.User = x;
            viewModel.Mission = mission;
            viewModel.Rewardsed = userRewards;

            return View(viewModel);
        }

        public ActionResult _ChooseRewards()
        {
            int UserID = 0;
            if (Session["UserID"] != null)
            {
                UserID = (int)Session["UserID"];
            }
            //take all the rewards
            UserRewardModel userReward = new UserRewardModel();
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                userReward.rewards = db.Rewards.ToList();

            }
            userReward.UserID = UserID;
            return PartialView(userReward);
        }

        public ActionResult _PartialTakeReward(int RewardsID, int UserID)
        {
            Rewards reward = null;
            Users user = null;
            UsersRewards userReward = new UsersRewards();
            Rewards availabilityReward = new Rewards();
            Users userEmail = null;
            Users managerEmail = null;
            Rewards userRewards = null;

            NoticeRewardsTakes noticeRewards = new NoticeRewardsTakes();

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {


                Users userUpdated = db.Users.Find(UserID);
                availabilityReward = db.Rewards.Find(RewardsID);
                reward = db.Rewards.Where(l => l.RewardsID == RewardsID).FirstOrDefault();
                user = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();

                //check if the points of the user are enough for the selected reward
                if (user.UserPoints >= reward.Points)
                {

                    userEmail = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();
                    managerEmail = db.Users.Where(l => l.UserID == userEmail.ManagerUserID).FirstOrDefault();
                    userRewards = db.Rewards.Where(l => l.RewardsID == RewardsID).FirstOrDefault();
                    //userReward = db.UsersRewards.Where(l => l.UserID == UserID && l.RewardsID == RewardsID).FirstOrDefault();
                    noticeRewards.UserID = UserID;
                    noticeRewards.ManagerID = managerEmail.UserID;
                    noticeRewards.RewardsID = reward.RewardsID;
                    noticeRewards.Date = DateTime.Now;
                    noticeRewards.Status = 2;
                    db.NoticeRewardsTakes.Add(noticeRewards);
                    db.SaveChanges();


                    if (Settings.SmtpHost != null)
                    {
                        EmailSender.SendEmail(new EmailSender.Email
                        {
                            SenderAddress = userEmail.EMail,
                            Subject = "Richiesta fine missione",
                            Body = "Richiedo Il Rewards: " + reward.Description + ".\r\n" +
                            "Grazie " + userEmail.Name + userEmail.Surname + "."
                        });
                    }
                    return Json(new { messaggio = $"Richiesta inviata con successo", flag = true });
                }
                else
                {
                    return Json(new { messaggio = $"Richiesta invalida,i tuoi punti non sono sufficienti", flag = false });

                    //sottrazione dei punti allo user
                    //userUpdated.UserPoints = user.UserPoints - reward.Points;

                    ////diminuzione dell'availability del reward
                    //availabilityReward.Availability = availabilityReward.Availability - 1;

                    ////Inserisco il nuovo reward dell'utente nel db
                    //userReward.UserID = user.UserID;
                    //userReward.RewardsID = reward.RewardsID;
                    //userReward.Note = "";
                    //userReward.RewarrdsDate = DateTime.Now;
                    //db.UsersRewards.Add(userReward);
                    //db.SaveChanges();
                    //return Json(new { messaggio = $"{reward.Type} aggiunto/a con successo", flag = true });
                    //}

                    //else
                    //{
                    //    return Json(new { messaggio = $"I punti non sono sufficienti", flag = false });
                    //}
                }
            }

        }


        //Send email notify to the manager
        public ActionResult SendMissionNotify(int TaskID, int UserID)
        {
            Users userEmail = null;
            Users managerEmail = null;
            Tasks userTask = null;
            Missions mission = null;
            NoticeMissionEnded notice = new NoticeMissionEnded();
            NoticeMissionEnded controlNotice = null;

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                userEmail = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();
                managerEmail = db.Users.Where(l => l.UserID == userEmail.ManagerUserID).FirstOrDefault();
                userTask = db.Tasks.Where(l => l.TaskID == TaskID).FirstOrDefault();
                mission = db.Missions.Where(l => l.UserID == UserID && l.TaskID == TaskID).FirstOrDefault();
                controlNotice = db.NoticeMissionEnded.Where(l => l.MissionID == mission.MissionID && l.UserID == UserID).FirstOrDefault();
                if (controlNotice == null)
                {

                    notice.MissionID = mission.MissionID;
                    notice.UserID = UserID;
                    notice.Date = DateTime.Now;
                    notice.Status = 2;
                    notice.ManagerID = managerEmail.UserID;
                    db.NoticeMissionEnded.Add(notice);
                    db.SaveChanges();

                }
                else
                {
                    return Json(new { messaggio = $"Richiesta gia inviata!", flag = false });

                }
            }
            if (Settings.SmtpHost != null)
            {
                EmailSender.SendEmail(new EmailSender.Email
                {
                    SenderAddress = userEmail.EMail,
                    Subject = "Richiesta fine missione",
                    Body = "Richiedo l'accettazione della fine della missione: " + userTask.Description + ".\r\n" +
                    "Grazie " + userEmail.Name + userEmail.Surname + "."
                });
                return Json(new { messaggio = $"Richiesta inviata con successo", flag = true });
            }
            else
            {
                return Json(new { messaggio = $"Richiesta(senza Email) inviata con successo", flag = true });
            }
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

        public ActionResult AcceptRewards(int RewardsID, int UserID)
        {
           
            UsersRewards usersRewards = new UsersRewards();
            NoticeRewardsTakes noticeRewards = null;
            Rewards rewards = null;
            Users user = null;
            Rewards availabilityReward = new Rewards();
           

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                rewards = db.Rewards.Where(l => l.RewardsID == RewardsID).FirstOrDefault();
                user = db.Users.Where(l => l.UserID == UserID).FirstOrDefault();

                //usersRewards = db.UsersRewards.Where(l => l.RewardsID == RewardsID && l.UserID == UserID).FirstOrDefault();
                noticeRewards = db.NoticeRewardsTakes.Where(l => l.RewardsID == rewards.RewardsID && l.UserID == UserID).FirstOrDefault();
               
                Users userUpdated = db.Users.Find(UserID);
                availabilityReward = db.Rewards.Find(RewardsID);
           


                if ( noticeRewards != null)
                {
                    //user.UserPoints = user.UserPoints + task.Points;
                    //noticeRewards.Status = 0;
                    //rewards. = true;
                    //noticeMission.Status = 0;
                    //db.SaveChanges();

                    //sottrazione dei punti allo user
                    userUpdated.UserPoints = userUpdated.UserPoints - rewards.Points;

                    //diminuzione dell'availability del reward
                    availabilityReward.Availability = availabilityReward.Availability - 1;

                    //Inserisco il nuovo reward dell'utente nel db
                    usersRewards.UserID = user.UserID;
                    usersRewards.RewardsID = rewards.RewardsID;   
                    usersRewards.RewardsDate = DateTime.Now;
                    usersRewards.Note = "";
                    db.UsersRewards.Add(usersRewards);
                    noticeRewards.Status = 0;
                    db.SaveChanges();




                    return Json(new { message = $"Rewards Aggiunto con successo", flag = true });

                }
            }

            return Json(new { message = $"Rewards non accettati per qualche problema", flag = false });
        }

        public ActionResult RefuseRewards(int RewardsID, int UserID)
        {
            NoticeRewardsTakes noticeRewards = null;
            Rewards usersRewards = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                usersRewards = db.Rewards.Where(l => l.RewardsID == RewardsID).FirstOrDefault();
                noticeRewards = db.NoticeRewardsTakes.Where(l => l.UserID == UserID && l.RewardsID == usersRewards.RewardsID).FirstOrDefault();
            }
            return PartialView(noticeRewards);
        }

        public ActionResult DoRefuseRewards(int RewardsID, int UserID)
        {
            NoticeRewardsTakes noticeRewards = null;
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                noticeRewards = db.NoticeRewardsTakes.Where(l => l.RewardsID== RewardsID && l.UserID == UserID).FirstOrDefault();
                noticeRewards.Status = 1;
                db.SaveChanges();
            }

            return RedirectToAction("ManagerProfile", noticeRewards.ManagerID);
        }



        //UserImage
        [HttpPost]
        public async Task<JsonResult> GetAADUserImageAsync(string EMail)
        {
            JsonResult ret = null;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            try
            {
                string graphResourceID = "https://graph.windows.net/";
                UserProfileController userProfileController = new UserProfileController();

                Uri servicePointUri = new Uri(graphResourceID);
                Uri serviceRoot = new Uri(servicePointUri, tenantID);
                ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                      async () => await userProfileController.GetTokenForApplication());

                // use the token for querying the graph to get the user details

                var result1 = await activeDirectoryClient.Users.ExecuteAsync(); ;

                var result = await activeDirectoryClient.Users
                    .Where(u => u.UserPrincipalName.Equals(EMail))
                    .ExecuteAsync();

                //var result = await activeDirectoryClient.Users
                //    .Where(u => u.ObjectId.Equals(userObjectID))
                //    .ExecuteAsync();
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






        //[HttpPost]
        //public async Task<JsonResult> GetAADUserImageAsync()
        //{
        //    JsonResult ret = null;
        //    string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
        //    string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
        //    try
        //    {
        //        string graphResourceID = "https://graph.windows.net/";
        //        UserProfileController userProfileController = new UserProfileController();

        //        Uri servicePointUri = new Uri(graphResourceID);
        //        Uri serviceRoot = new Uri(servicePointUri, tenantID);
        //        ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
        //              async () => await userProfileController.GetTokenForApplication());

        //        // use the token for querying the graph to get the user details

        //        var result = await activeDirectoryClient.Users
        //            .Where(u => u.ObjectId.Equals(userObjectID))
        //            .ExecuteAsync();
        //        IUser user = result.CurrentPage.ToList().First();

        //        DataServiceStreamResponse photo = await user.ThumbnailPhoto.DownloadAsync();
        //        using (MemoryStream s = new MemoryStream())
        //        {
        //            photo.Stream.CopyTo(s);
        //            var encodedImage = Convert.ToBase64String(s.ToArray());
        //            ret = Json(new
        //            {
        //                Success = true,
        //                Base64StringImage = String.Format("data:image/gif;base64,{0}", encodedImage)
        //            });
        //        }
        //    }
        //    catch (AdalException e)
        //    {
        //        ret = Json(new
        //        {
        //            Success = false,
        //            Message = e.Message
        //        });
        //    }
        //    // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
        //    catch (Exception e)
        //    {
        //        ret = Json(new
        //        {
        //            Success = false,
        //            Message = e.Message
        //        });
        //    }
        //    return ret;
        //}


    }
}



