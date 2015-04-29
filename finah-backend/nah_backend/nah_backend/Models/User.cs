using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_back.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Occupation { get; set; }
        public bool Admin { get; set; }
        public bool Active { get; set; }
        public bool Denied { get; set; }

        public User()
        {

        }

        public User(string name, string lastname, string email, string password, string occupation, bool admin = false, bool active = false, bool denied = false, int id = 0)
        {
            Name = name;
            LastName = lastname;
            Email = email;
            Password = password;
            Occupation = occupation;
            Admin = admin;
            Active = active;
            Denied = denied;
            Id = id;
        }
    }
}