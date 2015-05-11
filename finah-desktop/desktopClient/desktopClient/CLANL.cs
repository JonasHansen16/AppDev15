using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sprint_1_def
{
    public class CLANL
    {
        public Client CL { get; set; }
        public List<Answer> ANL { get; set; }

        public CLANL(Client cl = null, List<Answer> anl = null)
        {
            if (cl == null)
                cl = new Client();
            if (anl == null)
                anl = new List<Answer>();

            CL = cl;
            ANL = anl;
        }
    }
}