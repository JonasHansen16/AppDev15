using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sprint_1_def
{
    public class ADMUS
    {
        public User ADM { get; set; }
        public User US { get; set; }

        public ADMUS(User adm = null, User us = null)
        {
            if (adm == null)
                adm = new User();
            if (us == null)
                us = new User();

            ADM = adm;
            US = us;
        }
    }
}