﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RewardsForYou.Models
{
    public class SearchRewardsforyou : Users
    {
        public List<Users> ResultList { get; set; }

        public new int? UsersID { get; set; }

    }
}

