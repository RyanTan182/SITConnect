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
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker.Text = "Status : " + status;
            if (scores < 4)
            {
                lbl_pwdchecker.ForeColor = Color.Red;
                return;
            }
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
    }
}