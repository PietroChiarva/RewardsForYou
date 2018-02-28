using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class UserRewardModel : RewardsForYouEntities
    {
        

        public List<Rewards> rewards { get; set; }
        public int UserID { get; set; }
    }
}