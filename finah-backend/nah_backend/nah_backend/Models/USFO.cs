using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public class USFO
    {
        public User US { get; set; }
        public Form FO { get; set; }

        public USFO(User us = null, Form fo = null)
        {
            if (us == null)
                us = new User();
            if (fo == null)
                fo = new Form();

            US = us;
            FO = fo;
        }
    }
}