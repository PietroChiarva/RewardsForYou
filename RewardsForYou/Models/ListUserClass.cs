using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class ListUserClass : RewardsForYouEntities
    {
        public ListUserClass()
            {
            ResultList = new List<RewardsForYouEntities>();
            }
        public List<RewardsForYouEntities> ResultList { get; set; }
        public new int SerialNumber { get; set; }
    }
}