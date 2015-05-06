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
    public class QuestionnaireController : ApiController
    {
        private string _qIntro = 
            "SELECT questionnaire.intro, questionnaire.title " +
            "FROM client, form, questionnaire " +
            "WHERE client.id = @Id  " +
            "AND client.hash = @Hash  " +
            "AND client.formid = form.id " +
            "AND form.airid = questionnaire.id " +
            ";";
        private string _qCount = 
            "SELECT COUNT(questionlist.qid) AS amount " +
            "FROM client, form, questionnaire, questionlist " +
            "WHERE client.id = @Id " +
            "AND client.hash = @Hash " +
            "AND client.formid = form.id " + 
            "AND form.airid = questionnaire.id " +
            "AND questionnaire.id = questionlist.airid " +
            "AND questionlist.active = 1 " +
            "; ";
        private string _qExists = "SELECT id FROM client WHERE id = @Id AND hash = @Hash ;";

        // POST api/questionnaire/Intro
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// get the intro of a questionnaire.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>The intro of the client's questionnaire or null if the client does not exist.</returns>
        [AllowAnonymous]
        [Route("api/questionnaire/Intro")]
        public Intro Intro(Client client)
        {
            return questionnaireDBIntro(client);
        }

        /// <summary>
        /// This function will connect to the database, 
        /// execute a query asking the database for the client's questionnaire's intro, 
        /// and will return that intro if it exists.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>The intro of the client's questionnaire or null if the client does not exist.</returns>
        private Intro questionnaireDBIntro(Client client)
        {
            if (client.Hash == null || client.Hash == "")
                return null;

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qIntro, connection);
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
                    Intro output = new Intro();
                    output.Text = reader.GetString(0);
                    output.Title = reader.GetString(1);
                    return output;
                }
            }

            // If the client does not exist,
            // we return null
            return null;
        }

        // POST api/questionnaire/Count
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// count the total amount of questions in a questionnaire.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>The total amount of questions of the client's questionnaire or -1 if the client does not exist.</returns>
        [AllowAnonymous]
        [Route("api/questionnaire/Count")]
        public int Count(Client client)
        {
            // If our client doesn't exist, return -1
            if (!clientDBExists(client))
                return -1;

            // Else count the questions and return the count
            return questionnaireDBCount(client);
        }

        /// <summary>
        /// This function will connect to the database, 
        /// execute a query asking the database for the total amount of questions in a client's questionnaire, 
        /// and will return that amount.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>The total amount of questions of the client's questionnaire. -1 if no such user exists.</returns>
        private int questionnaireDBCount(Client client)
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
    }
}
