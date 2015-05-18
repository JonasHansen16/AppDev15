using nah_back.Providers;
using nah_backend.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web.Http;

namespace nah_backend.Controllers
{
    public class FormController : ApiController
    {
        private string _qUser = "SELECT id FROM usr WHERE username = @UserName AND pass = @Password AND denied = 0 AND active = 1;";

        private string _qMail = "SELECT email FROM usr WHERE username = @UserName AND pass = @Password AND denied = 0 AND active = 1;";

        private string _qClient = "SELECT id, hash, age, start, done, func FROM client WHERE formid = @FormId ;";

        private string _qFormCount = "SELECT COUNT(id) FROM form WHERE userid = @Uid ;";

        private string _qQuestionnaireExists = "SELECT id FROM questionnaire WHERE active = 1 AND id = @Id ;";

        private string _qFormExists = "SELECT id FROM form WHERE id = @Id AND userid = @Uid;";

        private string _qFormCheck = "UPDATE form SET checkedreport = 1 WHERE id = @Id;";

        private string _qGetAnswers = "SELECT qid, score, help FROM answer WHERE clientid = @Id AND final = 1;";

        private string _qGetQuestions =
            "SELECT question.id, question.txt, question.title " +
            "FROM form, questionnaire, questionlist, question " +
            "WHERE form.id = @Id " +
            "AND form.airid = questionnaire.id " +
            "AND questionnaire.id = questionlist.airid " +
            "AND questionlist.active = 1 " +
            "AND questionlist.qid = question.id " +
            ";";

        private string _qFormInsert = "INSERT INTO form (airid, userid, memo, category, relation, completed, checkedreport) VALUES (@Aid, @Uid, @Memo, @Category, @Relation, 0, 0);";

        private string _qLatestForm = "SELECT TOP(1) id FROM form WHERE userid = @Uid ORDER BY id DESC;";

        private string _qClientInsert = "INSERT INTO client (formid, hash, age, start, done, func) VALUES (@Fid, @Hash, @Age, 1, 0, @Function);";

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

        // POST api/form/Report
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// get the report of a form.
        /// </summary>
        /// <param name="usfo">
        /// An USFO object which contains, in order:
        /// -The user attempting to access the report (used: UserName and Password fields)
        /// -The form the user wants the report of (used: Id field)
        /// </param>
        /// <returns>A Report object containing the report. 
        /// Null if the user does not exist or if the form does not exist 
        /// or if the user does not have access to the form,
        /// or if a database error occurs.</returns>
        [AllowAnonymous]
        [Route("api/form/Report")]
        public Report Report(USFO usfo)
        {
            // First we make sure the user exists
            if (usfo.US == null)
                return null;
            usfo.US.Id = userCheck(usfo.US.UserName, usfo.US.Password);
            if (usfo.US.Id == -1)
                return null;

            // Then we make sure the form exists and the user has access to it
            if (usfo.FO == null || !formExists(usfo.FO.Id, usfo.US.Id))
                return null;

            // If the user has access to the form, we set its checkedreport to true
            if (!checkForm(usfo.FO.Id))
                return null;

            // And then we return our report.
            return generateReport(usfo.FO.Id);
        }

        /// <summary>
        /// This function will generate a report of a form
        /// by getting its clients, and then for each client
        /// getting their answers, and then filtering those
        /// lists of answers.
        /// </summary>
        /// <param name="id">The id of the form for which to generate a report.</param>
        /// <returns>The generated Report.</returns>
        private Report generateReport(int id)
        {
            Report output = new Report();
            // Get our clients
            output.ClientList = clientsDB(id);
            // Get our questions
            output.QuestionList = getQuestions(id);
            // For each client, get their list of answers and add it to the list of answer lists.
            foreach (ClientExp cl in output.ClientList)
            {
                List<Answer> al = getAnswers(cl.Id);
                output.AnswerList.Add(al);
            }
            // Now, we will filter the answerlists, keeping only a select few.
            filterLists(output.QuestionList, output.AnswerList);
            // And we return our filtered output.
            return output;
        }

        /// <summary>
        /// This function will filter the passed list of answerlists
        /// by first removing all answers which are not in the questionlist,
        /// and then by filtering all answers for which not every client
        /// requested help.
        /// </summary>
        /// <param name="ql">The list of questions.</param>
        /// <param name="lal">The list of answerlists.</param>
        private void filterLists(List<Question> ql, List<List<Answer>> lal)
        {
            // Filter out all the answers which do not have a corresponding question and all the answers which do not have a corresponding help request
            foreach (List<Answer> la in lal)
                foreach (Answer a in la)
                    if (!contains(ql, a) || !anyHelp(lal, a))
                        la.Remove(a);
        }

