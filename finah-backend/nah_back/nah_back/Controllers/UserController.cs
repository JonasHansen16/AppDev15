using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using nah_back.Models;
using nah_back.Providers;
using System.Data.SqlClient;
using System.Text;

namespace nah_back.Controllers
{
    public class UserController : ApiController
    {
        private string _qInsertUser = "INSERT INTO usr (name, lastname, email, username, pass, occupation, adm, active, denied) " +
                                      "VALUES (@Name, @LastName, @Email, @UserName, @Password, @Occupation, @Admin, @Active, @Denied);";

        // POST api/User/Register
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// insert a new user into the database using the appropriate
        /// data class. New users will not have admin privileges, they
        /// will start as inactive and they will not be denied. Certain
        /// text fields may be truncated to fit the database.
        /// </summary>
        /// <param name="user">The user to register.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Register")]
        public HttpResponseMessage Register(nah_back.Models.User user)
        {
            HttpResponseMessage resp = new HttpResponseMessage();

            // First we make sure all the required data is present.
            bool success = userDBCheck(user);            

            // If this succeeded,
            if (success)
            {   // we make sure the data fits the database
                userDBInsertMod(user);
                // And finally we try to insert the data into the database
                success = userDBInsert(user);
            }
                
            if (success)
            {
                resp.Content = new StringContent("OK", Encoding.Unicode);
                return resp;
            }

            resp.Content = new StringContent("NOK", Encoding.Unicode);
            return resp;
        }

        /// <summary>
        /// This function will check whether all required 
        /// data fields of the passed User object are set 
        /// and bigger than their required minimum lengths.
        /// </summary>
        /// <param name="input">The User object to check.</param>
        /// <returns>True if the User object has all mandatory fields, false otherwise.</returns>
        private bool userDBCheck(nah_back.Models.User input)
        {
            bool output = true;

            if (input.Name == null || input.Name.Length < DatabaseData.User.Name.minlen)
                output = false;

            if (input.LastName == null || input.LastName.Length < DatabaseData.User.LastName.minlen)
                output = false;

            if (input.Email == null || input.Email.Length < DatabaseData.User.Email.minlen)
                output = false;

            if (input.UserName == null || input.UserName.Length < DatabaseData.User.UserName.minlen)
                output = false;

            if (input.Password == null || input.Password.Length < DatabaseData.User.Password.minlen)
                output = false;

            if (input.Occupation == null)
                output = false;

            return output;
        }

        /// <summary>
        /// This method will modify the passed User and make sure it is fitfor inserting into
        /// the database by trimming its string fields accordingly and setting all its boolean 
        /// values to false.
        /// </summary>
        /// <param name="input">The user to modify.</param>
        private void userDBInsertMod(nah_back.Models.User input)
        {
            // Alter all strings to their respective lengths
            input.Name = stringDBMod(input.Name, DatabaseData.User.Name.maxlen);
            input.LastName = stringDBMod(input.LastName, DatabaseData.User.LastName.maxlen);
            input.Email = stringDBMod(input.Email, DatabaseData.default_maxlen);
            input.UserName = stringDBMod(input.UserName, DatabaseData.User.UserName.maxlen);
            input.Password = stringDBMod(input.Password, DatabaseData.default_maxlen);
            input.Occupation = stringDBMod(input.Occupation, DatabaseData.default_maxlen);
            input.Admin = false;
            input.Active = false;
            input.Denied = false;
        }

        /// <summary>
        /// This function will attempt to insert a user into the database.
        /// </summary>
        /// <param name="toInsert">The user to insert.</param>
        /// <returns>True if the user was successfully inserted, false otherwise.</returns>
        private bool userDBInsert(User toInsert)
        {
            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = generateInsertCommand(toInsert, connection);
            
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
        /// This function will generate an SqlCommand capable of 
        /// inserting the passed User into the database.
        /// </summary>
        /// <param name="toInsert">The User to insert.</param>
        /// <param name="connection">The database connection.</param>
        /// <returns>An SqlCommand capable of inserting the user.</returns>
        private SqlCommand generateInsertCommand(User toInsert, SqlConnection connection)
        {
            SqlCommand insertCommand = new SqlCommand(_qInsertUser, connection);
            insertCommand.Parameters.AddWithValue("@Name", toInsert.Name);
            insertCommand.Parameters.AddWithValue("@LastName", toInsert.LastName);
            insertCommand.Parameters.AddWithValue("@Email", toInsert.Email);
            insertCommand.Parameters.AddWithValue("@UserName", toInsert.UserName);
            insertCommand.Parameters.AddWithValue("@Password", toInsert.Password);
            insertCommand.Parameters.AddWithValue("@Occupation", toInsert.Occupation);
            insertCommand.Parameters.AddWithValue("@Admin", toInsert.Admin);
            insertCommand.Parameters.AddWithValue("@Active", toInsert.Active);
            insertCommand.Parameters.AddWithValue("@Denied", toInsert.Denied);
            return insertCommand;
        }

        /// <summary>
        /// Trims the input string and limits it to the specified
        /// maximum amount of characters.
        /// </summary>
        /// <param name="input">The string to alter.</param>
        /// <param name="maxchars">The maximum amount of characters the string may contain.</param>
        /// <returns>The input string, trimmed and reduced to the specified amount of characters if necessary.</returns>
        private string stringDBMod(string input, int maxchars)
        {
            input = input.Trim();
            if(input.Length > maxchars)
                input = input.Substring(0, maxchars);
            return input;
        }

        // GET: api/User/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/User/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }
    }
}
