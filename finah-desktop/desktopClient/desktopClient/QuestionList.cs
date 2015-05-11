using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Http;
using System.Windows;


namespace sprint_1_def
{
    class QuestionList
    {
        
        public QuestionList()
        {
          Questions = new List<Question>();
        }

        

        public List<Question> Questions { get; set; }
    }
}
