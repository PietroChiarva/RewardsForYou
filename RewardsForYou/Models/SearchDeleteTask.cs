using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class SearchDeleteTask: Tasks
    {
        public SearchDeleteTask()
        {
            Lista = new List<Tasks>();
        }

        public List<Tasks> Lista { get; set; }
    }
}