using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public class CLAN
    {
        public Client CL { get; set; }
        public Answer AN { get; set; }

        public CLAN(Client cl = null, Answer an = null)
        {
            if (cl == null)
                cl = new Client();
            if (an == null)
                an = new Answer();

            CL = cl;
            AN = an;
        }
    }
}