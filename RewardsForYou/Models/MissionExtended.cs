using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class MissionExtended
    {
        public int TaskID { get; set; }
        public String Type { get; set; }
        public String Description {get; set;}
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? EndDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DesiredEndDate { get; set; }
        public int Points { get; set; }
        public Boolean IsFinished { get; set; }
        public String Note { get; set; }
       
    }
}