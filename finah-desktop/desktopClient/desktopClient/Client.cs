using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sprint_1_def
{
    public class Client
    {
        public int Id { get; set; }
        public string Hash { get; set; }

        public Client(int id = 0, string hash = "")
        {
            Id = id;
            Hash = hash;
        }
    }
}