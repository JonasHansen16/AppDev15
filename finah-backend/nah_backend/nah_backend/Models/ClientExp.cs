using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public class ClientExp
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public int Age { get; set; }
        public bool Start { get; set; }
        public bool Done { get; set; }
        public string Function { get; set; }
        
        public ClientExp(int id = 0, string hash = "", int age = 0, bool start = false, bool done = false, string function = "")
        {
            Id = id;
            Hash = hash;
            Age = age;
            Start = start;
            Done = done;
            Function = function;
        }
    }
}