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

        public ActionResult AddTask()
        {
            using (RewardsForYouEntities db = new RewardsForYouEntities())
            {

                return View();
            }
        }

        public ActionResult DoAddTask(int TaskID, string Type, string Description, DateTime ExpiryDate, int Points, bool Finished)
        {

            if (TaskID != 0 && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Description)  && Points != 0 )
            {
                using (RewardsForYouEntities db = new RewardsForYouEntities())
                {
                    
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

