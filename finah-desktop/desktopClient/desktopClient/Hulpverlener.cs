using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sprint_1_def
{
    class Hulpverlener
    {
        private string naam;

        public string Naam
        {
            get { return naam; }
            set { naam = value; }
        }
        private string voornaam;

        public string Voornaam
        {
            get { return voornaam; }
            set { voornaam = value; }
        }
        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        private string beroep;

        public string Beroep
        {
            get { return beroep; }
            set { beroep = value; }
        }
        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public Hulpverlener(string name)
        {
            this.Naam = name;
        }
    }
}
