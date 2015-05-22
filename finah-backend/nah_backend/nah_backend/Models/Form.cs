using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public class Form
    {
        public int Id { get; set; }
        public string Memo { get; set; }
        public string Category { get; set; }
        public string Relation { get; set; }
        public bool Completed { get; set; }
        public bool CheckedReport { get; set; }
        public List<ClientExp> ClientList { get; set; }
        public int Repeats { get; set; }

        public Form(int id = 0, string memo = "", string category = "", string relation = "", bool completed = false, bool checkedreport = false, List<ClientExp> clientlist = null, int repeats = 0)
        {
            if (clientlist == null)
                clientlist = new List<ClientExp>();

            Id = id;
            Memo = memo;
            Category = category;
            Relation = relation;
            Completed = completed;
            CheckedReport = checkedreport;
            ClientList = clientlist;
            Repeats = repeats;
        }
    }
}