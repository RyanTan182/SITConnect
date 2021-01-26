using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace SITConnect
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;

        static string finalHash;
        static int failedattemptcount;
        static Nullable<DateTime> updatelogintime;
        static Nullable<DateTime> updateminpassword;
        static Nullable<DateTime> updatemaxpassword;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btn_submit_Click(object sender, EventArgs e)
        {
            // implement codes for the button event
            // Extract data from textbox
            int scores = checkPassword(tb_password.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
            }
            lbl_pwdchecker.Text = "Status : " + status;
            if (scores < 4)
            {
                lbl_pwdchecker.ForeColor = Color.Red;
                return;
            }
            if (ValidateInput())
            {
                lbl_pwdchecker.ForeColor = Color.Green;
                string pwd = tb_password.Text.ToString().Trim();
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                finalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;
                failedattemptcount = 0;
                updatelogintime = null;
                updateminpassword = null;
                updatemaxpassword = null;
                createAccount();
                Response.Redirect("Login.aspx");
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
        public void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @FirstName,@LastName, @CreditCard, @PasswordHash, @PasswordSalt, @IV, @Key, @DateOfBirth, @FailedAttemptCount, @UpdateLoginTime, @UpdateMinPassword, @UpdateMaxPassword)"))
                {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FirstName", tb_firstname.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", tb_lastname.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreditCard", encryptData(tb_creditcard.Text.Trim()));
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DateOfBirth", tb_dob.Text.Trim());
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@FailedAttemptCount", failedattemptcount);
                            cmd.Parameters.AddWithValue("@UpdateLoginTime", DBNull.Value);
                            cmd.Parameters.AddWithValue("@UpdateMinPassword", DateTime.Now.AddMinutes(5));
                            cmd.Parameters.AddWithValue("@UpdateMaxPassword", DateTime.Now.AddMinutes(15));
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
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        private bool ValidateInput()
        {
            errorMsg.Text = String.Empty;
            errorMsg.ForeColor = Color.Red;
            if (tb_email.Text == "")
            {
                errorMsg.Visible = true;
                errorMsg.Text += "Email is required!" + "<br/>";
            }
            string emp = SelectByUsername(tb_email.Text);
            if (emp != null)
            {
                errorMsg.Visible = true;
                errorMsg.Text += "Email already exists!" + "<br/>";
            }
            if (String.IsNullOrEmpty(tb_firstname.Text))
            {
                errorMsg.Visible = true;
                errorMsg.Text += "First Name is required!" + "<br/>";
            }
            if (String.IsNullOrEmpty(tb_lastname.Text))
            {
                errorMsg.Visible = true;
                errorMsg.Text += "Last name is required!" + "<br/>";
            }
            if (String.IsNullOrEmpty(tb_creditcard.Text))
            {
                errorMsg.Visible = true;
                errorMsg.Text += "Credit Card is required!" + "<br/>";
            }
            if (String.IsNullOrEmpty(tb_dob.Text))
            {
                errorMsg.Visible = true;
                errorMsg.Text += "Date of Birth is required!" + "<br/>";
            }
            if (String.IsNullOrEmpty(tb_creditcard.Text))
            {
                errorMsg.Visible = true;
                errorMsg.Text += "Credit Card is required!" + "<br/>";
            }
            if (String.IsNullOrEmpty(tb_password.Text))
            {
                errorMsg.Visible = true;
                errorMsg.Text += "Password is required!" + "<br/>";
            }
            if (String.IsNullOrEmpty(tb_confirmpassword.Text))
            {
                errorMsg.Visible = true;
                errorMsg.Text += "Please confirm your password!" + "<br/>";
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

        public string SelectByUsername(string email)
        {
            //Step 1 -  Define a connection to the database by getting
            //          the connection string from App.config
            SqlConnection myConn = new SqlConnection(MYDBConnectionString);

            //Step 2 -  Create a DataAdapter to retrieve data from the database table
            string sqlStmt = "Select * from Account where Email=@paraEmail";
            SqlDataAdapter da = new SqlDataAdapter(sqlStmt, myConn);
            da.SelectCommand.Parameters.AddWithValue("@paraEmail", email);

            //Step 3 -  Create a DataSet to store the data to be retrieved
            DataSet ds = new DataSet();

            //Step 4 -  Use the DataAdapter to fill the DataSet with data retrieved
            da.Fill(ds);

            //Step 5 -  Read data from DataSet.
            string act = null;
            int rec_cnt = ds.Tables[0].Rows.Count;
            if (rec_cnt == 1)
            {
                DataRow row = ds.Tables[0].Rows[0];  // Sql command returns only one record
                string firstname = row["FirstName"].ToString();
                string lastname = row["LastName"].ToString();
                string creditcard = row["CreditCard"].ToString();
                string iv = row["IV"].ToString();
                string key = row["Key"].ToString();
                string dob = row["DateOfBirth"].ToString();
                int failedattemptcount = Convert.ToInt32(row["FailedAttemptCount"]);
                DateTime updatelogintime = Convert.ToDateTime(row["UpdateLoginTime"]);
                DateTime updatemaxpassword = Convert.ToDateTime(row["UpdateMaxPassword"]);
                DateTime updateminpassword = Convert.ToDateTime(row["UpdateMinPassword"]);
                string passwordhash = row["PasswordHash"].ToString();
                string passwordsalt = row["PasswordSalt"].ToString();
            }
            return act;
        }
    }
}