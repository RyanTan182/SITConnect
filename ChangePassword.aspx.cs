using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        static string newfinalHash;
        string lastupdatepassword = DateTime.Now.ToString();
        static string newsalt;

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_email.Text = Session["SSEmail"].ToString();
        }
        public void UpdatePassword()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET passwordhash = @paraPasswordHash where email =  @paraEmail"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@paraEmail", lbl_email.Text);
                            cmd.Parameters.AddWithValue("@paraPasswordHash", newfinalHash);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        private int checkPassword(string password)
        {
            int score = 0;

            //Include implementation

            //Score 0 very weak
            //if length of password is less than 8 chars
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            //Score 2 Weak
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }
            return score;
        }
        protected void btn_submit_Click(object sender, EventArgs e)
        {
            
            bool validInput = ValidateInput();
            string oldpwd = HttpUtility.HtmlEncode(tb_oldpass.Text.ToString().Trim());
            string newcurrentpwd = HttpUtility.HtmlEncode(tb_newpassword.Text.ToString().Trim());
            string email = lbl_email.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);
            string passwordage1 = getPasswordAge1(email);
            string passwordage2 = getPasswordAge2(email);

            //string status = "";
            //switch (scores)
            //{
            //    case 1:
            //        status = "Very Weak";
            //        break;
            //    case 2:
            //        status = "Weak";
            //        break;
            //    case 3:
            //        status = "Medium";
            //        break;
            //    case 4:
            //        status = "Strong";
            //        break;
            //    case 5:
            //        status = "Excellent";
            //        break;
            //    default:
            //        break;
            //}
            //lbl_pwdchecker.Text = "Status : " + status;
            //if (scores < 4)
            //{
            //    lbl_pwdchecker.ForeColor = Color.Red;
            //    return;
            //}
            if (validInput)
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = oldpwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash))
                    {
                        //string pwdage1WithSalt = getPasswordAge1(email) + dbSalt;
                        //string pwdage2WithSalt = getPasswordAge2(email) + dbSalt;
                        //byte[] hashpass1WithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdage1WithSalt));
                        //byte[] hashpass2WithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdage2WithSalt));
                        //string userHash1 = Convert.ToBase64String(hashpass1WithSalt);
                        //string userHash2 = Convert.ToBase64String(hashpass2WithSalt);
                        string currentnewpwdWithSalt = newcurrentpwd + dbSalt;
                        byte[] currentnewhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(currentnewpwdWithSalt));
                        string currentnewuserHash = Convert.ToBase64String(currentnewhashWithSalt);
                        if (currentnewuserHash == passwordage1 || currentnewuserHash == passwordage2 ||currentnewuserHash==dbHash)
                        {
                            errorMsg.Visible = true;
                            errorMsg.Text = "Sorry! But you can't use the same password that was used in the last 2 generation! Please try again!";
                            errorMsg.ForeColor = Color.Red;
                        }
                        else
                        {
                            if (passwordage2 == null)
                            {
                                lbl_pwdchecker.ForeColor = Color.Green;
                                string newpwd =HttpUtility.HtmlEncode( tb_newpassword.Text.ToString().Trim());
                                //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                //byte[] newsaltByte = new byte[8];
                                //rng.GetBytes(newsaltByte);
                                //newsalt = Convert.ToBase64String(newsaltByte);
                                SHA512Managed newhashing = new SHA512Managed();
                                string newpwdWithSalt = newpwd + dbSalt;
                                byte[] plainHash = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpwd));
                                byte[] newhashWithSalt = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpwdWithSalt));
                                newfinalHash = Convert.ToBase64String(newhashWithSalt);
                                UpdatePassword();
                                updatePasswordTime(lbl_email.Text, DateTime.Now.AddMinutes(5), DateTime.Now.AddMinutes(15));
                                updatePasswordAge2(lbl_email.Text, dbHash);
                                Response.Redirect("HomePage.aspx");
                            }
                            else
                            {
                                if (passwordage1 == null)
                                {
                                    lbl_pwdchecker.ForeColor = Color.Green;
                                    string newpwd = HttpUtility.HtmlEncode(tb_newpassword.Text.ToString().Trim());
                                    //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                    //byte[] newsaltByte = new byte[8];
                                    //rng.GetBytes(newsaltByte);
                                    //newsalt = Convert.ToBase64String(newsaltByte);
                                    SHA512Managed newhashing = new SHA512Managed();
                                    string newpwdWithSalt = newpwd + dbSalt;
                                    byte[] plainHash = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpwd));
                                    byte[] newhashWithSalt = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpwdWithSalt));
                                    newfinalHash = Convert.ToBase64String(newhashWithSalt);
                                    UpdatePassword();
                                    updatePasswordTime(lbl_email.Text, DateTime.Now.AddMinutes(5), DateTime.Now.AddMinutes(15));
                                    updatePasswordAge1(lbl_email.Text, passwordage2);
                                    updatePasswordAge2(lbl_email.Text, dbHash);
                                    Response.Redirect("HomePage.aspx");
                                }
                                else
                                {
                                    lbl_pwdchecker.ForeColor = Color.Green;
                                    string newpwd = HttpUtility.HtmlEncode(tb_newpassword.Text.ToString().Trim());
                                    //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                    //byte[] newsaltByte = new byte[8];
                                    //rng.GetBytes(newsaltByte);
                                    //newsalt = Convert.ToBase64String(newsaltByte);
                                    SHA512Managed newhashing = new SHA512Managed();
                                    string newpwdWithSalt = newpwd + dbSalt;
                                    byte[] plainHash = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpwd));
                                    byte[] newhashWithSalt = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpwdWithSalt));
                                    newfinalHash = Convert.ToBase64String(newhashWithSalt);
                                    UpdatePassword();
                                    updatePasswordTime(lbl_email.Text, DateTime.Now.AddMinutes(5), DateTime.Now.AddMinutes(15));
                                    updatePasswordAge1(lbl_email.Text, passwordage2);
                                    updatePasswordAge2(lbl_email.Text, dbHash);
                                    Response.Redirect("HomePage.aspx");
                                }
                            }
                        }
                    }
                    else
                    {
                        errorMsg.Visible = true;
                        errorMsg.Text = "Incorrect current password! Please try again!";
                        errorMsg.ForeColor = Color.Red;
                    }
                }
            }
        }

        protected int updatePasswordTime(string email, DateTime updateminpassword, DateTime updatemaxpassword)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account SET UpdateMinPassword=@paraUpdateMinPassword, UpdateMaxPassword=@paraUpdateMaxPassword WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            command.Parameters.AddWithValue("@paraUpdateMinPassword", updateminpassword);
            command.Parameters.AddWithValue("@paraUpdateMaxPassword", updatemaxpassword);
            connection.Open();
            command.CommandText = sql;
            command.Connection = connection;
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
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
            string sql = "select PasswordSalt FROM Account WHERE Email=@EMAIL";
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

        protected int updatePasswordAge1(string email, string passwordage1)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account SET PasswordAge1= @paraPasswordAge1 WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            command.Parameters.AddWithValue("@paraPasswordAge1", passwordage1);
            connection.Open();
            command.CommandText = sql;
            command.Connection = connection;
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        protected string getPasswordAge1(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordAge1 FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordAge1"] != null)
                        {
                            if (reader["PasswordAge1"] != DBNull.Value)
                            {
                                h = reader["PasswordAge1"].ToString();
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

        protected int updatePasswordAge2(string email, string passwordage2)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account SET PasswordAge2= @paraPasswordAge2 WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            command.Parameters.AddWithValue("@paraPasswordAge2", passwordage2);
            connection.Open();
            command.CommandText = sql;
            command.Connection = connection;
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        protected string getPasswordAge2(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordAge2 FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordAge2"] != null)
                        {
                            if (reader["PasswordAge2"] != DBNull.Value)
                            {
                                h = reader["PasswordAge2"].ToString();
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

        private bool ValidateInput()
        {
            errorMsg.Text = String.Empty;
            errorMsg.ForeColor = Color.Red;

            if (String.IsNullOrEmpty(tb_oldpass.Text))
            {
                errorMsg.Text += "Please type in your current password!" + "<br/>";
            }
            if (String.IsNullOrEmpty(tb_newpassword.Text))
            {
                errorMsg.Text += "Please type in your new password" + "<br/>";
            }
            if (String.IsNullOrEmpty(tb_newconfirmpassword.Text))
            {
                errorMsg.Text += "Please confirm your password!" + "<br/>";
            }
            if (tb_newpassword.Text.ToString() != tb_newconfirmpassword.Text.ToString())
            {
                errorMsg.Text += "Password does not match! Please try again!" + "<br/>";
            }
            int scores = checkPassword(tb_newpassword.Text);
            if (scores < 4)
            {
                errorMsg.Text +="Password is too weak! Please try again!" + "<br/>";
            }

            if (String.IsNullOrEmpty(errorMsg.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}