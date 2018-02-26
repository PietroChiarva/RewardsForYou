using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class ViewModel
    {
        public Users User { get; set; }
        public IEnumerable<Tasks>Mission { get; set; }
        public Rewards Reward { get; set; }
        public List<Tasks> Tasks { get; internal set; }
    }
}