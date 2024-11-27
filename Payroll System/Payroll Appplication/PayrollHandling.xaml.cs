using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Payroll_Appplication
{
    /// <summary>
    /// Interaction logic for PayrollHandling.xaml
    /// </summary>
    public partial class PayrollHandling : Window, INotifyPropertyChanged
    {
        private bool _isRetracted;
        public bool IsRetracted
        {
            get { return _isRetracted; }
            set
            {
                _isRetracted = value;
                OnPropertyChanged();
            }
        }

        private bool overtimeIsChecked;

        public int clickCount = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OpenLoginCommand { get; }
        public ICommand OpenEmployeesCommand { get; }
        public ICommand OpenEditCommand { get; }
        public ICommand OpenRegisterCommand { get; }
        public ICommand OpenStatisticsCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenDashboardCommand { get; }
        public ICommand OpenPayrollHandlingCommand { get; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string connectionString = App.ConnectionString;

        string filePath = "C:\\Users\\coenr\\OneDrive\\Desktop\\output.pdf";
        //"C:\Users\coenr\OneDrive\Desktop\New Text Document.txt"

        public PayrollHandling()
        {
            InitializeComponent();
            DataContext = this;
            OpenLoginCommand = new RelayCommand(OpenLoginWindow);
            OpenEmployeesCommand = new RelayCommand(OpenEmployeesWindow);
            OpenEditCommand = new RelayCommand(OpenEmployeeEditWindow);
            OpenRegisterCommand = new RelayCommand(OpenRegisterEditWindow);
            OpenStatisticsCommand = new RelayCommand(OpenStatisticsWindow);
            OpenProfileCommand = new RelayCommand(OpenProfileWindow);
            OpenDashboardCommand = new RelayCommand(OpenDashboardWindow);
            OpenPayrollHandlingCommand = new RelayCommand(OpenPayrollHandlingWindow);

            LoadEmployeeNumbers();
            
        }

        private void RetractButton_Click(object sender, RoutedEventArgs e)
        {
            clickCount++;
            IsRetracted = !IsRetracted;
            if (clickCount % 2 == 0)
            {
                var retractStoryboard = (Storyboard)FindResource("RetractNavigationStoryboard");
                retractStoryboard.Begin();
            }
            else
            {
                var restoreStoryboard = (Storyboard)FindResource("RestoreNavigationStoryboard");
                restoreStoryboard.Begin();
            }

        }

        private void OpenLoginWindow()
        {

            GlobalData.AdminUsername = "";
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Hide();
        }

        private void OpenEmployeesWindow()
        {
            Employees employeeWindow = new Employees();
            employeeWindow.Show();
            this.Hide();
        }

        private void OpenEmployeeEditWindow()
        {
            EmployeeEdit employeeEdit = new EmployeeEdit();
            employeeEdit.Show();
            this.Hide();
        }

        private void OpenDashboardWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

        private void OpenPayrollHandlingWindow()
        {
            PayrollHandling payrollWindow = new PayrollHandling();
            payrollWindow.Show();
            this.Hide();
        }

        private void OpenStatisticsWindow()
        {
            Statistics statisticsWindow = new Statistics();
            statisticsWindow.Show();
            this.Hide();
        }

        private void OpenRegisterEditWindow()
        {
            RegisterPage registerWindow = new RegisterPage();
            registerWindow.Show();
            this.Hide();
        }

        private void OpenProfileWindow()
        {
            Profile profileWindow = new Profile();
            profileWindow.Show();
            this.Hide();
        }

        private void AddHoursButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve employee number and hours entered by the user
            int employeeNumber;
            decimal totalWorkHours = 0;
            decimal totalOvertimeHours = 0;

            if (!int.TryParse(cmbEmployeeNumAddWork.Text, out employeeNumber))
            {
                MessageBox.Show("Please enter a valid employee number.");
                return;
            }

            decimal hours;
            if (!decimal.TryParse(txtHours.Text, out hours))
            {
                MessageBox.Show("Please enter a valid number of hours.");
                return;
            }

            // Retrieve hourly rate and overtime rate of the employee from the database
            decimal hourlyRate = GetHourlyRate(employeeNumber);
            decimal overtimeRate = GetOvertimeRate(employeeNumber);

            if (hourlyRate == -100 || overtimeRate == -100)
            {
                MessageBox.Show("Employee not found.");
                return;
            }

            // Retrieve existing work hours and overtime hours
            totalWorkHours = GetTotalWorkHours(employeeNumber);
            totalOvertimeHours = GetTotalOvertimeHours(employeeNumber);

            // Update work hours or overtime hours based on checkbox status
            if (chkOvertime.IsChecked == false)
            {
                totalWorkHours += hours;
                overtimeIsChecked = false;
            }
            else
            {
                totalOvertimeHours += hours;
                overtimeIsChecked = true;
            }

            // Calculate new gross pay
            decimal grossPay = (totalWorkHours * hourlyRate) + (totalOvertimeHours * overtimeRate);

            // Update the database with the new total work hours and gross pay
            if (UpdateEmployeeDetails(employeeNumber, totalWorkHours, totalOvertimeHours, grossPay))
            {
                AuditLogger.LogAudit(GlobalData.AdminUsername, "Added Work Hours", "Added work Hours to Employee " + OutputBox.Text);
                OutputBox.Text = $"Work hours updated successfully.\nNew Gross Pay: {grossPay:C}";   
            }
            else
            {
                MessageBox.Show("Failed to update employee details. Please try again.");
            }
        }

        private void PrintPayslips_Click(object sender, RoutedEventArgs e)
        {
            // Create a Document object
            Document pdfDoc = new Document(PageSize.A4, 25, 25, 30, 30);

            //"C:\Users\coenr\OneDrive\Desktop\output.pdf"
            // File path for the PDF (ensure the directory exists)
            string filePath = "C:\\Users\\coenr\\OneDrive\\Desktop\\All_Payslips.pdf";
            // Ensure directory exists
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

            // Check if the checkbox is checked
            if (chkPrintAll.IsChecked == true)
            {
                // Clear the output box
                OutputBox2.Text = "";

                // Retrieve employee information for all employees from the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT EmployeeNo, EmployeeName, WorkHours, OvertimeHours, GrossPay FROM Employees";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    try
                    {
                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();

                        // Create a file stream to write the PDF file once
                        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            // Create a PDF writer to save the document
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fs);

                            // Open the document for writing
                            pdfDoc.Open();

                            while (reader.Read())
                            {
                                int employeeNumber = Convert.ToInt32(reader["EmployeeNo"]);
                                string employeeName = reader["EmployeeName"].ToString();
                                decimal hoursWorked = Convert.ToDecimal(reader["WorkHours"]);
                                decimal overtimeHours = Convert.ToDecimal(reader["OvertimeHours"]);
                                decimal totalGrossPay = Convert.ToDecimal(reader["GrossPay"]);

                                // Calculate deductions
                                decimal UIF = totalGrossPay * 0.01m; // 1% of total gross pay
                                decimal PAYE = 0; // Calculate PAYE later
                                decimal totalDeductions = UIF; // For now only UIF is deducted

                                // Calculate net salary
                                decimal netSalary = totalGrossPay - totalDeductions;

                                // Add content for each employee to the PDF
                                pdfDoc.Add(new iTextSharp.text.Paragraph($"Employee ID: {employeeNumber}\n" +
                                                         $"Employee Name: {employeeName}\n\n" +
                                                         $"Hours Worked: {hoursWorked}\n" +
                                                         $"Overtime Hours Worked: {overtimeHours}\n\n" +
                                                         $"Total Gross Pay: {totalGrossPay:C}\n\n" +
                                                         $"Minus 1% UIF: {UIF:C}\n" +
                                                         $"Minus PAYE: {PAYE:C}\n" +
                                                         $"Total Deductions: {totalDeductions:C}\n\n" +
                                                         $"Netto Salary: {netSalary:C}\n\n" +
                                                         $"====================================================="));

                                // Add payslip details to the output box
                                OutputBox2.Text = "All Payslips has been generated";
                            }

                            // Close the document after all employees are processed
                            pdfDoc.Close();
                            writer.Close();
                        }

                        Console.WriteLine("PDF created successfully at " + filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error retrieving employee information: " + ex.Message);
                    }
                }
            }
            else
            {
                // Retrieve employee number from the textbox
                if (!int.TryParse(cmbEmployeeNumPrintPayslips.Text, out int employeeNumber))
                {
                    MessageBox.Show("Please enter a valid employee number.");
                    return;
                }

                // Retrieve employee information from the database
                string employeeName = "";
                decimal hoursWorked = 0;
                decimal overtimeHours = 0;
                decimal totalGrossPay = 0;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT EmployeeName, WorkHours, OvertimeHours, GrossPay FROM Employees WHERE EmployeeNo = @EmployeeNo";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);

                    try
                    {
                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            employeeName = reader["EmployeeName"].ToString();
                            hoursWorked = Convert.ToDecimal(reader["WorkHours"]);
                            overtimeHours = Convert.ToDecimal(reader["OvertimeHours"]);
                            totalGrossPay = Convert.ToDecimal(reader["GrossPay"]);
                        }
                        else
                        {
                            MessageBox.Show("Employee not found.");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error retrieving employee information: " + ex.Message);
                        return;
                    }
                }

                // Calculate deductions
                decimal UIF = totalGrossPay * 0.01m; // 1% of total gross pay
                decimal PAYE = 0; // calculate PAYE later
                decimal totalDeductions = UIF; // For now only UIF is deducted

                // Calculate net salary
                decimal netSalary = totalGrossPay - totalDeductions;

                // Generate the PDF for the specific employee
                string filePath2 = $"C:\\Users\\coenr\\OneDrive\\Desktop\\Payslip_{employeeNumber}.pdf";
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath2)); // Ensure directory exists

                Document pdfDoc2 = new Document(PageSize.A4, 25, 25, 30, 30);
                try
                {
                    // Create a file stream to write the PDF file
                    using (FileStream fs = new FileStream(filePath2, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        // Create a PDF writer to save the document
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fs);

                        // Open the document for writing
                        pdfDoc.Open();

                        // Add content to the PDF (text, images, etc.)
                        pdfDoc.Add(new iTextSharp.text.Paragraph($"Employee ID: {employeeNumber}\n" +
                                                                 $"Employee Name: {employeeName}\n\n" +
                                                                 $"Hours Worked: {hoursWorked}\n" +
                                                                 $"Overtime Hours Worked: {overtimeHours}\n\n" +
                                                                 $"Total Gross Pay: {totalGrossPay:C}\n\n" +
                                                                 $"Minus 1% UIF: {UIF:C}\n" +
                                                                 $"Minus PAYE: {PAYE:C}\n" +
                                                                 $"Total Deductions: {totalDeductions:C}\n\n" +
                                                                 $"Netto Salary: {netSalary:C}\n\n"));

                        // Close the document
                        pdfDoc.Close();
                        writer.Close();

                        Console.WriteLine("PDF created successfully at " + filePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generating PDF: " + ex.Message);
                }

                // Display payslip details in the output box
                OutputBox2.Text = "Payslip has been generated";
            }
        }




            private bool UpdatePaymentStatus(int employeeNumber, DateTime datePaid)
        {
            // Update the status column to 'paid' for the specified employee and date paid
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE Employees SET Status = 'paid' WHERE EmployeeNo = @EmployeeNo";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    command.Parameters.AddWithValue("@DatePaid", datePaid);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }


        private decimal GetHourlyRate(int employeeNumber)
        {
            decimal hourlyRate = -1;


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                string query = "SELECT HourlyRate FROM Employees WHERE EmployeeNo = @EmployeeNo";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        hourlyRate = Convert.ToDecimal(result);
                    }
                }
            }

            return hourlyRate;
        }

        private decimal GetOvertimeRate(int employeeNumber)
        {
            decimal overtimeRate = -1;

            // Connect to the database and retrieve the hourly rate

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                string query = "SELECT OvertimeHoursRate FROM Employees WHERE EmployeeNo = @EmployeeNo";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        overtimeRate = Convert.ToDecimal(result);
                    }
                }
            }

            return overtimeRate;
        }

        private void UpdatePayButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve employee number and date paid from the textboxes
            int employeeNumber;
            if (!int.TryParse(cmbEmployeeNumPayUpdate.Text, out employeeNumber))
            {
                MessageBox.Show("Please enter a valid employee number.");
                return;
            }

            // Retrieve the selected date from the DatePicker control
            DateTime? datePaid = datePicker.SelectedDate;
            if (datePaid == null)
            {
                MessageBox.Show("Please select a valid date for when the employee was paid.");
                return;
            }

            // Update the status to 'paid' in the database
            if (UpdatePaymentStatus(employeeNumber, datePaid.Value))
            {
                AuditLogger.LogAudit(GlobalData.AdminUsername, "Updated Payment Status", "Updated payment status FORMAT employee" + OutputBox2.Text);
                OutputBox1.Text = $"Status updated to 'paid' for Employee {employeeNumber} on {datePaid.Value.ToShortDateString()}.";
            }
            else
            {
                MessageBox.Show("Failed to update payment status. Please try again.");
            }
        }


        private decimal GetTotalWorkHours(int employeeNumber)
        {
            decimal totalWorkHours = 0;

            // Connect to the database and retrieve the total work hours
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT WorkHours FROM Employees WHERE EmployeeNo = @EmployeeNo";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        totalWorkHours = Convert.ToDecimal(result);
                    }
                }
            }

            return totalWorkHours;
        }

        private decimal GetTotalOvertimeHours(int employeeNumber)
        {
            decimal totalOvertimeHours = 0;

            // Connect to the database and retrieve the total overtime hours
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT OvertimeHours FROM Employees WHERE EmployeeNo = @EmployeeNo";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        totalOvertimeHours = Convert.ToDecimal(result);
                    }
                }
            }

            return totalOvertimeHours;
        }


        private decimal GetGrossPay(int employeeNumber)
        {
            decimal grossPay = 0;

            // Connect to the database and retrieve the total work hours
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT GrossPay FROM Employees WHERE EmployeeNo = @EmployeeNo";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        grossPay = Convert.ToDecimal(result);
                    }
                }
            }

            return grossPay;
        }

        private bool UpdateEmployeeDetails(int employeeNumber, decimal totalWorkHours, decimal totalOvertimeHours, decimal grossPay)
        {
            // Update the database with the new total work hours and gross pay
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                if (overtimeIsChecked == false)
                {
                    string query = "UPDATE Employees SET WorkHours = @WorkHours, GrossPay = @GrossPay WHERE EmployeeNo = @EmployeeNo";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@WorkHours", totalWorkHours);
                        command.Parameters.AddWithValue("@GrossPay", grossPay);
                        command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                else
                {
                    string query = "UPDATE Employees SET OvertimeHours = @OvertimeHours, GrossPay = @GrossPay WHERE EmployeeNo = @EmployeeNo";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OvertimeHours", totalOvertimeHours);
                        command.Parameters.AddWithValue("@GrossPay", grossPay);
                        command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
        }

        private void LoadEmployeeNumbers()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT EmployeeNo FROM Employees";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    // Clear existing items
                    cmbEmployeeNumAddWork.Items.Clear();
                    cmbEmployeeNumPayUpdate.Items.Clear();
                    cmbEmployeeNumPrintPayslips.Items.Clear();

                    // Add employee numbers to ComboBoxes
                    while (reader.Read())
                    {
                        cmbEmployeeNumAddWork.Items.Add(reader["EmployeeNo"].ToString());
                        cmbEmployeeNumPayUpdate.Items.Add(reader["EmployeeNo"].ToString());
                        cmbEmployeeNumPrintPayslips.Items.Add(reader["EmployeeNo"].ToString());
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void cmbEmployeeNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEmployeeNumAddWork.SelectedItem != null)
            {
                // Fetch and display employee data based on selected employee number
                FetchAndDisplayEmployeeName(Convert.ToInt32(cmbEmployeeNumAddWork.SelectedItem));
            }
        }

        private void cmbEmployeeNum_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEmployeeNumPayUpdate.SelectedItem != null)
            {
                // Fetch and display employee data based on selected employee number
                FetchAndDisplayEmployeeName2(Convert.ToInt32(cmbEmployeeNumPayUpdate.SelectedItem));
            }
        }

        private void cmbEmployeeNum_SelectionChanged3(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEmployeeNumPrintPayslips.SelectedItem != null)
            {
                // Fetch and display employee data based on selected employee number
                FetchAndDisplayEmployeeName3(Convert.ToInt32(cmbEmployeeNumPrintPayslips.SelectedItem));
            }
        }

        private void FetchAndDisplayEmployeeName(int employeeNumber)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Employees WHERE EmployeeNo = @EmployeeNo";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        OutputBox.Text = reader["EmployeeName"].ToString();

                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void FetchAndDisplayEmployeeName2(int employeeNumber)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Employees WHERE EmployeeNo = @EmployeeNo";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        OutputBox1.Text = reader["EmployeeName"].ToString();

                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void FetchAndDisplayEmployeeName3(int employeeNumber)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Employees WHERE EmployeeNo = @EmployeeNo";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        OutputBox2.Text = reader["EmployeeName"].ToString();

                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
