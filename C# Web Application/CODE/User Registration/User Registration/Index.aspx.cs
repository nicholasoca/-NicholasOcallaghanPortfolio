using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace User_Registration
{
    public partial class Index : Page
    {
        // Your connection string
        string connectionString = @"Server=NICHOLASDESKTOP\SQLEXPRESS;Database=UserRegistrationDB;Integrated Security=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Clear();
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    int userID = Convert.ToInt32(Request.QueryString["id"]);
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        sqlCon.Open();
                        SqlDataAdapter sqlDa = new SqlDataAdapter("UserViewByID", sqlCon);
                        sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlDa.SelectCommand.Parameters.AddWithValue("@UserID", userID);
                        DataTable dtbl = new DataTable();
                        sqlDa.Fill(dtbl);

                        hfUserID.Value = userID.ToString();
                        txtFirstName.Text = dtbl.Rows[0]["FirstName"].ToString();
                        txtLastName.Text = dtbl.Rows[0]["LastName"].ToString();
                        txtContact.Text = dtbl.Rows[0]["Contact"].ToString();
                        ddlGender.Items.FindByValue(dtbl.Rows[0]["Gender"].ToString()).Selected = true;
                        txtAddress.Text = dtbl.Rows[0]["Address"].ToString();
                        txtUsername.Text = dtbl.Rows[0]["Username"].ToString();
                        txtPassword.Text = dtbl.Rows[0]["Password"].ToString();
                        txtPassword.Attributes.Add("value", dtbl.Rows[0]["Password"].ToString());
                        txtConfirmPassword.Text = dtbl.Rows[0]["Password"].ToString();
                        txtConfirmPassword.Attributes.Add("value", dtbl.Rows[0]["Password"].ToString());
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "" || txtPassword.Text == "")
                lblErrorMessage.Text = "Please Fill Mandatory Fields";
            else if (txtPassword.Text != txtConfirmPassword.Text)
                lblErrorMessage.Text = "Passwords do not match";
            else
            {
                // Hash the password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text.Trim());

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    SqlCommand sqlCmd = new SqlCommand("UserAddOrEdit", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@UserID", Convert.ToInt32(hfUserID.Value == "" ? "0" : hfUserID.Value));
                    sqlCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@Contact", txtContact.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
                    sqlCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@Password", hashedPassword); // Store hashed password
                    sqlCmd.ExecuteNonQuery();
                    Clear();
                    lblSuccessMessage.Text = "Submitted Successfully";
                }
            }
        }


        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Get the login credentials
            string username = txtLoginUsername.Text.Trim();
            string password = txtLoginPassword.Text.Trim();

            // Check if the provided credentials are valid
            if (IsValidLogin(username, password))
            {
                // Redirect to captureModule.aspx on successful login
                Response.Redirect("captureModule.aspx");
            }
            else
            {
                // Display login error message without redirecting
                lblErrorMessage.Text = "Invalid login credentials";

                // Debugging: Add a line to check if this block is executed
                // This can help you identify if the issue is with the validation
                // or the redirection
                System.Diagnostics.Debug.WriteLine("Invalid credentials block executed.");
            }
        }

        private bool IsValidLogin(string username, string password)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlCommand sqlCmd = new SqlCommand("SELECT UserID, Password FROM [dbo].[UserRegistrationDB] WHERE Username = @Username", sqlCon);
                sqlCmd.Parameters.AddWithValue("@Username", username);

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int userID = Convert.ToInt32(reader["UserID"]);
                        string storedPasswordHash = reader["Password"].ToString();

                        // Compare the entered password with the stored hash
                        if (VerifyPassword(password, storedPasswordHash))
                        {
                            // Set the user ID in a session variable
                            Session["UserID"] = userID;

                            System.Diagnostics.Debug.WriteLine("Login successful.");
                            return true;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Login failed. Invalid credentials.");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No matching record found.");
                    }
                }
            }

            // If no matching record is found or credentials don't match, return false
            return false;
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        { 
            return enteredPassword == storedPasswordHash;
        }





        void Clear()
        {
            txtFirstName.Text = txtLastName.Text = txtContact.Text = txtAddress.Text = txtUsername.Text = txtPassword.Text = txtConfirmPassword.Text = "";
            hfUserID.Value = "";
            lblSuccessMessage.Text = lblErrorMessage.Text = "";
        }
    }
}
