using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace LabAttempt
{
    public class FeaturedPetitions
    {

        public static Boolean IsPetitionFeatured(int petID)
        {
            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet aSet = new DataSet();
            string aConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            string theQuery = "SELECT * FROM [FeaturedPetitions] WHERE petitionID = '" + petID + "'";
            connection.ConnectionString = aConnectionString;
            command = new SqlCommand(theQuery, connection);
            adapter.SelectCommand = command;
            connection.Open();
            adapter.Fill(aSet, "Results");
            connection.Close();
            if (aSet.Tables["Results"].Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Boolean AddFeatured(int petitionID)
        {
            if (!IsPetitionFeatured(petitionID))
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
                SqlCommand commandInsert = new SqlCommand();
                connection.Open();
                string insertQuery = "INSERT INTO [FeaturedPetitions] values ('" + petitionID + "',GETDATE())";
                commandInsert = new SqlCommand(insertQuery, connection);
                commandInsert.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            else
            {

                return false;
            }
        }

        public static Boolean RemoveFeatured(int petitionID)
        {
            if (IsPetitionFeatured(petitionID))
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
                SqlCommand commandDelete = new SqlCommand();
                connection.Open();
                string deleteQuery = "DELETE FROM [FeaturedPetitions] WHERE petitionID ='" + petitionID + "'";
                commandDelete = new SqlCommand(deleteQuery, connection);
                commandDelete.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}