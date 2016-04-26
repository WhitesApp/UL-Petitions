using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace LabAttempt
{
    public partial class SearchResultPage4 : System.Web.UI.Page
    {
        private ArrayList relevantPetitions = new ArrayList();
        private string searchTerm;
        private DataTable relevantPetitionDataTable;
        private int advancedSearch;
        private string theQuery;
        private int searchPageNumber;
        private int numberOfResultsPerPage;



        protected void Page_Load(object sender, EventArgs e)
        {
            numberOfResultsPerPage = 10;

            if (Request.Cookies["Useremail"] == null)
            {
                Response.Redirect("UserLogIn.aspx");
            }

            if (IsPostBack)
            {
                searchPageNumber = (int)ViewState["searchPageNumber"];
                theQuery = (string)ViewState["theQuery"];
                searchTerm = (string)ViewState["searchTerm"];
                advancedSearch = (int)ViewState["advancedSearch"];
                relevantPetitionDataTable = (DataTable)ViewState["relevantPetitionDataTable"];
                
            }
            
            if (String.IsNullOrEmpty(Request.QueryString["searchTerm"]) && String.IsNullOrEmpty(Request.QueryString["advancedSearch"]))
            {
                lblResult.Text = "Please enter a search.";
                //lblDisplaySearchTerm.Visible = false;
            }
            else
            {
                //lblDisplaySearchTerm.Visible = true;

                if (!IsPostBack)
                {
                    searchTerm = Request.QueryString["searchTerm"];
                    advancedSearch = Convert.ToInt32(Request.QueryString["advancedSearch"]);
                    searchPageNumber = 0;

                    if (advancedSearch == 1)
                    {


                        string categoryCodeSQL;
                        string userTypeCodeSQL;
                        string facultyCodeSQL;
                        string reachedTargetSQL;
                        int categoryCode = Convert.ToInt32(Request.QueryString["categoryCode"]);
                        int userTypeCode = Convert.ToInt32(Request.QueryString["userTypeCode"]);
                        int facultyCode = Convert.ToInt32(Request.QueryString["facultyCode"]);
                        int reachedTarget = Convert.ToInt32(Request.QueryString["reachedTarget"]);

                        if (String.IsNullOrEmpty(searchTerm))
                        {
                            searchTerm = "";
                        }

                        if (categoryCode == -1 || categoryCode == 0)
                        {
                            categoryCodeSQL = "";
                        }
                        else
                        {
                            categoryCodeSQL = "AND categorycode = '" + categoryCode + "'";
                        }

                        if (userTypeCode == -1 || userTypeCode == 0)
                        {
                            userTypeCodeSQL = "";
                        }
                        else
                        {
                            userTypeCodeSQL = "AND usertypecode = '" + userTypeCode + "'";
                        }

                        if (facultyCode == -1 || facultyCode == 0)
                        {
                            facultyCodeSQL = "";
                        }
                        else
                        {
                            facultyCodeSQL = "AND facultycode = '" + facultyCode + "'";
                        }

                        if (reachedTarget == 1)
                        {
                            reachedTargetSQL = "AND petitionID IN (select petitionID from [signs] group by petitionID having count(*) >= (select target from [petitions] where signs.petitionID = petitions.petitionID))";

                        }
                        else
                        {
                            reachedTargetSQL = "";
                        }

                        theQuery = "SELECT petitionID FROM [Petitions], [Users] WHERE petitions.email = users.email AND (reason LIKE @searchTerm OR title LIKE @searchTermTwo) " + categoryCodeSQL + facultyCodeSQL + userTypeCodeSQL + reachedTargetSQL + " ORDER BY Petitions.timestamp DESC";
                    }

                    else // NOT advanced search
                    {
                        if (String.IsNullOrEmpty(searchTerm))
                        {
                            searchTerm = "";
                        }
                        theQuery = "SELECT petitionID FROM [Petitions], [Users] WHERE petitions.email = users.email AND (reason LIKE @searchTerm OR title LIKE @searchTermTwo) ORDER BY Petitions.timestamp DESC";
                    }

                    GetAndDisplaySimpleSearchResults();
                }
            }
        }



        protected void Page_PreRender(object sender, EventArgs e)
        {

            // pack your ViewState right before the page is rendered which happens just before it is sent back to the browser
            ViewState["relevantPetitionDataTable"] = relevantPetitionDataTable;
            ViewState["searchTerm"] = searchTerm;
            ViewState["advancedSearch"] = advancedSearch;
            ViewState["theQuery"] = theQuery;
            ViewState["searchPageNumber"] = searchPageNumber;
            ViewState["numberOfResultsPerPage"] = numberOfResultsPerPage;
        }

        protected void searchButton_Click(object sender, EventArgs e)
        {
            //// deal with number per page//////////////////////////////////////////////////////////////////////////////
            searchTerm = searchBox.Text;


            advancedSearch = 0;
            searchPageNumber = 0;

            if (String.IsNullOrEmpty(searchTerm))
            {
                searchTerm = "";
            }

            theQuery = "SELECT petitionID FROM [Petitions], [Users] WHERE petitions.email = users.email AND (reason LIKE @searchTerm OR title LIKE @searchTermTwo) ORDER BY Petitions.timestamp DESC";

            GetAndDisplaySimpleSearchResults();
        }

        protected void repeaterControl_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SignButton")
            {
                string email = Request.Cookies["Useremail"].Value;
                int eItemIndex = e.Item.ItemIndex;
                if (Petitions.IsPetitionComplete(Convert.ToInt32(relevantPetitionDataTable.Rows[eItemIndex][0])))
                {
                    Response.Redirect("PetitionPage2.aspx?petitionID=" + relevantPetitionDataTable.Rows[eItemIndex][0]);
                }
                else if (!SignPetition.HasEmailSignedPetition(email, Convert.ToInt32(relevantPetitionDataTable.Rows[eItemIndex][0])))
                {
                    // if not signed, sign it
                    SignPetition.AddSignature(email, Convert.ToInt32(relevantPetitionDataTable.Rows[eItemIndex][0]));
                    //////
                    relevantPetitionDataTable.Rows[eItemIndex]["Signs"] = (Convert.ToInt32(relevantPetitionDataTable.Rows[eItemIndex]["signs"]) + 1);
                    //relevantPetitionDataTable.Rows[eItemIndex]["SignedOrNotSigned"] = "Unsign";
                    //////
                    if (Petitions.IsPetitionComplete(Convert.ToInt32(relevantPetitionDataTable.Rows[eItemIndex][0])))
                    {
                        relevantPetitionDataTable.Rows[eItemIndex]["SignedOrNotSigned"] = "Completed";
                    }
                    else
                    {
                        relevantPetitionDataTable.Rows[eItemIndex]["SignedOrNotSigned"] = "Unsign";
                    }
                }
                else
                {
                    // if already signed, remove signature from it
                    SignPetition.RemoveSignature(email, Convert.ToInt32(relevantPetitionDataTable.Rows[eItemIndex][0]));
                    ////// This deals with changing the signs count and sign button WITHOUT reloading all the petitions in the repeater. This means that if something changes popularity, the user would notice until reload.
                    relevantPetitionDataTable.Rows[eItemIndex]["Signs"] = (Convert.ToInt32(relevantPetitionDataTable.Rows[eItemIndex]["signs"]) - 1);
                    relevantPetitionDataTable.Rows[eItemIndex]["SignedOrNotSigned"] = "Sign";
                    //////
                }

                //GetAndDisplaySimpleSearchResults();
                repeaterControl.DataSource = relevantPetitionDataTable;
                repeaterControl.DataBind();
                UpdatePanel1.Update();
            }
            else
            {
                string redirectString = "";
                redirectString = Repeater.EnablePetitionRepeaterControls(relevantPetitionDataTable, e.CommandName, e.Item.ItemIndex, Request.Cookies["Useremail"].Value);
                Response.Redirect(redirectString);
            }

        }

        protected void lnkbtnNext_Click(object sender, EventArgs e)
        {
            searchPageNumber++;
            GetAndDisplaySimpleSearchResults();
        }

        protected void lnkbtnPrevious_Click(object sender, EventArgs e)
        {
            searchPageNumber--;
            GetAndDisplaySimpleSearchResults();
        }

        protected void GetAndDisplaySimpleSearchResults()
        {

            DataSet aSet = Repeater.GetRepeaterInfo(searchTerm, theQuery, Request.Cookies["Useremail"].Value, searchPageNumber, numberOfResultsPerPage);

            relevantPetitionDataTable = aSet.Tables["AllPetitionInfo"];

            if (aSet.Tables["Results"].Rows.Count == 0 || (searchTerm == "" && advancedSearch == 0))
            {
                lblResult.Text = "Sorry, could not find any petition matching these criteria.";
                //lblDisplaySearchTerm.Visible = false;
                lnkbtnPrevious.Visible = false;
                lnkbtnNext.Visible = false;

                DataTable dummie = new DataTable();
                dummie.Columns.Add(new DataColumn("PetitionID", typeof(string)));
                dummie.Columns.Add(new DataColumn("Title", typeof(string)));
                dummie.Columns.Add(new DataColumn("Reason", typeof(string)));
                dummie.Columns.Add(new DataColumn("Signs", typeof(string)));
                dummie.Columns.Add(new DataColumn("Target", typeof(string)));
                dummie.Columns.Add(new DataColumn("Creator", typeof(string)));
                dummie.Columns.Add(new DataColumn("SignedOrNotSigned", typeof(string)));
                dummie.Columns.Add(new DataColumn("Filename", typeof(string)));
                dummie.Columns.Add(new DataColumn("Email", typeof(string)));

                repeaterControl.DataSource = dummie;
                repeaterControl.DataBind();
            }

            else // ie, there were relevant petitions...
            {
                lblResult.Text = "";
                
                relevantPetitions = Repeater.GetRelevantPetitionsAsArrayList(aSet);

                repeaterControl.DataSource = aSet.Tables["AllPetitionInfo"];
                repeaterControl.DataBind();

                if (searchPageNumber == 0)
                {
                    lnkbtnPrevious.Visible = false;
                }
                else
                {
                    lnkbtnPrevious.Visible = true;
                }

                if (aSet.Tables["Results"].Rows.Count > ((searchPageNumber + 1) * numberOfResultsPerPage))
                {
                    lnkbtnNext.Visible = true;
                }
                else
                {
                    lnkbtnNext.Visible = false;
                }
            }
            //lblDisplaySearchTerm.Text = searchTerm;
        }



    }
}