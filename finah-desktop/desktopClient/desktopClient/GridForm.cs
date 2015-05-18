using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sprint_1_def
{
   

    class GridForm
    {
        public int Id { get; set; }
        public string Info { get; set; }
        public string Categorie { get; set; }
        public string Relatie { get; set; }
        public int C_Leeftijd  { get; set; }
        public int M_Leeftijd { get; set; }
        public bool Rapport { get; set; }
        public bool C_Ingevuld { get; set; }
        public bool M_Ingevuld { get; set; }

        public GridForm(int id, string info, string category, string relation, bool done)
        {
            Id = id;
            Info = info;
            Categorie = category;
            Relatie = relation;
            Rapport = done;
           
            


        }
    }
}
