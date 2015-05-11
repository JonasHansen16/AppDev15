using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sprint_1_def
{
    public class ADMST
    {
        public User ADM { get; set; }
        public int ST { get; set; }

        public ADMST(User adm = null, int st = 0)
        {
            if (adm == null)
                adm = new User();

            ADM = adm;
            ST = st;
        }
    }
}