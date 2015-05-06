using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public class Answer
    {
        public int QuestionID { get; set; }
        public int Score { get; set; }
        public bool Help { get; set; }

        public Answer(int questionID = 0, int score = 0, bool help = false)
        {
            QuestionID = questionID;
            Score = score;
            Help = help;
        }
    }
}