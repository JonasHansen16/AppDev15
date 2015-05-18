using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sprint_1_def
{
    public class Report
    {
        public List<List<Answer>> AnswerList { get; set; }

        public List<Question> QuestionList { get; set; }

        public List<ClientExp> ClientList { get; set; }

        public Report(List<List<Answer>> answerlist = null, List<Question> questionlist = null, List<ClientExp> clientlist = null)
        {
            if (answerlist == null)
                answerlist = new List<List<Answer>>();
            if (questionlist == null)
                questionlist = new List<Question>();
            if (clientlist == null)
                clientlist = new List<ClientExp>();

            AnswerList = answerlist;
            QuestionList = questionlist;
            ClientList = clientlist;
        }
    }
}