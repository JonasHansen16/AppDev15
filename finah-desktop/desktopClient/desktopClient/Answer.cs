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
        public Answer()
        {

        }

        public Answer(int clientId)
        {
            ClientId = clientId;
        }

        public void writeAnswerToTextFile()
        {
            StreamWriter userWriter = new StreamWriter("../../users/"+ ClientId, true);

            
            userWriter.Write(ClientId);
            userWriter.Write(QuestionId);
            userWriter.Write(AnswerButton);
            userWriter.Write(Help);
            
            userWriter.Close();
        }



        public int ClientId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerButton { get; set; }
        public int Help { get; set; }

        
        
    }
}
