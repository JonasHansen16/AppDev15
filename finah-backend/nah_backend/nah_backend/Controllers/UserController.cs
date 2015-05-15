using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using nah_backend.Models;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using nah_back.Providers;

namespace nah_back.Controllers
{
    public class UserController : ApiController
    {
        private string _qInsert = "INSERT INTO usr (name, lastname, email, username, pass, occupation, adm, active, denied) " +
                                      "VALUES (@Name, @LastName, @Email, @UserName, @Password, @Occupation, @Admin, @Active, @Denied);";

        private string _qExists = "SELECT id FROM usr WHERE username = @UserName;";

        private string _qDenied = "SELECT id FROM usr WHERE username = @UserName AND denied = 1;";

        private string _qActive = "SELECT id FROM usr WHERE username = @UserName AND active = 1;";

        private string _qLogin = "SELECT id, name, lastname, email, username, occupation, adm, active, denied FROM usr WHERE username = @UserName AND pass = @Password AND denied = 0 AND active = 1;";

        // POST api/user/Register
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// insert a new user into the database using the appropriate
        /// data class. New users will not have admin privileges, they
        /// will start as inactive and they will not be denied. Certain
        /// text fields may be truncated to fit the database.
        /// If the operation is successful, this function will return
        /// true; else false.
        /// </summary>
        /// <param name="user">The user to register.</param>
        /// <returns>True if the registration was successful, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/user/Register")]
        public bool Register(User user)
        {
            // First we make sure all the required data is present.
            bool success = userDBCheck(user);

            // If all the data is present, we check if the user does not yet exist
            if (success)
                success = !Exists(user);

            // If the user does not yet exist
            if (success)
            {   // we make sure the data fits the database.
                userDBInsertMod(user);
                // And finally we try to insert the data into the database.
                success = userDBInsert(user);
            }

            // Return whether we were succesful or not.
            return success;
        }

        /// <summary>
        /// This function will check whether all required 
        /// data fields of the passed User object are set 
        /// and bigger than their required minimum lengths.
        /// </summary>
        /// <param name="input">The User object to check.</param>
        /// <returns>True if the User object has all mandatory fields, false otherwise.</returns>
        private bool userDBCheck(User input)
        {
            if (input.Name == null || input.Name.Length < DatabaseData.User.Name.minlen)
                return false;

            if (input.LastName == null || input.LastName.Length < DatabaseData.User.LastName.minlen)
                return false;

            if (input.Email == null || input.Email.Length < DatabaseData.User.Email.minlen)
                return false;

            if (input.UserName == null || input.UserName.Length < DatabaseData.User.UserName.minlen)
                return false;

            if (input.Password == null || input.Password.Length < DatabaseData.User.Password.minlen)
                return false;

            if (input.Occupation == null)
                return false;

            return true;
        }

        /// <summary>
        /// This method will modify the passed User and make sure it is fitfor inserting into
        /// the database by trimming its string fields accordingly and setting all its boolean 
        /// values to false.
        /// </summary>
        /// <param name="input">The user to modify.</param>
        private void userDBInsertMod(User input)
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
            SqlCommand insertCommand = new SqlCommand(_qInsert, connection);
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
            if (input.Length > maxchars)
                input = input.Substring(0, maxchars);
            return input;
        }

        // POST api/user/Login
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// check if a certain user with a hashed password exists.
        /// If the user exists, and the password is correct, this
        /// function will return all information regarding that user.
        /// If the user does not exist or the password is incorrect,
        /// NULL is returned instead.
        /// The returned password field will never be filled in.
        /// The expected password is MD5-hashed.
        /// </summary>
        /// <param name="user">The user to register.</param>
        /// <returns>A user object, filled with the actual data of the user, 
        /// or NULL if the user does not exist, is inactive or denied. 
        /// The returned password field will never be filled in.</returns>
        [AllowAnonymous]
        [Route("api/user/Login")]
        public User Login(User user)
        {
            return userDBLogin(user.UserName, user.Password);
        }

        /// <summary>
        /// Searches the database for the user with a given name
        /// and hashed password, and returns all data regarding 
        /// the user in the form of a User object. This object
        /// does NOT contain the user's password.
        /// </summary>
        /// <param name="name">The username of the user.</param>
        /// <param name="pass">The hashed password of the user.</param>
        /// <returns>A User object containing all the information of the user, barring the password.</returns>
        private User userDBLogin(string name, string pass)
        {
            if(name == null || name == "" || pass == null || pass == "") 
                return null;

            User output = new User();

            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qLogin, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@UserName", name);
                selectCommand.Parameters.AddWithValue("@Password", pass);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                {
                    // Turn it into a User and return it
                    //id, name, lastname, email, username, occupation, adm, active, denied
                    output.Id = reader.GetInt32(0);
                    output.Name = reader.GetString(1);
                    output.LastName = reader.GetString(2);
                    output.Email = reader.GetString(3);
                    output.UserName = reader.GetString(4);
                    output.Occupation = reader.GetString(5);
                    output.Admin = reader.GetBoolean(6);
                    output.Active = reader.GetBoolean(7);
                    output.Denied = reader.GetBoolean(8);
                    output.Password = "";
                    return output;
                }

                // If the user does not exist, is denied or inactive
                return null; // return NULL
            }
        }

        // POST api/user/Exists
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// check if a user with the specified username exists or not.
        /// This information can be used at registration or at login time 
        /// to provide more detailed feedback.
        /// </summary>
        /// <param name="user">The function will check if this user exists via its username.</param>
        /// <returns>True if the user exists, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/user/Exists")]
        public bool Exists(User user)
        {
            return genericSingleUserQuery(user.UserName, _qExists);
        }

        // POST api/user/Denied
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// check if a user with the specified username is denied or not.
        /// This information can be used at registration or at login time 
        /// to provide more detailed feedback.
        /// </summary>
        /// <param name="user">The function will check if this user is denied via its username.</param>
        /// <returns>True if the user exists and is denied, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/user/Denied")]
        public bool Denied(User user)
        {
            return genericSingleUserQuery(user.UserName, _qDenied);
        }

        // POST api/user/Active
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// check if a user with the specified username is active or not.
        /// This information can be used at registration or at login time 
        /// to provide more detailed feedback.
        /// </summary>
        /// <param name="user">The function will check if this user is active via its username.</param>
        /// <returns>True if the user exists and is active, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/user/Active")]
        public bool Active(User user)
        {
            return genericSingleUserQuery(user.UserName, _qActive);
        }

        /// <summary>
        /// This function is capable of executing any selecting query
        /// that only has '@Username' as a parameter. It will execute
        /// any such passed queries, and check the results. If there
        /// are any results, this function will return true. If there
        /// are no results, it will return false.
        /// </summary>
        /// <param name="username">The username to look for.</param>
        /// <param name="query">The query to execute.</param>
        /// <returns>True if the executed query returned at least one row, false otherwise.
        /// False if the passed username is null or the empty string.</returns>
        private bool genericSingleUserQuery(string username, string query)
        {
            // If the username is null or the empty string, we return false.
            if (username == null || username == "")
                return false;

            // Open connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(query, connection);
                // Set @UserName parameter
                selectCommand.Parameters.AddWithValue("@UserName", username);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return true; // We found the user
                // Else we did not find the user
                return false;
            }
        }
    }
}
