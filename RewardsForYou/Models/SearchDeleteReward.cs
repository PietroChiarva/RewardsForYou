using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class SearchDeleteReward: Rewards
    {

        public SearchDeleteReward()
        {
            Lista = new List<Rewards>();
        }

        public List<Rewards> Lista { get; set; }
    }
}