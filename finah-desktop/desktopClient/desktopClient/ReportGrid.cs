using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sprint_1_def
{
    class ReportGrid
    {
        public string Theme { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public bool Help1 { get; set; }
        public bool Help2 { get; set; }
        public int Id { get; set; }
        public bool Valid { get; set; }

        public ReportGrid(string theme, int score1, bool hulp1, int score2, bool hulp2)
        {
            Theme = theme;
            Score1 = score1;
            Score2 = score2;
            Help1 = hulp1;
            Help2 = hulp2;

        } 

    }
}
