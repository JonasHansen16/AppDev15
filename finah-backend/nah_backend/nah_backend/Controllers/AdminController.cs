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
    public class AdminController : ApiController
    {
        private string _qAdmin = "SELECT id FROM usr WHERE username = @UserName AND pass = @Password AND adm = 1;";

        private string _qAmountInactive = "SELECT COUNT(id) AS amount FROM usr WHERE active = 0 AND denied = 0;";

        private string _qSetActive = "UPDATE usr SET active = 1 WHERE id = @Id ;";

        private string _qSetDenied = "UPDATE usr SET denied = 1 WHERE id = @Id ;";

        private string _qInactive =
            "WITH largelatest AS " +
            "( " +
            "    SELECT TOP(@Start + @Max) id, name, lastname, email, username, occupation, adm, active, denied " +
            "    FROM usr " +
            "    WHERE active = 0 AND denied = 0 " +
            "    ORDER BY id DESC " +
            "), " +
            "latest AS " +
            "( " +
            "    SELECT TOP(@Max) id, name, lastname, email, username, occupation, adm, active, denied " +
            "    FROM largelatest " +
            "    ORDER BY id ASC " +
            ") " +
            "SELECT id, name, lastname, email, username, occupation, adm, active, denied " +
            "FROM latest " +
            "ORDER BY id DESC " +
            ";";

        // POST api/admin/Inactive
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// get a list of up to DatabaseData.UserList.maxlen inactive users, 
        /// ranked newest-first, starting from the passed number.
        /// Example: calling this function with number 0 will get
        /// the DatabaseData.UserList.maxlen latest inactive users. 
        /// Calling it with number DatabaseData.UserList.maxlen will 
        /// get the next DatabaseData.UserList.maxlen amount.
        /// </summary>
        /// <param name="admst">An ADMST object which contains a user with admin privileges and the amount of users to be skipped at the start of the list.</param>
        /// <returns>
        /// A list of inactive users. 
        /// Null if the user does not have admin privileges or the passed amount is less than zero. 
        /// The last full list of inactive users if there are no such users.
        /// </returns>
        [AllowAnonymous]
        [Route("api/admin/Inactive")]
        public List<User> Inactive(ADMST admst)
        {
            // First we make sure the passed number is positve
            if (admst.ST < 0)
                return null;

            // Then we make sure the passed user is in fact an admin
            if (!adminCheck(admst.ADM.UserName, admst.ADM.Password))
                return null;

            // If the user is an admin, then we fetch the list of inactive users and return it
            return inactiveUserDB(admst.ST, DatabaseData.UserList.maxlen);
        }

        //private List<User> getInactiveUsers

        /// <summary>
        /// Checks whether the user identified by his name and
        /// his password is an admin or not.
        /// </summary>
        /// <param name="name">The name of the admin.</param>
        /// <param name="pass">The hashed password of the admin.</param>
        /// <returns>True if the users exists and is an admin, false otherwise.</returns>
        private bool adminCheck(string name, string pass)
        {
            if (name == null || name == "" || pass == null || pass == "")
                return false;

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qAdmin, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@UserName", name);
                selectCommand.Parameters.AddWithValue("@Password", pass);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return true; // We found the admin and return true
            }

            // If the user does not exist, or is not an admin, we return false
            return false;
        }

        /// <summary>
        /// Searches the database for a number of inactive users, ranked chronologically.
        /// Returns the latest max users, with start offset.
        /// </summary>
        /// <param name="offset">The offset of users to skip.</param>
        /// <param name="max">The maximum amount of users to return.</param>
        /// <returns>A list of users, which will be empty if a negative argument was passed. 
        /// The last list of max users if there are no such users.</returns>
        private List<User> inactiveUserDB(int offset, int max)
        {
            List<User> output = new List<User>();

            if (offset < 0 || max < 0)
                return output;

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qInactive, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Max", max);
                selectCommand.Parameters.AddWithValue("@Start", offset);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader();
                // Read results
                while (reader.Read())
                {
                    // Turn it into a User and return it
                    //id, name, lastname, email, username, occupation, adm, active, denied
                    User toAdd = new User();
                    toAdd.Id = reader.GetInt32(0);
                    toAdd.Name = reader.GetString(1);
                    toAdd.LastName = reader.GetString(2);
                    toAdd.Email = reader.GetString(3);
                    toAdd.UserName = reader.GetString(4);
                    toAdd.Occupation = reader.GetString(5);
                    toAdd.Admin = reader.GetBoolean(6);
                    toAdd.Active = reader.GetBoolean(7);
                    toAdd.Denied = reader.GetBoolean(8);
                    toAdd.Password = "";
                    output.Add(toAdd);
                }
            }

            // Return our list
            return output;
        }

        // POST api/admin/AmountInactive
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// get the current amount of inactive users.
        /// </summary>
        /// <param name="user">A user with admin privileges.</param>
        /// <returns>
        /// The amount of inactive users. -1 if the passed User is not an admin.
        /// </returns>
        [AllowAnonymous]
        [Route("api/admin/AmountInactive")]
        public int AmountInactive(User user)
        {
            // First we make sure the passed user is in fact an admin
            if (!adminCheck(user.UserName, user.Password))
                return -1;

            // If the user is an admin, then we fetch the amount of inactive users and return it
            return inactiveAmountDB();
        }

        /// <summary>
        /// This function will return the amount of inactive users in the database.
        /// </summary>
        /// <returns>The amount of inactive users in the database.</returns>
        private int inactiveAmountDB()
        {
            int output = 0;

            // Get connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qAmountInactive, connection);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // Read results
                if (reader.Read())
                {
                    output = reader.GetInt32(0);
                }
            }

            // Return our output
            return output;
        }

        // POST api/admin/Activate
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// activate a user.
        /// </summary>
        /// <param name="admus">An ADMUS object which contains an admin user and a second user.</param>
        /// <returns>
        /// True if the database was successfully updated, false otherwise. False if the first passed user is not an admin.
        /// </returns>
        [AllowAnonymous]
        [Route("api/admin/Activate")]
        public bool Activate(ADMUS admus)
        {
            if (admus.ADM == null || admus.US == null || !adminCheck(admus.ADM.Name, admus.ADM.Password))
                return false;

            return activateDB(admus.US.Id);
        }

        private bool activateDB(int id)
        {
            // Open connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Prepare command and set parameters
                SqlCommand insertCommand = new SqlCommand(_qSetActive, connection);
                insertCommand.Parameters.AddWithValue("@Id", id);
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
            }

            // Else we return true
            return true;
        }

        // POST api/admin/Deny
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// deny a user.
        /// </summary>
        /// <param name="admus">An ADMUS object which contains an admin user and a second user.</param>
        /// <returns>
        /// True if the database was successfully updated, false otherwise. False if the first passed user is not an admin.
        /// </returns>
        [AllowAnonymous]
        [Route("api/admin/Deny")]
        public bool Deny(ADMUS admus)
        {
            if (admus.ADM == null || admus.US == null || !adminCheck(admus.ADM.Name, admus.ADM.Password))
                return false;

            return denyDB(admus.US.Id);
        }

        private bool denyDB(int id)
        {
            // Open connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Prepare command and set parameters
                SqlCommand insertCommand = new SqlCommand(_qSetDenied, connection);
                insertCommand.Parameters.AddWithValue("@Id", id);
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
            }

            // Else we return true
            return true;
        }
    }
}
