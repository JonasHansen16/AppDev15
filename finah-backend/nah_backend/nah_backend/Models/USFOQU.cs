using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public class USFOQU
    {
        public User US { get; set; }
        public Form FO { get; set; }
        public Questionnaire QU { get; set; }

        public USFOQU(User us = null, Form fo = null, Questionnaire qu = null)
        {
            if (us == null)
                us = new User();
            if (fo == null)
                fo = new Form();
            if (qu == null)
                qu = new Questionnaire();

            US = us;
            FO = fo;
            QU = qu;
        }
    }
}