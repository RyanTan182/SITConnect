using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class HomePage : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", true);
                }
                else
                {   
                    lblMsg.Text = "Congratulations!, you are logged in.";
                    lblMsg.ForeColor = System.Drawing.Color.Green;
                    btn_logout.Visible = true;
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }
        protected void LogoutMe(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        protected void EditPassword(object sender, EventArgs e)
        {

            string userid = Session["SSEmail"].ToString();
            DateTime minpassword = Convert.ToDateTime(getminPassword(userid));
            DateTime maxpassword = Convert.ToDateTime(getmaxPassword(userid));
            if (DateTime.Now < minpassword)
            {
                lblMsg.Text = "Unable to change password! Please wait and try again later!";
                lblMsg.ForeColor = System.Drawing.Color.Red;
                btn_logout.Visible = true;
            }
            else
            {
                Response.Redirect("ChangePassword.aspx");
            }
        }

        protected int updateminpassword(string email, DateTime updateminpassword)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account SET UpdateMinPassword=@paraUpdateMinPassword WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            command.Parameters.AddWithValue("@paraUpdateMinPassword", updateminpassword);
            connection.Open();
            command.CommandText = sql;
            command.Connection = connection;
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        protected string getminPassword(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select UpdateMinPassword FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["UpdateMinPassword"] != null)
                        {
                            if (reader["UpdateMinPassword"] != DBNull.Value)
                            {
                                s = reader["UpdateMinPassword"].ToString();
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

        protected int updatemaxpassword(string email, DateTime updatemaxpassword)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account SET UpdateMaxPassword=@paraUpdateMaxPassword WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            command.Parameters.AddWithValue("@paraUpdateMaxPassword", updatemaxpassword);
            connection.Open();
            command.CommandText = sql;
            command.Connection = connection;
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        protected string getmaxPassword(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select UpdateMaxPassword FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["UpdateMaxPassword"] != null)
                        {
                            if (reader["UpdateMaxPassword"] != DBNull.Value)
                            {
                                s = reader["UpdateMaxPassword"].ToString();
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
    }
}