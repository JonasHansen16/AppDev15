using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sprint_1_def
{
    class Answer
    {
        public Answer(int qId, int answerButton, bool help)
        {
            QuestionId = qId;
            Score = answerButton;
            Help = help;
        }

        public Answer()
        {

        }

     

        
        public int QuestionId { get; set; }
        public int Score { get; set; }
        public bool Help { get; set; }

        
        
    }
}
