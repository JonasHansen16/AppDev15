using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
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