using nah_back.Providers;
using nah_backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace nah_backend.Controllers
{
    public class QuestionController : ApiController
    {
        private string _qNext = 
            "SELECT question.id, question.txt, question.title " + 
            "FROM client, form, questionnaire, questionlist, question, " +
            "( " +
            "    SELECT ISNULL(MAX(answer.qid), 0) AS id " +
            "    FROM client, answer " +
            "    WHERE client.id = answer.clientid " +
            "    AND answer.final = 1 " +
            ") AS lastanswer " +
            "WHERE client.id = @Id AND client.hash = @Hash " +
            "AND client.formid = form.id " +
            "AND form.airid = questionnaire.id " +
            "AND questionlist.airid = questionnaire.id " +
            "AND questionlist.active = 1 " +
            "AND questionlist.qid = question.id " +
            "AND question.id > lastanswer.id " +
            "ORDER BY question.id ASC " +
            ";";

        private string _qPrevious =
            "UPDATE answer " +
            "SET final = 0 " +
            "WHERE clientid = @Id " +
            "AND qid = " +
            "   ( " +
            "    SELECT mqid " +
            "    FROM " +
            "       ( " +
            "            SELECT MAX(qid) as mqid " +
            "            FROM answer " +
            "            WHERE clientid = @Id AND final = 1 " +
            "        ) as temp " +
            "   ) " +
            ";";

        private string _qAll =
            "SELECT question.id, question.txt, question.title " +
            "FROM client, form, questionnaire, questionlist, question " +
            "WHERE client.id = @Id AND client.hash = @Hash " +
            "AND client.formid = form.id " +
            "AND form.airid = questionnaire.id " +
            "AND questionnaire.id = questionlist.airid " +
            "AND questionlist.qid = question.id " +
            ";";

        private string _qImage =
            "SELECT question.img AS img " +
            "FROM client, form, questionnaire, questionlist, question " +
            "WHERE client.id = @Id " +
            "AND client.hash = @Hash  " +
            "AND client.formid = form.id " +
            "AND form.airid = questionnaire.id " +
            "AND questionnaire.id = questionlist.airid " +
            "AND questionlist.active = 1 " +
            "AND questionlist.qid = question.id " +
            "AND question.id = @Qid " +
            ";";

        private string _qExists = "SELECT id FROM client WHERE id = @Id AND hash = @Hash";

        // POST api/question/Next
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// get the next question a client has to fill in.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>The next unanswered question of the client, or null if the client does not exist or has no remaining unanswered questions.</returns>
        [AllowAnonymous]
        [Route("api/question/Next")]
        public Question Next(Client client)
        {
            return questionDBNext(client);
        }

        /// <summary>
        /// This function will connect to the database, 
        /// execute a query asking the database for the client's next unanswered question, 
        /// and will return that question if it exists.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>The next unanswered question of the client, or null if the client does not exist or has no remaining unanswered questions.</returns>
        private Question questionDBNext(Client client)
        {
            if (client.Hash == null || client.Hash == "")
                return null;

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qNext, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", client.Id);
                selectCommand.Parameters.AddWithValue("@Hash", client.Hash);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read()) 
                { // Get all relevant data and return the result.
                    Question output = new Question();
                    output.Id = reader.GetInt32(0);
                    output.Text = reader.GetString(1);
                    output.Title = reader.GetString(2);
                    return output;
                }
            }

            // If the client does not exist, or has no more questions left to answer,
            // we return null
            return null;
        }

        // POST api/question/All
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// get all question a client has to fill in, regardless on whether
        /// they have already been filled in or not.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>All questions of the client, or null if the client does not exist.</returns>
        [AllowAnonymous]
        [Route("api/question/All")]
        public List<Question> All(Client client)
        {
            return questionDBAll(client);
        }

        /// <summary>
        /// This function will connect to the database, 
        /// execute a query asking the database for all the client's questions, 
        /// and will return them
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>All questions of the client, or null if the client does not exist.</returns>
        private List<Question> questionDBAll(Client client)
        {
            if (client.Hash == null || client.Hash == "")
                return null;

            bool success = false;
            List<Question> output = new List<Question>();

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qAll, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", client.Id);
                selectCommand.Parameters.AddWithValue("@Hash", client.Hash);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader();
                // Put all results in a list
                while (reader.Read())
                { // Get all relevant data and put it in a Question object
                    Question current = new Question();
                    current.Id = reader.GetInt32(0);
                    current.Text = reader.GetString(1);
                    current.Title = reader.GetString(2);
                    output.Add(current);
                    success = true;
                }
            }

            // If we were unsuccessful, return null
            if (!success)
                return null;
            else
                return output;
        }

        // POST api/question/PreviousQuestion
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// return to the client's previous question by invalidating his
        /// last answer. To return to the previous question: call 
        /// api/question/NextQuestion after calling this API function.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the database was successfully updated, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/question/PreviousQuestion")]
        public bool PreviousQuestion(Client client)
        {
            return questionDBPreviousQuestion(client);
        }

        /// <summary>
        /// This function will allow the user of the API to
        /// set a client's start bit.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the client was successfully updated, false otherwise.</returns>
        private bool questionDBPreviousQuestion(Client client)
        {
            if(clientDBExists(client))
                return false;
            

            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = new SqlCommand(_qPrevious, connection);
            insertCommand.Parameters.AddWithValue("@Id", client.Id);
            // Send query
            try
            {
                connection.Open();
                insertCommand.ExecuteNonQuery();
            }
            // If an error occurs, we return false
            catch (SqlException)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }

            // Else we return true
            return true;
        }

        /// <summary>
        /// This function will connect to the database, 
        /// execute a query asking the database if the client specified by its id and hash exists, 
        /// and will return true if there is at least one such client.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the Client id-hash combination exists, false otherwise.</returns>
        private bool clientDBExists(Client client)
        {
            if (client.Hash == null || client.Hash == "")
                return false;

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qExists, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", client.Id);
                selectCommand.Parameters.AddWithValue("@Hash", client.Hash);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return true; // We found the client and return true
            }

            // If the client does not exist, we return false
            return false;
        }

        // POST api/question/Image
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// return the image of a question.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <param name="qid">The id of the question.</param>
        /// <returns>The image if the question exists and the client has access to it, null otherwise.</returns>
        [AllowAnonymous]
        [Route("api/question/Image")]
        public byte[] Image(Client client, int qid)
        {
            return questionDBImage(client, qid);
        }

        /// <summary>
        /// This function will fetch a question's image from the database.
        /// </summary>
        /// <param name="client">A client with access to the question.</param>
        /// <param name="qid">The id of the question.</param>
        /// <returns>The image if the question exists and the client has access to it, null otherwise.</returns>
        private byte[] questionDBImage(Client client, int qid)
        {
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qImage, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", client.Id);
                selectCommand.Parameters.AddWithValue("@Hash", client.Hash);
                selectCommand.Parameters.AddWithValue("@Qid", qid);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return (byte[])reader["img"]; // We found the client and return true
            }

            // If the client does not exist, we return false
            return null;
        }
    }
}
