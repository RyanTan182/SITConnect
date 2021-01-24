using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        static String lockStatus;
        static int attemptcount = 0;
        string lastfailedattempt = DateTime.Now.ToString();
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_submit_Click(object sender, EventArgs e)
        {
            string pwd = tb_password.Text.ToString().Trim();
            string userid = tb_email.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            int dbcount = getFailedAttemptCount(userid);
            string checkemail = getEmail();

            try
            {
                if (ValidateCaptcha())
                {
                    if (checkemail != null)
                    {

                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);
                            if (dbcount == 3)
                            {
                                DateTime unlockedtime = Convert.ToDateTime(getDateTime(userid));
                                if (DateTime.Now < unlockedtime)
                                {
                                    errorMsg.Visible = true;
                                    errorMsg.Text = "Your Account has been locked out due to many failed login attempts just now. Please refresh and try again later";
                                }
                                else if (DateTime.Now > unlockedtime)
                                {
                                    resetFailedAttemptCount(userid);
                                    updateLoginTime(userid, DateTime.Now);
                                    errorMsg.Visible = true;
                                    errorMsg.Text = "Sorry for the wait! Your account has been automatically unlocked! Please try logging in again now";
                                }
                            }
                            else if (userHash.Equals(dbHash))
                            {
                                if (dbcount < 3)
                                {
                                    Session["LoggedIn"] = tb_email.Text.Trim();
                                    Session["SSEmail"] = tb_email.Text;

                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;

                                    resetFailedAttemptCount(userid);
                                    updateLoginTime(userid, DateTime.Now);
                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                    Response.Redirect("HomePage.aspx", false);
                                }
                                else if (dbcount >= 3)
                                {
                                    errorMsg.Visible = true;
                                    errorMsg.Text = "Your Account has been locked out due to many failed login attempts just now. Please contact adminstrator.";
                                }
                            }
                            else if (dbcount == 2)
                            {
                                int updatedattemptcount = dbcount + 1;
                                updateLoginTime(userid, DateTime.Now.AddMinutes(1));
                                updateFailedAttemptCount(userid, updatedattemptcount);

                            }
                            else
                            {
                                errorMsg.Visible = true;
                                errorMsg.Text = "Login Failed!";
                                dbcount += 1;
                                updateFailedAttemptCount(userid, dbcount);
                            }
                        }
                    }
                    else
                    {

                        errorMsg.Visible = true;
                        errorMsg.Text = "Login Failed!";

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }

        }
        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter.
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :)
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret Key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6LdO-RMaAAAAAM0klkSvO3BKXDVwXvt4IdajTRmt &response=" + captchaResponse);

            try
            {

                //Codes to receive the Response in JSON formart from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        //The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        //To show the JSON response string for learning purpose

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonObject to handle the response e.g. success or Error
                        //Deserialize Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        //Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected string getDBHash(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected string getEmail()
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Email FROM Account";
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != null)
                        {
                            if (reader["Email"] != DBNull.Value)
                            {
                                s = reader["Email"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected int getFailedAttemptCount(string email)
        {
            int g = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select FailedAttemptCount FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["FailedAttemptCount"] != null)
                        {
                            if (reader["FailedAttemptCount"] != DBNull.Value)
                            {
                                g = int.Parse(reader["FailedAttemptCount"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return g;
        }

        protected int updateFailedAttemptCount(string email,int failedattemptcount)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account SET FailedAttemptCount=@ParaFailedAttemptCount WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            command.Parameters.AddWithValue("@ParaFailedAttemptCount", failedattemptcount);
            connection.Open();
            command.CommandText = sql;
            command.Connection = connection;
            int result=command.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        protected int resetFailedAttemptCount(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account SET FailedAttemptCount=@paraFailedAttemptCount WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            command.Parameters.AddWithValue("@paraFailedAttemptCount",0);
            //command.Parameters.AddWithValue("@ParaFailedAttemptCount", failedattemptcount);
            connection.Open();
            command.CommandText = sql;
            command.Connection = connection;
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        protected int updateLoginTime(string email,DateTime updatelogintime)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account SET UpdateLoginTime=@paraUpdateLoginTime WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            command.Parameters.AddWithValue("@paraUpdateLoginTime", updatelogintime);
            connection.Open();
            command.CommandText = sql;
            command.Connection = connection;
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        protected string getDateTime(string email)
        {
            string s=null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select UpdateLoginTime FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["UpdateLoginTime"] != null)
                        {
                            if (reader["UpdateLoginTime"] != DBNull.Value)
                            {
                                s =reader["UpdateLoginTime"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected void btn_register_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx");
        }

        protected void btn_forgot_Click(object sender, EventArgs e)
        {
            Response.Redirect("ForgotPassword.aspx");
        }
    }
}