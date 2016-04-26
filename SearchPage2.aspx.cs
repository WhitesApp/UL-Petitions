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
    public partial class SearchPage2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Cookies["Useremail"] == null)
            {
                Response.Redirect("UserLogIn.aspx");
            }
        }

        protected void btnAdvancedSearch_Click(object sender, EventArgs e)
        {
            
            // basic functionality of this page working. Needs to be filled up massively. 
            // to test this, type "Some" into the search field and leave all the other dropdowns on their default. Should return petition id 5. 

            string searchTerm = txtSearchTerm.Text;
            string categoryCode = ddlCategory.SelectedValue;
            string userTypeCode =ddlUserType.SelectedValue;
            string facultyCode = ddlFaculty.SelectedValue;
            string reachedTarget;

            // removed if reverting
            if (checkboxTargetReached.Checked == true)
            {
                reachedTarget = "1";

            }
            else
            {
                reachedTarget = "0";
            }
            //

            Response.Redirect("SearchResultPage4.aspx?searchTerm=" + searchTerm + "&categoryCode=" + categoryCode + "&userTypeCode=" + userTypeCode + "&facultyCode=" + facultyCode + "&reachedTarget=" + reachedTarget + "&advancedSearch=" + 1);
            // this may need to be changed back to 2

        }
    }
}