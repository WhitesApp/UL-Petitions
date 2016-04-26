using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace LabAttempt
{
    public partial class PetitionPage2 : System.Web.UI.Page
    {
        private int handoverPetID;
        public int signs;
        public int target;
        public int males;
        public int females;
        public DataTable commentsTable;
        public string timelineString;

        // PreRender ///////////////////////////////
        protected void Page_PreRender(object sender, EventArgs e)
        {
            // pack your ViewState right before the page is rendered which happens just before it is sent back to the browser
            ViewState["handoverPetID"] = handoverPetID;
            ViewState["target"] = target;
            ViewState["signs"] = signs;
            ViewState["male"] = males;
            ViewState["female"] = females;
            ViewState["commentsTable"] = commentsTable;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.Attributes.Add("style", "word-break:break-all; word-wrap:break-word");
            //if (Session["New"] != null)
            //{
            //    WelcomeLbl.Text = "Welcome " + Session["New"].ToString();
            //}
            //else
            //{
            //    Response.Redirect("UserLogIn.aspx");
            //}

            if (Request.Cookies["Useremail"] == null)
            {
                Response.Redirect("UserLogIn.aspx");
            }


            if (!IsPostBack)
            {
                handoverPetID = Convert.ToInt32(Request.QueryString["petitionID"]); // this requires the page to be loaded as a result of a redirect.
                //handoverPetID = 15; //use this to test the page without a redirect

                Petitions thisPetition = new Petitions(handoverPetID);
                lblSigns.Text = thisPetition.GetSigns().ToString();
                signs = thisPetition.GetSigns();
                target = thisPetition.GetTarget();
                lblTitle.Text = thisPetition.GetTitle();
                lblTarget.Text = thisPetition.GetTarget().ToString();
                lblReason.Text = thisPetition.GetReason().ToString();
                lblPetID.Text = handoverPetID.ToString();

                Users petitionAuthor = new Users(thisPetition.GetEmail());
                lblFirstname.Text = petitionAuthor.GetFirstname();
                lblSurname.Text = petitionAuthor.GetSurname();

                makeButtonSaySignOrUnsign();


                ////
                DisplayCharts();


                Image1.ImageUrl = thisPetition.GetFilename();
                //ScriptManager.RegisterStartupScript(
                //    UpdatePanelSigns,
                //    this.GetType(),
                //    "MyAction",
                //    "createChart(" + target + ", " + signs + ");",
                //    true);



                /////////////
                //Statistics petitionStatistics = new Statistics(handoverPetID);
                //int[] maleFemaleCounts = petitionStatistics.NoOfMalesAndFemales();
                //maleCount.Text = maleFemaleCounts[0].ToString();
                //femaleCount.Text = maleFemaleCounts[1].ToString();
                /////////////
                
                ///////////////
                //males = maleFemaleCounts[0];
                //females = maleFemaleCounts[1];

                //ScriptManager.RegisterStartupScript(
                //    UpdatePanelSigns,
                //    this.GetType(),
                //    "MyAction2",
                //    "createChart2(" + males + ", " + females + ");",
                //    true);
                //////////////

                getCommentsAndDataBindToGridView();

                Statistics petitionStatistics = new Statistics(handoverPetID);
                timelineString = string.Join(",", petitionStatistics.GetTimeline());

                //GridView2.DataSource = petitionStatistics.NoOfDifferentUserTypes();
                //GridView2.DataBind();
                //GridView3.DataSource = petitionStatistics.NoOfDifferentFaculties();
                //GridView3.DataBind();

                PanelStatistics.Visible = false;
                PanelComments.Visible = true;
                PanelTimeline.Visible = false;

            }

            if (IsPostBack)
            {
                target = Convert.ToInt32(ViewState["target"]);
                signs = Convert.ToInt32(ViewState["signs"]);
                handoverPetID = Convert.ToInt32(ViewState["handoverPetID"]);
                males = Convert.ToInt32(ViewState["male"]);
                females = Convert.ToInt32(ViewState["female"]);
                commentsTable = (DataTable)ViewState["commentsTable"];
            }
        }

        protected void makeButtonSaySignOrUnsign()
        {
            if (Petitions.IsPetitionComplete(handoverPetID))
            {
                btnSign.Text = "Completed";
                btnSign.BackColor = System.Drawing.Color.LimeGreen;
            }
            else if (SignPetition.HasEmailSignedPetition(Request.Cookies["Useremail"].Value, handoverPetID)) // this if-else determines whether the user in the cookie has signed the petition.
            {
                btnSign.Text = "Unsign";
                btnSign.BackColor = System.Drawing.Color.Crimson;
            }
            else
            {
                btnSign.Text = "Sign";
                btnSign.BackColor = System.Drawing.Color.PowderBlue;
            }
        }

        // Sign petition button ///////////////////////////////
        protected void btnSign_Click(object sender, EventArgs e)
        {
            if (Petitions.IsPetitionComplete(handoverPetID))
            {
                //Response.Redirect("PetitionPage2.aspx?petitionID=" + handoverPetID);
            }
            else if (!SignPetition.HasEmailSignedPetition(Request.Cookies["Useremail"].Value, handoverPetID))
            {
                // if not signed, sign it
                SignPetition.AddSignature(Request.Cookies["Useremail"].Value, handoverPetID);
                
                btnSign.BackColor = System.Drawing.Color.Orange;
            }
            else
            {
                // if already signed, remove signature from it
                SignPetition.RemoveSignature(Request.Cookies["Useremail"].Value, handoverPetID);
                btnSign.BackColor = System.Drawing.Color.Yellow;
            }


            //////////
            DisplayCharts();

            //Petitions thisPetition = new Petitions(handoverPetID);
            //lblSigns.Text = thisPetition.GetSigns().ToString();

            //makeButtonSaySignOrUnsign();

            //signs = thisPetition.GetSigns(); 

            //ScriptManager.RegisterStartupScript(
            //UpdatePanelSigns,
            //this.GetType(),
            //"MyAction",
            //"createChart(" + target + ", " + signs + ");",
            //true);


            /////////////
            //Statistics petitionStatistics = new Statistics(handoverPetID);
            //int[] maleFemaleCounts = petitionStatistics.NoOfMalesAndFemales();
            //maleCount.Text = maleFemaleCounts[0].ToString();
            //femaleCount.Text = maleFemaleCounts[1].ToString();
            /////////////

            ///////////////
            //males = maleFemaleCounts[0];
            //females = maleFemaleCounts[1];

            //ScriptManager.RegisterStartupScript(
            //    UpdatePanelSigns,
            //    this.GetType(),
            //    "MyAction2",
            //    "createChart2(" + males + ", " + females + ");",
            //    true);
            //////////////

            UpdatePanelSigns.Update();
            UpdatePanelCommentsAndStastics.Update();

        }

        // Add a comment button ///////////////////////////////
        protected void btnAddComment_Click(object sender, EventArgs e)
        {
            //handoverPetID = Convert.ToInt32(ViewState["handoverPetID"]);

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            SqlCommand commandSelect = new SqlCommand();
            SqlCommand commandInsert = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet aSet = new DataSet();

            string commentDetail = txtBoxAddComment.Text;
            int commentID;
            string commentIdQuery = "SELECT MAX (commentID) from [Comments]";
            string insertQuery;
            connection.Open();

            commandSelect = new SqlCommand(commentIdQuery, connection);
            adapter.SelectCommand = commandSelect;
            adapter.Fill(aSet, "Results");
            if (aSet.Tables["Results"].Rows.Count == 0)
            {
                commentID = 1;
            }
            else
            {
                commentID = Convert.ToInt32(aSet.Tables["Results"].Rows[0][0].ToString());
                commentID++;
            }

            insertQuery = "INSERT INTO [Comments] values ('" + commentID + "', '" + Request.Cookies["Useremail"].Value + "','" + handoverPetID + "', @commentDetail,GETDATE())";
            commandInsert = new SqlCommand(insertQuery, connection);
            commandInsert.Parameters.AddWithValue("@commentDetail", commentDetail);
            commandInsert.ExecuteNonQuery();

            connection.Close();
            
            getCommentsAndDataBindToGridView();

            txtBoxAddComment.Text = "";

            UpdatePanelComments.Update();

        }

        // Method to get comments relating to the petition in a dataset ///////////////////////////////
        protected void getCommentsAndDataBindToGridView()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            SqlCommand commandSelect = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet aSet = new DataSet();

            string getCommentInfoQuery = "select commentID, firstname, surname, commentdetail, comments.timestamp from comments, users where comments.email = users.email AND petitionID = " + handoverPetID + " order by timestamp desc";
            connection.Open();

            commandSelect = new SqlCommand(getCommentInfoQuery, connection);
            adapter.SelectCommand = commandSelect;
            adapter.Fill(aSet, "Results");

            connection.Close();

            //
            commentsTable = aSet.Tables["Results"].Copy();
            aSet.Tables["Results"].Columns.Remove("commentID");
            GridView1.DataSource = aSet.Tables["Results"];
            GridView1.DataBind();

            /////////////////////////////////////////////////////////////
            //
            //return aSet;
        }

        // Manages paging for comments ///////////////////////////////
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;

            getCommentsAndDataBindToGridView();

            UpdatePanelComments.Update();
        }


        protected void DisplayCharts()
        {
            
            Petitions thisPetition = new Petitions(handoverPetID);
            lblSigns.Text = thisPetition.GetSigns().ToString();

            makeButtonSaySignOrUnsign();

            signs = thisPetition.GetSigns(); 

            ScriptManager.RegisterStartupScript(
            UpdatePanelSigns,
            this.GetType(),
            "MyAction",
            "createChart(" + target + ", " + signs + ");",
            true);


            ///////////
            Statistics petitionStatistics = new Statistics(handoverPetID);
            int[] maleFemaleCounts = petitionStatistics.NoOfMalesAndFemales();
            maleCount.Text = maleFemaleCounts[0].ToString();
            femaleCount.Text = maleFemaleCounts[1].ToString();
 
            males = maleFemaleCounts[0];
            females = maleFemaleCounts[1];

            ScriptManager.RegisterStartupScript(
                UpdatePanelSigns,
                this.GetType(),
                "MyAction2",
                "createChart2(" + males + ", " + females + ");",
                true);
            ////////////

            DataTable userTypesStats = petitionStatistics.NoOfDifferentUserTypes();

            //string temp = userTypesStats.Rows[0][1].ToString();
            //string temp2 = userTypesStats.Rows[1][1].ToString();
            //string temp3 = userTypesStats.Rows[2][1].ToString();
            //string temp4 = userTypesStats.Rows[3][1].ToString();
            //string temp5 = userTypesStats.Rows[4][1].ToString();
            //string temp6 = userTypesStats.Rows[5][1].ToString();
            //string temp7 = userTypesStats.Rows[6][1].ToString();

            ScriptManager.RegisterStartupScript(
                UpdatePanelSigns,
                this.GetType(),
                "MyAction3",
                "createChart3(" + userTypesStats.Rows[0][1] + ", " + userTypesStats.Rows[1][1] + ", " + userTypesStats.Rows[2][1] + ", " + userTypesStats.Rows[3][1] + ", " + userTypesStats.Rows[4][1] + ", " + userTypesStats.Rows[5][1] + ", " + userTypesStats.Rows[6][1] + ");",
                true);

            DataTable facultyStats = petitionStatistics.NoOfDifferentFaculties();

            ScriptManager.RegisterStartupScript(
                UpdatePanelSigns,
                this.GetType(),
                "MyAction4",
                "createChart4(" + facultyStats.Rows[0][1] + ", " + facultyStats.Rows[1][1] + ", " + facultyStats.Rows[2][1] + ", " + facultyStats.Rows[3][1] + ", " + facultyStats.Rows[4][1] + ");",
                true);

            
            timelineString = string.Join(",", petitionStatistics.GetTimeline());

            //ScriptManager.RegisterStartupScript(
            //    UpdatePanelSigns,
            //    this.GetType(),
            //    "MyAction5",
            //    "drawChart()",
            //    true);
        }

        protected void DisplayStatsCharts()
        {
            Statistics petitionStatistics = new Statistics(handoverPetID);
            int[] maleFemaleCounts = petitionStatistics.NoOfMalesAndFemales();
            maleCount.Text = maleFemaleCounts[0].ToString();
            femaleCount.Text = maleFemaleCounts[1].ToString();

            males = maleFemaleCounts[0];
            females = maleFemaleCounts[1];

            ScriptManager.RegisterStartupScript(
                UpdatePanelCommentsAndStastics,
                this.GetType(),
                "MyAction2",
                "createChart2(" + males + ", " + females + ");",
                true);
            ////////////

            DataTable userTypesStats = petitionStatistics.NoOfDifferentUserTypes();

            //string temp = userTypesStats.Rows[0][1].ToString();
            //string temp2 = userTypesStats.Rows[1][1].ToString();
            //string temp3 = userTypesStats.Rows[2][1].ToString();
            //string temp4 = userTypesStats.Rows[3][1].ToString();
            //string temp5 = userTypesStats.Rows[4][1].ToString();
            //string temp6 = userTypesStats.Rows[5][1].ToString();
            //string temp7 = userTypesStats.Rows[6][1].ToString();

            ScriptManager.RegisterStartupScript(
                UpdatePanelCommentsAndStastics,
                this.GetType(),
                "MyAction3",
                "createChart3(" + userTypesStats.Rows[0][1] + ", " + userTypesStats.Rows[1][1] + ", " + userTypesStats.Rows[2][1] + ", " + userTypesStats.Rows[3][1] + ", " + userTypesStats.Rows[4][1] + ", " + userTypesStats.Rows[5][1] + ", " + userTypesStats.Rows[6][1] + ");",
                true);

            DataTable facultyStats = petitionStatistics.NoOfDifferentFaculties();

            ScriptManager.RegisterStartupScript(
                UpdatePanelCommentsAndStastics,
                this.GetType(),
                "MyAction4",
                "createChart4(" + facultyStats.Rows[0][1] + ", " + facultyStats.Rows[1][1] + ", " + facultyStats.Rows[2][1] + ", " + facultyStats.Rows[3][1] + ", " + facultyStats.Rows[4][1] + ");",
                true);

            timelineString = string.Join(",", petitionStatistics.GetTimeline());

            ScriptManager.RegisterStartupScript(
                UpdatePanelCommentsAndStastics,
                this.GetType(),
                "MyAction5",
                "drawChart()",
                true);
        }

        protected void DisplayTimeline()
        {
            Statistics petitionStatistics = new Statistics(handoverPetID);
            timelineString = string.Join(",", petitionStatistics.GetTimeline());

            ScriptManager.RegisterStartupScript(
                UpdatePanelCommentsAndStastics,
                this.GetType(),
                "MyAction5",
                "drawChart()",
                true);
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GridViewRow FlaggedRow = GridView1.SelectedRow;
            //string FlaggedCommentID = FlaggedRow.Cells[1].Text;
            //Response.Redirect("ConfirmFlaggedComment.aspx?FlaggedCommentID=" + FlaggedCommentID);
            
            //GridView1.SelectedIndex;
            int FlaggedCommentID = Convert.ToInt32(commentsTable.Rows[GridView1.SelectedRow.RowIndex][0]);
            Response.Redirect("ConfirmFlaggedComment.aspx?FlaggedCommentID=" + FlaggedCommentID);
        }

        //protected void DeleteComment_Click(Object sender, EventArgs e)
        //{
        //    SqlConnection UsersConnection = new SqlConnection();
        //    UsersConnection.ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
        //    UsersConnection.Open();

        //    string checkUserExists = "Select count(*) from Users where Email = '" + Session["New"].ToString() + "'";
        //    SqlCommand AdministratorCommand = new SqlCommand(checkUserExists, UsersConnection);

        //    int temp = Convert.ToInt32(AdministratorCommand.ExecuteScalar().ToString());
        //    UsersConnection.Close();

        //    if (temp == 1)
        //    {
        //        GridView1.DeleteRow(GridView1.SelectedIndex);
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "popup", "alert('Comment deleted');", true);
        //    } 
        //}


        protected void FlagPetitionBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("ConfirmFlaggedPetition.aspx?FlaggedPetitionsID=" + handoverPetID);
        }

        protected void ShowStats_Click(object sender, EventArgs e)
        {
            PanelStatistics.Visible = true;
            PanelComments.Visible = false;
            PanelTimeline.Visible = false;
            DisplayStatsCharts();
            UpdatePanelCommentsAndStastics.Update();
            ShowComments.BackColor = System.Drawing.Color.White;
            ShowStats.BackColor = System.Drawing.Color.Gainsboro;
            ShowTimeline.BackColor = System.Drawing.Color.White;
        }

        protected void ShowComments_Click(object sender, EventArgs e)
        {
            PanelStatistics.Visible = false;
            PanelComments.Visible = true;
            PanelTimeline.Visible = false;
            //DisplayCharts();
            getCommentsAndDataBindToGridView();
            UpdatePanelCommentsAndStastics.Update();
            ShowComments.BackColor = System.Drawing.Color.Gainsboro;
            ShowStats.BackColor = System.Drawing.Color.White;
            ShowTimeline.BackColor = System.Drawing.Color.White;
        }

        protected void ShowTimeline_Click(object sender, EventArgs e)
        {
            PanelStatistics.Visible = false;
            PanelComments.Visible = false;
            PanelTimeline.Visible = true;
            DisplayStatsCharts();
            //UpdatePanelCommentsAndStastics.Update();
            ShowComments.BackColor = System.Drawing.Color.White;
            ShowStats.BackColor = System.Drawing.Color.White;
            ShowTimeline.BackColor = System.Drawing.Color.Gainsboro;
        }

    }
}

