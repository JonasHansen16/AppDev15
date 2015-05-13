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
    public class FormController : ApiController
    {
        private string _qUser = "SELECT id FROM usr WHERE username = @UserName AND pass = @Password AND denied = 0 AND active = 1;";

        private string _qClient = "SELECT id, hash, age, start, done, func FROM client WHERE formid = @FormId ;";

        private string _qFormCount = "SELECT COUNT(id) FROM form WHERE userid = @Uid ;";

        private string _qForm = 
            "WITH largelatest AS " +
            "( " +
            "    SELECT TOP(@Start + @Max) id, memo, category, relation, completed, checkedreport " +
            "    FROM form " +
            "    WHERE userid = @Uid " +
            "    ORDER BY id DESC " +
            "), " +
            "latest AS " +
            "( " +
            "    SELECT TOP(@Max) id, memo, category, relation, completed, checkedreport " +
            "    FROM largelatest " +
            "    ORDER BY id ASC " +
            ") " +
            "SELECT id, memo, category, relation, completed, checkedreport " +
            "FROM latest " +
            "ORDER BY id DESC " +
            ";";

        // POST api/form/All
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// get a list of up to DatabaseData.FormList.maxlen forms, 
        /// ranked newest-first, starting from the passed number.
        /// </summary>
        /// <param name="usst">An USST object which contains a user and the amount of forms to be skipped at the start of the list.</param>
        /// <returns>
        /// A list of forms. 
        /// Null if the user does not exist or the passed amount is less than zero. 
        /// The last full list of forms if there are no such forms.
        /// </returns>
        [AllowAnonymous]
        [Route("api/form/All")]
        public List<Form> All(USST usst)
        {
            // First we make sure the passed number is positive
            if (usst.ST < 0)
                return null;

            // Then we make sure the passed user exists
            int usID = userCheck(usst.US.UserName, usst.US.Password);

            if (usID == -1)
                return null;

            // If the user exists, we can use his id
            usst.US.Id = usID;

            // We fetch the list of forms and return it
            return formsDB(usst.US.Id, usst.ST, DatabaseData.FormList.maxlen);
        }

        /// <summary>
        /// Checks whether the user identified by his name and
        /// his password exists or not.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="pass">The hashed password of the user.</param>
        /// <returns>The user's id if the users exists, -1 otherwise.</returns>
        private int userCheck(string name, string pass)
        {
            if (name == null || name == "" || pass == null || pass == "")
                return -1;

            // Get connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qUser, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@UserName", name);
                selectCommand.Parameters.AddWithValue("@Password", pass);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return reader.GetInt32(0); // We return the id of the user
            }

            // If the user does not exist, we return -1
            return -1;
        }

        /// <summary>
        /// Searches the database for a number of forms, ranked chronologically.
        /// Returns the latest max forms, with start offset.
        /// </summary>
        /// <param name="userid">The id of the user who's forms to search for.</param>
        /// <param name="offset">The offset of forms to skip.</param>
        /// <param name="max">The maximum amount of forms to return.</param>
        /// <returns>A list of forms, which will be empty if a negative argument was passed. 
        /// The last list of max forms if there are no such forms.</returns>
        private List<Form> formsDB(int userid, int offset, int max)
        {
            List<Form> output = new List<Form>();

            if (offset < 0 || max < 0 || userid < 0)
                return output;

            // First we get the list of forms without Clients
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qForm, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Max", max);
                selectCommand.Parameters.AddWithValue("@Start", offset);
                selectCommand.Parameters.AddWithValue("@Uid", userid);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader();
                // Read results
                while (reader.Read())
                {
                    // Turn it into a User and return it
                    //id, name, lastname, email, username, occupation, adm, active, denied
                    Form toAdd = new Form();
                    toAdd.Id = reader.GetInt32(0);
                    toAdd.Memo = reader.GetString(1);
                    toAdd.Category = reader.GetString(2);
                    toAdd.Relation = reader.GetString(3);
                    toAdd.Completed = reader.GetBoolean(4);
                    toAdd.CheckedReport = reader.GetBoolean(5);
                    output.Add(toAdd);
                }
            }

            // Then, for each form, we get the matching clients
            foreach (Form f in output)
                f.ClientList = clientsDB(f.Id);

            // Return our list
            return output;
        }

        /// <summary>
        /// Searches the database for a number of clients with the specified formid.
        /// </summary>
        /// <param name="formid">The id of the form to search clients of.</param>
        /// <returns>A list of clients, which will be empty if a negative argument was passed.</returns>
        private List<ClientExp> clientsDB(int formid)
        {
            List<ClientExp> output = new List<ClientExp>();

            if (formid < 0)
                return output;

            // Get the clients with the specified formid
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qClient, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@FormId", formid);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader();
                // Read results
                while (reader.Read())
                {
                    // Turn it into a ClientExp and add it to the list
                    ClientExp toAdd = new ClientExp();
                    toAdd.Id = reader.GetInt32(0);
                    toAdd.Hash = reader.GetString(1);
                    toAdd.Age = reader.GetInt32(2);
                    toAdd.Start = reader.GetBoolean(3);
                    toAdd.Done = reader.GetBoolean(4);
                    toAdd.Function = reader.GetString(5);
                    output.Add(toAdd);
                }
            }

            // Return our list
            return output;
        }

        // POST api/form/Count
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// get the amount of forms a certain user has.
        /// </summary>
        /// <param name="usst">A User object which contains the name and the hashed password of the user.</param>
        /// <returns>
        /// The amount of forms that user has, or -1 if the user does not exist.
        /// </returns>
        [AllowAnonymous]
        [Route("api/form/Count")]
        public int Count(User user)
        {
            // First we make sure the passed user exists
            int usID = userCheck(user.UserName, user.Password);

            if (usID == -1)
                return -1;

            // If the user exists, we can use his id
            user.Id = usID;

            //  We fetch the amount of forms and return it
            return formCountDB(user.Id);
        }

        /// <summary>
        /// Gets the amount of forms a certain user has in the database.
        /// </summary>
        /// <param name="uid">The user's id.</param>
        /// <returns>The amount of forms the user has, or -1 if something goes wrong or a negative parameter is passed.</returns>
        private int formCountDB(int uid)
        {
            if (uid < 0)
                return -1;

            // Get connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qFormCount, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Uid", uid);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return reader.GetInt32(0); // We return the amount of forms
            }

            // If someting goes wrong, we return -1
            return -1;
        }
    }
}
