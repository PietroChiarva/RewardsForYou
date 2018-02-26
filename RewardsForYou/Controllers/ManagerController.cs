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

        public ActionResult _PartialTaskDetails(int UserID, string Name, string Surname, string EMail, int ManagerUserID)
        {
            ViewModel viewModel = new ViewModel();
            Missions x = null;
            IQueryable<Tasks> t = null;

            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {
                //get tasks of the user
                x = db.Missions.Where(l => l.UserID == UserID).FirstOrDefault();
                t = db.Tasks.Where(l => l.TaskID == x.TaskID);

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

        public ActionResult DoAddTask(int TaskID, string Type, string Description, DateTime ExpiryDate, int Points, string Finished)
        {

            if (TaskID != 0 && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Description)  && Points != 0 && !string.IsNullOrEmpty(Finished))
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
}



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

