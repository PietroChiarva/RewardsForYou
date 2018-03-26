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

namespace RewardsForYou.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index(int? UserID)
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
            //MissionModel missionModel = new MissionModel();
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
                task = db.Missions.Include(m => m.Tasks).Where(l => l.UserID == UserID && l.Status == 0).Select(l => new MissionExtended()
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
                    return Json(new {  messaggio = $"Richiesta invalida,i tuoi punti non sono sufficienti", flag = true });

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





   