        /// <summary>
        /// Returns true if any of the answers for the 
        /// same question as the passed answer request help
        /// </summary>
        /// <param name="lal">The list of answer lists.</param>
        /// <param name="a">The answer.</param>
        /// <returns>True if any of the answers for the same question 
        /// as the passed answer request help, false otherwise.</returns>
        private bool anyHelp(List<List<Answer>> lal, Answer a)
        {
            foreach (List<Answer> la in lal)
                foreach (Answer curr in la)
                    if (curr.QuestionID == a.QuestionID && curr.Help)
                        return true;

            return false;
        }

        /// <summary>
        /// Checks whether the questionlist contains the question answered in the answer.
        /// </summary>
        /// <param name="ql">The questionlist.</param>
        /// <param name="a">The answer.</param>
        /// <returns>True if the questionlist contains the answer, false otherwise.</returns>
        private bool contains(List<Question> ql, Answer a)
        {
            foreach (Question q in ql)
                if (q.Id == a.QuestionID)
                    return true;

            return false;
        }

        /// <summary>
        /// Sets the checkedreport of a Form to true.
        /// </summary>
        /// <param name="id">The id of the form.</param>
        /// <returns>True if successful, false otherwise.</returns>
        private bool checkForm(int id)
        {
            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = new SqlCommand(_qFormCheck, connection);
            insertCommand.Parameters.AddWithValue("@Id", id);

            // Execute our query
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
        /// Returns all of a client's final answers.
        /// </summary>
        /// <param name="id">The id of the client.</param>
        /// <returns>A list of the client's final answers.</returns>
        private List<Answer> getAnswers(int id)
        {
            List<Answer> output = new List<Answer>();

            if (id < 0)
                return output;

            // Get connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qGetAnswers, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", id);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader();
                // Put our results in a list of Answers
                while (reader.Read())
                {
                    Answer a = new Answer();
                    a.QuestionID = reader.GetInt32(0);
                    a.Score = reader.GetInt32(1);
                    a.Help = reader.GetBoolean(2);
                    output.Add(a);
                }
            }

            // Return our list of answers
            return output;
        }

        /// <summary>
        /// Returns all of a form's questions.
        /// </summary>
        /// <param name="id">The id of the form.</param>
        /// <returns>A list of the form's questions.</returns>
        private List<Question> getQuestions(int id)
        {
            List<Question> output = new List<Question>();

            if (id < 0)
                return output;

            // Get connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qGetQuestions, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", id);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader();
                // Put our results in a list of Questions
                while (reader.Read())
                {
                    Question q = new Question();
                    q.Id = reader.GetInt32(0);
                    q.Text = reader.GetString(1);
                    q.Title = reader.GetString(2);
                    output.Add(q);
                }
            }

            // Return our list of answers
            return output;
        }


        /// <summary>
        /// Returns true if the form indicated by its 
        /// id exists and the user indicated by uid has 
        /// acccess to it, and false otherwise.
        /// </summary>
        /// <param name="id">The id of the form.</param>
        /// <param name="uid">The id of the user.</param>
        /// <returns>True if the form indicated by its id exists and the user has access to it, false otherwise.</returns>
        private bool formExists(int id, int uid)
        {
            // Get connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qFormExists, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", id);
                selectCommand.Parameters.AddWithValue("@Uid", uid);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return true; // We return true
            }

