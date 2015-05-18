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
    public class ClientController : ApiController
    {
        private string _qExists = "SELECT id FROM client WHERE id = @Id AND hash = @Hash ;";
        private string _qStart = "SELECT start FROM client WHERE id = @Id AND hash = @Hash ;";
        private string _qDone = "SELECT done FROM client WHERE id = @Id AND hash = @Hash ;";
        private string _qSetStart = "UPDATE client SET start = 1 WHERE id = @Id AND hash = @Hash ;";
        private string _qUnSetStart = "UPDATE client SET start = 0 WHERE id = @Id AND hash = @Hash ;";
        private string _qSetDone = "UPDATE client SET done = 1 WHERE id = @Id AND hash = @Hash ;";
        private string _qUnSetDone = "UPDATE client SET done = 0 WHERE id = @Id AND hash = @Hash ;";
        private string _qGetForm = "SELECT formid FROM client WHERE id = @Id AND hash = @Hash ";
        private string _qAllDone = "SELECT client.id FROM client WHERE client.formid = @Id AND client.done = 0 ;";
        private string _qSetFormDone = "UPDATE form SET completed = 1 WHERE id = @Id ;";
        
        // POST api/client/Exists
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// check if a certain client exists or not.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the Client id-hash combination exists, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/client/Exists")]
        public bool Exists(Client client)
        {
            return clientDBExists(client);
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
            if(client.Hash == null || client.Hash == "") 
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

        // POST api/client/Start
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// check if a certain client has already started with his 
        /// questionnaire or not.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the Client has not yet started with his questionnaire, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/client/Start")]
        public bool Start(Client client)
        {
            return clientDBStart(client);
        }

        /// <summary>
        /// This function will connect to the database, 
        /// execute a query asking the database if the client has already started his questionnaire, 
        /// and will return true if he has.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the Client id-hash combination exists and the client has not yet started his questionnaire, false otherwise.</returns>
        private bool clientDBStart(Client client)
        {
            if (client.Hash == null || client.Hash == "")
                return false;

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qStart, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", client.Id);
                selectCommand.Parameters.AddWithValue("@Hash", client.Hash);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return reader.GetBoolean(0); // We found the client and return whether it has started or not
            }

            // If the client does not exist, we return false
            return false;
        }

        // POST api/client/Started
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// check if a certain client has already finished with his 
        /// questionnaire or not.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the Client has already finished with his questionnaire, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/client/Done")]
        public bool Done(Client client)
        {
            return clientDBDone(client);
        }

        /// <summary>
        /// This function will connect to the database, 
        /// execute a query asking the database if the client has already finished his questionnaire, 
        /// and will return true if he has.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the Client id-hash combination exists and the client has finished his questionnaire, false otherwise.</returns>
        private bool clientDBDone(Client client)
        {
            if (client.Hash == null || client.Hash == "")
                return false;

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qDone, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", client.Id);
                selectCommand.Parameters.AddWithValue("@Hash", client.Hash);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return reader.GetBoolean(0); // We found the client and return whether it has finished or not
            }

            // If the client does not exist, we return false
            return false;
        }

        // POST api/client/SetStart
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// set a client's start bit.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the client was successfully updated, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/client/SetStart")]
        public bool SetStart(Client client)
        {
            return clientDBSetStart(client);
        }

        /// <summary>
        /// This function will allow the user of the API to
        /// set a client's start bit.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the client was successfully updated, false otherwise.</returns>
        private bool clientDBSetStart(Client client)
        {
            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = new SqlCommand(_qSetStart, connection);
            insertCommand.Parameters.AddWithValue("@Id", client.Id);
            insertCommand.Parameters.AddWithValue("@Hash", client.Hash);
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

        // POST api/client/UnSetStart
        /// <summary>
        /// This function will allow the user of the API to
        /// unset a client's start bit.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the client was successfully updated, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/client/UnSetStart")]
        public bool UnSetStart(Client client)
        {
            return clientDBUnSetStart(client);
        }

        /// <summary>
        /// This POST api function will allow the user of the API to
        /// unset a client's start bit.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the client was successfully updated, false otherwise.</returns>
        private bool clientDBUnSetStart(Client client)
        {
            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = new SqlCommand(_qUnSetStart, connection);
            insertCommand.Parameters.AddWithValue("@Id", client.Id);
            insertCommand.Parameters.AddWithValue("@Hash", client.Hash);
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

        // POST api/client/SetDone
        /// <summary>
        /// This api function will allow the user of the API to
        /// set a client's done bit. It will also set the form's
        /// done bit if all the clients are done.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the client was successfully updated, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/client/SetDone")]
        public bool SetDone(Client client)
        {
            // Set client's done bit
            if(!clientDBSetDone(client))
                return false;

            // Get client's form id
            int id = DBGetFormID(client);
            // Return false if the client has no form
            if (id == -1)
                return false;

            // Check if all clients are done with that form, if so: set its done bit
            if (allClientsDone(id))
                DBFormDone(id);

            return true;
        }

        /// <summary>
        /// Sets a form's completed bit to true.
        /// </summary>
        /// <param name="formid">The form to set to completed.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        private bool DBFormDone(int formid)
        {
            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = new SqlCommand(_qSetFormDone, connection);
            insertCommand.Parameters.AddWithValue("@Id", formid);
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
        /// Gets a client's form id.
        /// </summary>
        /// <param name="client">The client, identified by his id and his hash.</param>
        /// <returns>The client's form id; or -1 if the client does not exist.</returns>
        private int DBGetFormID(Client client)
        {
            if (client.Hash == null || client.Hash == "")
                return -1;

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qGetForm, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", client.Id);
                selectCommand.Parameters.AddWithValue("@Hash", client.Hash);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return reader.GetInt32(0); // We found the client and return its form id
            }

            // If the client does not exist, we return -1
            return -1;
        }

        /// <summary>
        /// Checks if all clients are done with the passed form.
        /// </summary>
        /// <param name="formid">The id of the form to check.</param>
        /// <returns>True if all clients are done, false otherwise.</returns>
        private bool allClientsDone(int formid)
        {
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qAllDone, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", formid);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return false; // We have a client that is not yet done
            }

            // If we have no such clients, we return true.
            return true;
        }

        /// <summary>
        /// This function will allow the user of the API to
        /// set a client's done bit.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the client was successfully updated, false otherwise.</returns>
        private bool clientDBSetDone(Client client)
        {
            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = new SqlCommand(_qSetDone, connection);
            insertCommand.Parameters.AddWithValue("@Id", client.Id);
            insertCommand.Parameters.AddWithValue("@Hash", client.Hash);
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

        // POST api/client/UnSetDone
        /// <summary>
        /// This api function will allow the user of the API to
        /// unset a client's done bit.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the client was successfully updated, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/client/UnSetDone")]
        public bool UnSetDone(Client client)
        {
            return clientDBUnSetDone(client);
        }

        /// <summary>
        /// This function will allow the user of the API to
        /// unset a client's done bit.
        /// </summary>
        /// <param name="client">The Client containing an id-hash combination.</param>
        /// <returns>True if the client was successfully updated, false otherwise.</returns>
        private bool clientDBUnSetDone(Client client)
        {
            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = new SqlCommand(_qUnSetDone, connection);
            insertCommand.Parameters.AddWithValue("@Id", client.Id);
            insertCommand.Parameters.AddWithValue("@Hash", client.Hash);
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
    }
}
