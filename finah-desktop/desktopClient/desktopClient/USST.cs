using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sprint_1_def
{
    public class USST
    {
        public User US { get; set; }
        public int ST { get; set; }

        public USST(User us = null, int st = 0)
        {
            if (us == null)
                us = new User();

            US = us;
            ST = st;
        }
    }
}