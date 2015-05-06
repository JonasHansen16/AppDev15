using nah_back.Models;
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
    public class AnswerController : ApiController
    {
        private string _qReset = "UPDATE answer SET final = FALSE WHERE clientid = @Id ;";
        private string _qExists = "SELECT id FROM client WHERE id = @Id AND hash = @Hash ;";
        private string _qInval = "UPDATE answer SET final = FALSE WHERE clientid = @Id AND qid = @Qid ;";
        private string _qInsert = "INSERT INTO answer (clientid, qid, score, help, final) VALUES (@Id, @Qid, @Score, @Help, 1) ;";
        private string _qCount =
            "SELECT COUNT(answer.id) AS amount " +
            "FROM client, answer " +
            "WHERE client.id = @Id " +
            "AND client.hash = @Hash " +
            "AND client.id = answer.clientid " +
            "AND answer.final = 1 " +
            "GROUP BY answer.id " +
            "; ";

        // POST api/answer/Reset
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// invalidate all of a client's answers.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the database was updated successfully, false otherwise. False if the client does not exist.</returns>
        [AllowAnonymous]
        [Route("api/answer/Reset")]
        public bool Reset(Client client)
        {
            return answerDBReset(client);
        }

        /// <summary>
        /// This function will
        /// invalidate all of a client's answers.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the database was successfully updated, false otherwise. False if the client does not exist.</returns>
        private bool answerDBReset(Client client)
        {
            // If the user does not exist, return false.
            if (!clientDBExists(client))
                return false;

            // Open connection
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            // Prepare command and set parameters
            SqlCommand insertCommand = new SqlCommand(_qReset, connection);
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

        // POST api/answer/Insert
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// insert an answer into the database.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <param name="answer">The Answer containing the question id, the score and whether or not help was requested.</param>
        /// <returns>True if the database was updated successfully, false otherwise. 
        /// False if the client does not exist or if the answer does not fit the required format.</returns>
        [AllowAnonymous]
        [Route("api/answer/Insert")]
        public bool Insert(Client client, Answer answer)
        {
            // If the client does not exist, we return false
            if (!clientDBExists(client))
                return false;
            // If the answer does not fit the database, we return false
            if (!answerDBCheck(answer))
                return false;
            // If something fails while invalidating the previous answers for the same question, we return false
            if (!answerDBInvalidate(client, answer))
                return false;
            // If everything else succeeds, return whether or not we were able to insert the answer
            return answerDBInsert(client, answer);
        }

        // POST api/answer/Check
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// check if an answer is valid for the database.
        /// </summary>
        /// <param name="answer">The Answer containing the question id, the score and whether or not help was requested.</param>
        /// <returns>True if the answer fits the required format. 
        /// False if the answer does not fit the required format.</returns>
        [AllowAnonymous]
        [Route("api/answer/Check")]
        public bool Check(Answer answer)
        {
            return answerDBCheck(answer);
        }

        /// <summary>
        /// Checks the passed Answer object to see if it conforms to the database 
        /// standards of minimum and maximum values as defined in DatabaseData.
        /// </summary>
        /// <param name="answer">The Answer object to check.</param>
        /// <returns>True if this object is valid according to database rules, false otherwise.</returns>
        private bool answerDBCheck(Answer answer)
        {
            // If our score is too low, we reject it.
            if (answer.Score < DatabaseData.Answer.Score.minval)
                return false;
            // Conversely, if the score is too high, we reject it.
            if (answer.Score > DatabaseData.Answer.Score.maxval)
                return false;
            // If the user requests help without meeting the minimum score to do so
            // We also reject the object
            if (answer.Help && answer.Score < DatabaseData.Answer.Help.minscore)
                return false;

            // If all previous checks failed, the object is valid.
            return true;
        }

        /// <summary>
        /// This function will
        /// invalidate all answers made by the client with the same ID as the answer parameter.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <param name="answer">The Answer containing the question id, the score and whether or not help was requested.</param>
        /// <returns>True if the database was successfully updated, false otherwise.</returns>
        private bool answerDBInvalidate(Client client, Answer answer)
        {
            // Open connection
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            // Prepare command and set parameters
            SqlCommand insertCommand = new SqlCommand(_qInval, connection);
            insertCommand.Parameters.AddWithValue("@Id", client.Id);
            insertCommand.Parameters.AddWithValue("@Qid", answer.QuestionID);
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
        /// This function will
        /// insert an answer into the database.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the database was successfully updated, false otherwise.</returns>
        private bool answerDBInsert(Client client, Answer answer)
        {
            // Open connection
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            // Prepare command and set parameters
            SqlCommand insertCommand = new SqlCommand(_qInsert, connection);
            insertCommand.Parameters.AddWithValue("@Id", client.Id);
            insertCommand.Parameters.AddWithValue("@Qid", answer.QuestionID);
            insertCommand.Parameters.AddWithValue("@Score", answer.Score);
            insertCommand.Parameters.AddWithValue("@Help", answer.Help);
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

        // POST api/answer/Count
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// count the amount of valid answers a client has given so far.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>The amount of answers the client has given so far or -1 if the client does not exist.</returns>
        [AllowAnonymous]
        [Route("api/answer/Count")]
        public int Count(Client client)
        {
            // If our client doesn't exist, return -1
            if (!clientDBExists(client))
                return -1;

            // Else count the answers and return the count
            return answerDBCount(client);
        }

        /// <summary>
        /// This function will connect to the database, 
        /// execute a query asking the database for the amount of valid answers a client has given, 
        /// and will return that amount.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>The amount of answers the client has given so far or -1 if the client does not exist.</returns>
        private int answerDBCount(Client client)
        {
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qCount, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", client.Id);
                selectCommand.Parameters.AddWithValue("@Hash", client.Hash);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                { // Return the result.
                    return reader.GetInt32(0);
                }
            }

            // If we did not find a result, we return -1
            return -1;
        }

        // POST api/answer/InsertList
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// insert a list of answers into the database.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <param name="answer">The Answer containing the question id, the score and whether or not help was requested.</param>
        /// <returns>True if the database was updated successfully, false otherwise. 
        /// False if the client does not exist or if one of the answers does not fit the required format.</returns>
        [AllowAnonymous]
        [Route("api/answer/InsertList")]
        public bool InsertList(Client client, List<Answer> answers)
        {
            // If the client does not exist, we return false
            if (!clientDBExists(client))
                return false;
            // If any of the answers does not fit the database, we return false
            foreach(Answer answer in answers)
                if (!answerDBCheck(answer))
                    return false;
            // If something fails while invalidating the previous answers for the same questions, we return false
            foreach (Answer answer in answers)
                if (!answerDBInvalidate(client, answer))
                    return false;
            // If everything else succeeds, insert all the answers. If something goes wrong, return false
            foreach (Answer answer in answers)
                if (!answerDBInsert(client, answer))
                    return false;
            // If everything succeeded, return true
            return true;
        }
    }
}
