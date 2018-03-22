using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace RewardsForYou.Models
{
    public class UsersRewardsExtended
    {
        public int RewardsID { get; set; }
        public String Type { get; set; }
        public String Description { get; set; }
        public int Points { get; set; }
        public decimal? Availability { get; set; }
        public String UserName { get; set; }
        public int UserID { get; set; }
        public int NoticeRewardsTakeID { get; set; }

    }
}