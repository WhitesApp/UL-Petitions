using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


namespace LabAttempt
{
    public class Petitions
    {
        private int petitionID;
        private string email;
        private string title;
        private int categoryCode;
        private string reason;
        private string filename;
        private string timestamp;
        private int target;

        private int signs;

        public Petitions(int petID)
        {
            this.GetInfoFromPetitions(petID);
            this.GetInfoFromSigns(petID);

        }

        private void GetInfoFromPetitions(int petID)
        {
            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet aSet = new DataSet();
            string aConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            string theQuery = "SELECT * FROM [petitions] WHERE petitionID = '" + petID + "'";
            connection.ConnectionString = aConnectionString;
            command = new SqlCommand(theQuery, connection);
            adapter.SelectCommand = command;
            connection.Open();
            adapter.Fill(aSet, "Results");

            connection.Close();
            if (aSet.Tables["Results"].Rows.Count == 0) 
            {
                this.petitionID = -1; // makes petitionID = -1 if there is no petition found.
            }
            else
            {
                this.petitionID = Convert.ToInt32(aSet.Tables["Results"].Rows[0][0]);
                this.email = aSet.Tables["Results"].Rows[0][1].ToString();
                this.title = aSet.Tables["Results"].Rows[0][2].ToString();
                this.categoryCode = Convert.ToInt32(aSet.Tables["Results"].Rows[0][3]);
                this.reason = aSet.Tables["Results"].Rows[0][4].ToString();
                this.filename = aSet.Tables["Results"].Rows[0][5].ToString();
                this.timestamp = aSet.Tables["Results"].Rows[0][6].ToString();
                this.target = Convert.ToInt32(aSet.Tables["Results"].Rows[0][7]);
            }
        }

        private void GetInfoFromSigns(int petID)
        {
            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet aSet = new DataSet();
            string aConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            string noOfSigns = "SELECT COUNT (*) FROM [signs] WHERE petitionID = '" + petID + "'";
            connection.ConnectionString = aConnectionString;
            command = new SqlCommand(noOfSigns, connection);
            adapter.SelectCommand = command;
            connection.Open();
            adapter.Fill(aSet, "Results");
            this.signs = Convert.ToInt32(aSet.Tables["Results"].Rows[0][0]);
            connection.Close();
        }

        public static int InsertPetition(string email, string title, int categoryCode, string reason, string filename, int target)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            SqlCommand commandSelect = new SqlCommand();
            SqlCommand commandInsert = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet aSet = new DataSet();

            int petitionID;
            string petitionIdQuery = "SELECT MAX (petitionID) from [Petitions]";
            string insertQuery;
            connection.Open();

            commandSelect = new SqlCommand(petitionIdQuery, connection);
            adapter.SelectCommand = commandSelect;
            adapter.Fill(aSet, "Results");
            petitionID = Convert.ToInt32(aSet.Tables["Results"].Rows[0][0].ToString());
            petitionID++;

            insertQuery = "INSERT INTO [Petitions] values ('" + petitionID + "', '" + email + "','" + title + "', '" + categoryCode + "', '" + reason + "','" + filename + "',GETDATE(), '" + target + "')";
            commandInsert = new SqlCommand(insertQuery, connection);
            commandInsert.ExecuteNonQuery();

            connection.Close();

            
            return petitionID; // returns petitionID of the new petition
        }

        public static bool IsPetitionComplete(int petitionID)
        {
            Petitions thisPetition = new Petitions(petitionID);
            if (thisPetition.GetSigns() >= thisPetition.GetTarget())
            {
                return true;
            }
            else return false;
        }

        public int GetPetitionID()
        {
            return this.petitionID;
        }

        public string GetEmail()
        {
            return this.email;
        }

        public string GetTitle()
        {
            return this.title;
        }

        public int GetCategoryCode()
        {
            return this.categoryCode;
        }
        public string GetReason()
        {
            return this.reason;
        }
        public string GetFilename()
        {
            return this.filename;
        }

        public string GetTimestamp()
        {
            return timestamp;
        }

        public int GetTarget()
        {
            return target;
        }

        public int GetSigns()
        {
            return signs;
        }



    }
}