using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace LabAttempt
{
    public class Users
    {
        private string email;
        private string password;
        private int gender;
        private string firstname;
        private string surname;
        private string timestamp;
        private int userTypeCode;
        private int facultyCode;
        private string filename;

        public Users(string email)
        {
            this.getInfoFromUsers(email);
        }

        private void getInfoFromUsers(string email)
        {
            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet aSet = new DataSet();
            string aConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            connection.ConnectionString = aConnectionString;
            string aQuery = "SELECT * FROM [users] where email = '" + email + "'";
            command = new SqlCommand(aQuery, connection);
            adapter.SelectCommand = command;
            connection.Open();
            adapter.Fill(aSet, "Results");
            connection.Close();
            if (aSet.Tables["Results"].Rows.Count == 0)
            {
                this.email = "noemail";
            }
            else 
            {
                this.email = aSet.Tables["Results"].Rows[0][0].ToString();
                this.password = aSet.Tables["Results"].Rows[0][1].ToString();
                this.gender = Convert.ToInt32(aSet.Tables["Results"].Rows[0][2]);
                this.firstname = aSet.Tables["Results"].Rows[0][3].ToString();
                this.surname = aSet.Tables["Results"].Rows[0][4].ToString();
                this.timestamp = aSet.Tables["Results"].Rows[0][5].ToString();
                this.userTypeCode = Convert.ToInt32(aSet.Tables["Results"].Rows[0][6]);
                this.facultyCode = Convert.ToInt32(aSet.Tables["Results"].Rows[0][7]);
                this.filename = aSet.Tables["Results"].Rows[0][8].ToString();
            }

        }

        public Boolean SignedThisPetition(int petID)
        {
            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet aSet = new DataSet();
            string aConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            connection.ConnectionString = aConnectionString;
            string aQuery = "SELECT * FROM [signs] where email = '" + this.email + "' AND petitionID = '" + petID + "'";
            command = new SqlCommand(aQuery, connection);
            adapter.SelectCommand = command;
            connection.Open();
            adapter.Fill(aSet, "Results");
            connection.Close();
            if (aSet.Tables["Results"].Rows.Count == 0)
            {
                return false; // no row found, therefore 
            }
            else
            {
                return true;
            }
        }

        public string GetEmail()
        {
            return this.email;
        }

        public string GetPassword()
        {
            return this.password;
        }

        public int GetGender()
        {
            return this.gender;
        }

        public string GetFirstname()
        {
            return this.firstname;
        }

        public string GetSurname()
        {
            return this.surname;
        }

        public string GetTimestamp()
        {
            return this.timestamp;
        }

        public int GetUserTypeCode()
        {
            return this.userTypeCode;
        }

        public int GetFacultyCode()
        {
            return this.facultyCode;
        }

        public string GetFilename()
        {
            return this.filename;
        }

    }
}