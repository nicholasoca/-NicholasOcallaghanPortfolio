using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;

namespace User_Registration
{
    public partial class captureModule : Page
    {
        // Your connection string
        private static string connectionString = @"Server=NICHOLASDESKTOP\SQLEXPRESS;Database=UserRegistrationDB;Integrated Security=True;";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Code to execute only when the page is loaded for the first time
            }
            else
            {
                // Code to execute on postback (when the page is submitted)
            }
        }

        public static string SaveData(List<ModuleData> modules)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();

                    foreach (ModuleData module in modules)
                    {
                        SqlCommand sqlCmd = new SqlCommand("INSERT INTO ModuleDetails (ModuleCode, ModuleName, ModuleCredit, ClassHoursPerWeek, NumberOfWeeks, StartDate, NumberOfHoursWorked, DateWorked, UserID) VALUES (@ModuleCode, @ModuleName, @ModuleCredit, @ClassHoursPerWeek, @NumberOfWeeks, @StartDate, @NumberOfHoursWorked, @DateWorked, @UserID)", sqlCon);

                        sqlCmd.Parameters.AddWithValue("@ModuleCode", module.ModuleCode);
                        sqlCmd.Parameters.AddWithValue("@ModuleName", module.ModuleName);
                        sqlCmd.Parameters.AddWithValue("@ModuleCredit", module.ModuleCredit);
                        sqlCmd.Parameters.AddWithValue("@ClassHoursPerWeek", module.ClassHoursPerWeek);
                        sqlCmd.Parameters.AddWithValue("@NumberOfWeeks", module.NumberOfWeeks);
                        sqlCmd.Parameters.AddWithValue("@StartDate", module.StartDate);
                        sqlCmd.Parameters.AddWithValue("@NumberOfHoursWorked", module.NumberOfHoursWorked);
                        sqlCmd.Parameters.AddWithValue("@DateWorked", module.DateWorked);
                        sqlCmd.Parameters.AddWithValue("@UserID", module.UserID);

                        sqlCmd.ExecuteNonQuery();
                    }
                }

                // Return a success message
                return "Data saved successfully!";
            }
            catch (Exception ex)
            {
                // Log the error or handle it as needed
                // You can also return an error message
                return $"Error: {ex.Message}";
            }
        }

        protected void btnSaveToDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input fields before saving data
                if (ValidateInput())
                {
                    // Example: Insert data into the database
                    ModuleData module = new ModuleData
                    {
                        ModuleCode = moduleCode.Text,
                        ModuleName = moduleName.Text,
                        ModuleCredit = ConvertToInt32Safe(moduleCredit.Text),
                        ClassHoursPerWeek = ConvertToInt32Safe(classHoursPerWeek.Text),
                        NumberOfWeeks = ConvertToInt32Safe(numberOfWeeks.Text),
                        StartDate = ParseDateTimeExactSafe(startDate.Text, "yyyy/MM/dd"),
                        NumberOfHoursWorked = ConvertToInt32Safe(numberOfHoursWorked.Text),
                        DateWorked = ParseDateTimeExactSafe(dateWorked.Text, "yyyy/MM/dd"),
                        UserID = GetCurrentUserID()
                    };

                    // Log the values to check if they are correct
                    System.Diagnostics.Debug.WriteLine($"ModuleCode: {module.ModuleCode}");
                    System.Diagnostics.Debug.WriteLine($"ModuleName: {module.ModuleName}");
                    System.Diagnostics.Debug.WriteLine($"ModuleCredit: {module.ModuleCredit}");
                    System.Diagnostics.Debug.WriteLine($"ClassHoursPerWeek: {module.ClassHoursPerWeek}");
                    System.Diagnostics.Debug.WriteLine($"NumberOfWeeks: {module.NumberOfWeeks}");
                    System.Diagnostics.Debug.WriteLine($"StartDate: {module.StartDate}");
                    System.Diagnostics.Debug.WriteLine($"NumberOfHoursWorked: {module.NumberOfHoursWorked}");
                    System.Diagnostics.Debug.WriteLine($"DateWorked: {module.DateWorked}");
                    System.Diagnostics.Debug.WriteLine($"UserID: {module.UserID}");

                    InsertModuleData(module);

                    // Provide feedback to the user
                    lblSuccessMessage.Text = "Data saved successfully.";
                }
                else
                {
                    // Handle validation errors
                    lblErrorMessage.Text = "Validation failed. Please check your input.";
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, show an error message)
                lblErrorMessage.Text = "An error occurred while processing the data.";
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private bool ValidateInput()
        {
            List<string> validationErrors = new List<string>();

            if (string.IsNullOrEmpty(moduleCode.Text))
                validationErrors.Add("Module Code is required.");

            if (string.IsNullOrEmpty(moduleName.Text))
                validationErrors.Add("Module Name is required.");

            // Update validation for Module Credit
            if (string.IsNullOrEmpty(moduleCredit.Text) || ConvertToInt32Safe(moduleCredit.Text) <= 0)
                validationErrors.Add("Module Credit must be a valid positive integer.");

            // Update validation for Class Hours Per Week
            if (string.IsNullOrEmpty(classHoursPerWeek.Text) || ConvertToInt32Safe(classHoursPerWeek.Text) <= 0)
                validationErrors.Add("Class Hours Per Week must be a valid positive integer.");

            // Update validation for Number of Weeks
            if (string.IsNullOrEmpty(numberOfWeeks.Text) || ConvertToInt32Safe(numberOfWeeks.Text) <= 0)
                validationErrors.Add("Number of Weeks must be a valid positive integer.");

            // Update validation for Start Date
            if (string.IsNullOrEmpty(startDate.Text) || !DateTime.TryParseExact(startDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                validationErrors.Add($"Start Date must be a valid date in the format yyyy-MM-dd. Entered value: {startDate.Text}");



            // Update validation for Number of Hours Worked
            if (string.IsNullOrEmpty(numberOfHoursWorked.Text) || ConvertToInt32Safe(numberOfHoursWorked.Text) <= 0)
                validationErrors.Add("Number of Hours Worked must be a valid positive integer.");

            // Update validation for Date Worked
            if (string.IsNullOrEmpty(dateWorked.Text) || !DateTime.TryParseExact(dateWorked.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                validationErrors.Add($"Date Worked must be a valid date in the format yyyy-MM-dd. Entered value: {dateWorked.Text}");

            if (validationErrors.Count > 0)
            {
                Label1.Text = string.Join("<br />", validationErrors);
                return false;
            }

            return true;
        }


        private DateTime ParseDateTimeExactSafe(string value, string format)
        {
            DateTime result;
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            // Handle conversion failure, you might want to throw an exception or return a default value
            throw new ArgumentException("Value is not a valid DateTime.");
        }


        private int ConvertToInt32Safe(string value)
        {
            int result;
            return int.TryParse(value, out result) ? result : 0;
        }

        private int GetCurrentUserID()
        {
            // Retrieve user ID from the session, return -1 if not found or not an integer
            if (Session["UserID"] != null && int.TryParse(Session["UserID"].ToString(), out int userID))
            {
                return userID;
            }
            return -1;
        }

        private static void InsertModuleData(ModuleData module)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();

                string query = @"
                    INSERT INTO dbo.ModuleDetails (ModuleCode, ModuleName, ModuleCredit, ClassHoursPerWeek, NumberOfWeeks, StartDate, NumberOfHoursWorked, DateWorked, UserID)
                    VALUES (@ModuleCode, @ModuleName, @ModuleCredit, @ClassHoursPerWeek, @NumberOfWeeks, @StartDate, @NumberOfHoursWorked, @DateWorked, @UserID);
                ";

                using (SqlCommand sqlCmd = new SqlCommand(query, sqlCon))
                {
                    // Add parameters
                    sqlCmd.Parameters.AddWithValue("@ModuleCode", module.ModuleCode);
                    sqlCmd.Parameters.AddWithValue("@ModuleName", module.ModuleName);
                    sqlCmd.Parameters.AddWithValue("@ModuleCredit", module.ModuleCredit);
                    sqlCmd.Parameters.AddWithValue("@ClassHoursPerWeek", module.ClassHoursPerWeek);
                    sqlCmd.Parameters.AddWithValue("@NumberOfWeeks", module.NumberOfWeeks);
                    sqlCmd.Parameters.AddWithValue("@StartDate", module.StartDate);
                    sqlCmd.Parameters.AddWithValue("@NumberOfHoursWorked", module.NumberOfHoursWorked);
                    sqlCmd.Parameters.AddWithValue("@DateWorked", module.DateWorked);
                    sqlCmd.Parameters.AddWithValue("@UserID", module.UserID);

                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public class ModuleData
        {
            public string ModuleCode { get; set; }
            public string ModuleName { get; set; }
            public int ModuleCredit { get; set; }
            public int ClassHoursPerWeek { get; set; }
            public int NumberOfWeeks { get; set; }
            public DateTime StartDate { get; set; }
            public int NumberOfHoursWorked { get; set; }
            public DateTime DateWorked { get; set; }
            public int UserID { get; set; }
        }
    }
}
