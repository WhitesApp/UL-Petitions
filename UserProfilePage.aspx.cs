 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace LabAttempt
{
    public partial class UserProfilePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Cookies["Useremail"] == null)
            {
                Response.Redirect("UserLogIn.aspx");
            }

            ////////
            //Response.Redirect("UserProfilePage.aspx?handoverEmail=" ++ ");
            ////////
            //string handoverEmail = "lady@email.com"; 



            string handoverEmail = Request.QueryString["handoverEmail"].ToString();
            Users thisUser = new Users(handoverEmail);

            lblName.Text = thisUser.GetFirstname() + " " + thisUser.GetSurname();

            Image1.ImageUrl = thisUser.GetFilename(); 

            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet aSet = new DataSet();
            string aConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            connection.ConnectionString = aConnectionString;
            string aQuery = "SELECT FacultyName FROM [Faculties] where FacultyCode = '" + thisUser.GetFacultyCode() + "'";
            command = new SqlCommand(aQuery, connection);
            adapter.SelectCommand = command;
            connection.Open();
            adapter.Fill(aSet, "Results");
            connection.Close();

            lblFaculty.Text = aSet.Tables["Results"].Rows[0][0].ToString();


            aQuery = "SELECT UserTypeName FROM [UserTypes] where UserTypeCode = '" + thisUser.GetUserTypeCode() + "'";
            command = new SqlCommand(aQuery, connection);
            adapter.SelectCommand = command;
            connection.Open();
            adapter.Fill(aSet, "Results2");
            connection.Close();

            lblType.Text = aSet.Tables["Results2"].Rows[0][0].ToString();


            aQuery = "SELECT Count(*) FROM [Signs] where Email = '" + handoverEmail + "'";
            command = new SqlCommand(aQuery, connection);
            adapter.SelectCommand = command;
            connection.Open();
            adapter.Fill(aSet, "Results3");
            connection.Close();

            if (aSet.Tables["Results3"].Rows.Count == 0)
            {
                lblNoOfSigns.Text = "0 petitions signed.";
            }
            else
            {
                lblNoOfSigns.Text = aSet.Tables["Results3"].Rows[0][0].ToString() + " petitions signed.";
            }

            aQuery = "SELECT Count(*) FROM [Petitions] where Email = '" + handoverEmail + "'";
            command = new SqlCommand(aQuery, connection);
            adapter.SelectCommand = command;
            connection.Open();
            adapter.Fill(aSet, "Results4");
            connection.Close();

            if (aSet.Tables["Results4"].Rows.Count == 0)
            {
                lblNoOfPetitionsCreated.Text = "0 petitions created.";
            }
            else
            {
                lblNoOfPetitionsCreated.Text = aSet.Tables["Results4"].Rows[0][0].ToString() + " petitions created.";
            }

            lblJoined.Text = "Joined " + thisUser.GetTimestamp();
        }
    }
}