            // If the questionnaire does not exist, we return false
            return false;
        }

        // POST api/form/New
        /// <summary>
        /// This POST api function will allow the user of the API to
        /// create a new form in the database. In practice, this
        /// equates to creating a new questionnaire for users to
        /// fill in, although it will not create a new questionnaire
        /// in the database.
        /// </summary>
        /// <param name="usfoqu">
        /// An USFOQU object which contains, in order:
        /// -The user attempting to create the form (used: UserName and Password fields)
        /// -The form the user wants to create
        /// -The questionnaire the user wants the clients to fill in (used: Id field)
        /// </param>
        /// <returns>True if the form was successfully created, false otherwise.</returns>
        [AllowAnonymous]
        [Route("api/form/New")]
        public bool New(USFOQU usfoqu)
        {
            // First we make sure the user exists
            if (usfoqu.US == null)
                return false;
            usfoqu.US.Id = userCheck(usfoqu.US.UserName, usfoqu.US.Password);
            if (usfoqu.US.Id == -1)
                return false;

            // We fetch the user's email
            usfoqu.US.Email = userMail(usfoqu.US.UserName, usfoqu.US.Password);

            // Then we make sure the questionnaire exists
            if (usfoqu.QU == null || !questionnaireExists(usfoqu.QU.Id))
                return false;

            // And then we make sure all the required data is present.
            bool success = formDBCheck(usfoqu.FO);

            // If all the data is present,
            if (success)
            {   // we make sure the data fits the database.
                formDBInsertMod(usfoqu.FO);
                // And finally we try to insert the data into the database.
                success = formDBInsert(usfoqu.FO, usfoqu.US.Id, usfoqu.QU.Id);
            }

            // If the form was successfully inserted, we send a mail to the user informing him of this.
            sendMail(usfoqu.FO, usfoqu.US);

            // Return whether we were succesful or not.
            return success;
        }

        /// <summary>
        /// Returns the user's email.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="pass">The hashed password of the user.</param>
        /// <returns>The user's email if the users exists, an empty string otherwise.</returns>
        private string userMail(string name, string pass)
        {
            if (name == null || name == "" || pass == null || pass == "")
                return "";

            // Get connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qMail, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@UserName", name);
                selectCommand.Parameters.AddWithValue("@Password", pass);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return reader.GetString(0); // We return the email of the user
            }

            // If the user does not exist, we return the empty string
            return "";
        }

        /// <summary>
        /// Sends a mail containing the clients' data,
        /// and a link on how to get to their questionnaires.
        /// </summary>
        /// <param name="fo">The form the mail is about.</param>
        /// <param name="us">The user to send the mail to.</param>
        private void sendMail(Form fo, User us)
        {
            // Set from and to
            string from = ConfigurationManager.AppSettings["email"];
            string to = us.Email;

            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress(from, ConfigurationManager.AppSettings["emailfrom"], System.Text.Encoding.UTF8);
            mail.Subject = ConfigurationManager.AppSettings["emailsubject"];
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = "";

            for (int i = 0; i < fo.ClientList.Count; i++)
            {
                mail.Body += "Client " + i + 1 + 
                    "<br/>Functie: " + fo.ClientList[i].Function +
                    "<br/>ID: " + fo.ClientList[i].Id + 
                    "<br/>Hash: " + fo.ClientList[i].Hash + 
                    "<br/>Link: " + ConfigurationManager.AppSettings["websitebase"] +
                    ConfigurationManager.AppSettings["websitenav"] +
                    "?uid=" + fo.ClientList[i].Id + "&hash=" + fo.ClientList[i].Hash;
            }
                    

            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;

            SmtpClient client = new SmtpClient();
            //Add the Creddentials- use your own email id and password

            client.Credentials = new System.Net.NetworkCredential(from, ConfigurationManager.AppSettings["emailpass"]);

            client.Port = 587; // Gmail works on this port
            client.Host = ConfigurationManager.AppSettings["emailhost"];
            client.EnableSsl = true; //Gmail works on Server Secured Layer
            try
            {
                client.Send(mail);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Returns true if the questionnaire indicated by its 
        /// qid exists and is active, and false otherwise.
        /// </summary>
        /// <param name="qid">The id of the questionnaire.</param>
        /// <returns>True if the questionnaire indicated by its qid exists and is active, false otherwise.</returns>
        private bool questionnaireExists(int qid)
        {
            // Get connection
            using (SqlConnection connection = DatabaseAccessProvider.GetConnection())
            {
                // Create command
                SqlCommand selectCommand = new SqlCommand(_qQuestionnaireExists, connection);
                // Set parameters
                selectCommand.Parameters.AddWithValue("@Id", qid);
                // Open connection
                connection.Open();
                // Execute query
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                // If we have a result
                if (reader.Read())
                    return true; // We return true
            }

            // If the questionnaire does not exist, we return false
            return false;
        }

        /// <summary>
        /// This function will check whether all required 
        /// data fields of the passed Form object are set.
        /// </summary>
        /// <param name="input">The Form object to check.</param>
        /// <returns>True if the Form object has all mandatory fields, false otherwise.</returns>
        private bool formDBCheck(Form input)
        {
            if (input == null)
                return false;

            if (input.Memo == null)
                return false;

            if (input.Category == null)
                return false;

            if (input.Relation == null)
                return false;

            if (input.ClientList == null || input.ClientList.Count < DatabaseData.Form.ClientList.minlen)
                return false;

            foreach (ClientExp cl in input.ClientList)
                if (!clientDBCheck(cl))
                    return false;

            return true;
        }

        /// <summary>
        /// This function will check whether all required data
        /// fields of the passed ClientExp object are set.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool clientDBCheck(ClientExp input)
        {
            if (input.Age < 0)
                return false;

            if (input.Function == null)
                return false;

            return true;
        }

        /// <summary>
        /// This method will modify the passed Form and make sure it is fit for inserting into
        /// the database by trimming its string fields accordingly. It will also generate a
        /// hash for each client, and trim that accordingly.
        /// </summary>
        /// <param name="input">The user to modify.</param>
        private void formDBInsertMod(Form input)
        {
            // Alter all strings to their respective lengths
            input.Memo = stringDBMod(input.Memo, DatabaseData.Form.Memo.maxlen);
            input.Category = stringDBMod(input.Category, DatabaseData.Form.Category.maxlen);
            input.Relation = stringDBMod(input.Relation, DatabaseData.Form.Relation.maxlen);

            // Foreach client
            foreach (ClientExp cl in input.ClientList)
            {
                // Generate hash
                cl.Hash = generateHash();
                // Make sure it fits the database
                clientDBInsertMod(cl);
            }

        }

        /// <summary>
        /// This method will modify the passed ClientExp and make sure it is fit for inserting into
        /// the database by trimming its string fields accordingly.
        /// </summary>
        /// <param name="input">The user to modify.</param>
        private void clientDBInsertMod(ClientExp input)
        {
            input.Hash = stringDBMod(input.Hash, DatabaseData.Client.Hash.maxlen);
            input.Function = stringDBMod(input.Hash, DatabaseData.Client.Function.maxlen);
        }

        /// <summary>
        /// Generates a random hash string.
        /// </summary>
        /// <returns>A randomly generated string.</returns>
        private string generateHash()
        {
            // Generate random bytes
            var bytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            // Return the random bytes converted to a random string, 
            // with hyphens removed and after it has been converted to lower case.
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// This function will attempt to insert a form into the database.
        /// </summary>
        /// <param name="toInsert">The form to insert.</param>
        /// <param name="uid">The id of the user who is inserting the form.</param>
        /// <param name="qid">The id of the questionnaire the clients of the form should fill in.</param>
        /// <returns>True if the form was successfully inserted, false otherwise.</returns>
        private bool formDBInsert(Form toInsert, int uid, int qid)
        {
            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = generateInsertCommand(toInsert, uid, qid, connection);
            SqlCommand selectCommand = new SqlCommand(_qLatestForm, connection);

            int fid = -1;

            try
            {
                connection.Open();
                // Insert our form
                insertCommand.ExecuteNonQuery();
                // Get the id of the form we just inserted
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                    fid = reader.GetInt32(0);
                reader.Close();
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

            // If we did not find our form, return false
            if (fid == -1)
                return false;

            // Else, insert all our clients
            foreach (ClientExp cl in toInsert.ClientList)
                if (!clientDBInsert(cl, fid))
                    return false;

            // Else we return true
            return true;
        }

        /// <summary>
        /// Inserts a client into the database.
        /// </summary>
        /// <param name="toInsert">The client to insert.</param>
        /// <param name="fid">The form the client should fill in.</param>
        /// <returns>Whether or not the insertion was successful.</returns>
        private bool clientDBInsert(ClientExp toInsert, int fid)
        {
            // Open connection and set parameters
            SqlConnection connection = DatabaseAccessProvider.GetConnection();
            SqlCommand insertCommand = new SqlCommand(_qClientInsert, connection);
            insertCommand.Parameters.AddWithValue("@Fid", fid);
            insertCommand.Parameters.AddWithValue("@Hash", toInsert.Hash);
            insertCommand.Parameters.AddWithValue("@Age", toInsert.Age);
            insertCommand.Parameters.AddWithValue("@Function", toInsert.Function);

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
        /// inserting the passed Form into the database.
        /// </summary>
        /// <param name="toInsert">The Form to insert.</param>
        /// <param name="uid">The id of the user.</param>
        /// <param name="qid">The id of the questionnaire.</param>
        /// <param name="connection">The database connection.</param>
        /// <returns>An SqlCommand capable of inserting the Form.</returns>
        private SqlCommand generateInsertCommand(Form toInsert, int uid, int qid, SqlConnection connection)
        {
            SqlCommand insertCommand = new SqlCommand(_qFormInsert, connection);
            insertCommand.Parameters.AddWithValue("@Aid", qid);
            insertCommand.Parameters.AddWithValue("@Uid", uid);
            insertCommand.Parameters.AddWithValue("@Memo", toInsert.Memo);
            insertCommand.Parameters.AddWithValue("@Category", toInsert.Category);
            insertCommand.Parameters.AddWithValue("@Relation", toInsert.Relation);
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
