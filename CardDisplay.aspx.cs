using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LabAttempt
{
    public partial class CardDisplay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // TEMPORARY Cookie
            Response.Cookies["Useremail"].Value = "john@email.com";
            //
            int petitionID = 2; // TEMP setting petitionID
            //

            Petitions thisPetition = new Petitions(petitionID);
            if (thisPetition.GetPetitionID() == -1)
            {
                Response.Write("No petition found");
            }
            else
            {
                lblTitle.Text = thisPetition.GetTitle(); // Petition Title
                lblSigns.Text = thisPetition.GetSigns().ToString(); // No. of signatures the petition has gotten
                lblTimestamp.Text = thisPetition.GetTimestamp();
                lblTarget.Text = thisPetition.GetTarget().ToString();
                lblReason.Text = thisPetition.GetReason();

                Users thisUser = new Users(thisPetition.GetEmail());
                if (thisUser.GetEmail().Equals("noemail"))
                {
                    Response.Write("No user found");
                }
                else
                {
                    lblFirstname.Text = thisUser.GetFirstname(); // Author firstname
                    lblSurname.Text = thisUser.GetSurname(); // Author surname
                }
                
                if (SignPetition.HasEmailSignedPetition(Request.Cookies["Useremail"].Value, petitionID)) // this if-else determines whether the user in the cookie has signed the petition.
                {
                    btnSign.Text = "Unsign";
                }
                else btnSign.Text = "Sign";
            }


            

        }

        protected void btnSign_Click(object sender, EventArgs e)
        {
            Response.Write(SignPetition.AddSignature(Request.Cookies["Useremail"].Value, 2));
            // SignPetition.AddSignature() signs the petition using the user store in the cookie.
            // SignPetition.AddSignature() checks if they have already signed it and returns false if they have.
            // if they have not already signed it, it signs the petition and returns true. 
        }
    }
}