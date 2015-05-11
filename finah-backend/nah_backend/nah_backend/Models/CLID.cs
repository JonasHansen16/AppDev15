using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public class CLID
    {
        public Client CL { get; set; }
        public int ID { get; set; }

        public CLID(Client cl = null, int id = 0)
        {
            if (cl == null)
                cl = new Client();

            CL = cl;
            ID = id;
        }
    }
}