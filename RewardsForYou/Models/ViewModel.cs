using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class ViewModel : RewardsForYouEntities
    {
    
        public Users User { get; set; }
        public List<MissionExtended> Mission { get; set; } 
        public List<UsersRewardsExtended> Rewardsed { get; set; }
        public List<Tasks> Task { get; set; }
        public List<Rewards> Reward { get; set; }
        public String ManagerName { get; set; }
        public List<Users> UserName { get; set; }
        public List<Missions> MissionDesiredDate { get; set; }
        public List<object> MissionArray { get; set; }
        public List<NoticeMissionEnded> noticeMissions { get; set; }

      


    }
}