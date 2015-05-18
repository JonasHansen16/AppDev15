using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sprint_1_def
{
    class GridUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Occupation { get; set; }



        public GridUser(string name, string lastname, string email, string occupation,string username, int id = 0)
        {
            UserName = username;
            Name = name;
            LastName = lastname;
            Email = email;
            Occupation = occupation;
            Id = id;
        }
    }
}
