using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Controls;
using System;

namespace Payroll_Appplication
{
    public partial class Employees : Window, INotifyPropertyChanged
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

        public int clickCount = 1;

        string connectionString = App.ConnectionString;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Clear1 = false, Clear2 = false;

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

        public Employees()
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
            LoadDataFromDatabase();
            LoadEmployeeNumbers();
        }

        public class EmployeeRecord
        {
            public string EmployeeName { get; set; }
            public int EmployeeNo { get; set; }
            public double WorkHours { get; set; }
            public double OvertimeHours { get; set; }
            public double GrossPay { get; set; }
            public double StatutoryPay { get; set; }
            public double Deductions { get; set; }
            public double NetSalary { get; set; }
            public string PhoneNumber { get; set; }
            public string EmailAddress { get; set; }
            public string MaritalStatus { get; set; }
            public string Status { get; set; }
        }

        private void LoadDataFromDatabase()
        {
            List<EmployeeRecord> employeeRecords = new List<EmployeeRecord>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employees";
                MySqlCommand command = new MySqlCommand(query, connection);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        EmployeeRecord employee = new EmployeeRecord();
                        employee.EmployeeName = reader.GetString("EmployeeName");
                        employee.EmployeeNo = reader.GetInt32("EmployeeNo");
                        employee.WorkHours = reader.GetDouble("WorkHours");
                        employee.OvertimeHours = reader.GetDouble("OvertimeHours");
                        employee.GrossPay = reader.GetDouble("GrossPay");
                        employee.StatutoryPay = reader.GetDouble("StatutoryPay");
                        employee.Deductions = reader.GetDouble("Deductions");
                        employee.NetSalary = reader.GetDouble("NetSalary");
                        employee.PhoneNumber = reader.GetString("PhoneNumber");
                        employee.EmailAddress = reader.GetString("EmailAddress");
                        employee.MaritalStatus = reader.GetString("MaritalStatus");
                        employee.Status = reader.GetString("Status");

                        employeeRecords.Add(employee);
                    } 

                }
            }



            EmployeeListView.ItemsSource = employeeRecords;
            EmployeeContactView.ItemsSource = employeeRecords;
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
                    cmbEmployeeNum.Items.Clear();
                    cmbEmployeeNum2.Items.Clear();

                    // Add employee numbers to ComboBoxes
                    while (reader.Read())
                    {
                        cmbEmployeeNum.Items.Add(reader["EmployeeNo"].ToString());
                        cmbEmployeeNum2.Items.Add(reader["EmployeeNo"].ToString());
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
            if (cmbEmployeeNum.SelectedItem != null)
            {
                int selectedEmployeeNo = Convert.ToInt32(cmbEmployeeNum.SelectedItem);
                List<EmployeeRecord> filteredEmployeeRecords = GetFilteredEmployeeRecords(selectedEmployeeNo);
                EmployeeListView.ItemsSource = filteredEmployeeRecords;
            }
        }

        private void cmbEmployeeNum2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEmployeeNum2.SelectedItem != null)
            {
                int selectedEmployeeNo = Convert.ToInt32(cmbEmployeeNum2.SelectedItem);
                List<EmployeeRecord> filteredEmployeeRecords = GetFilteredEmployeeRecords(selectedEmployeeNo);
                EmployeeContactView.ItemsSource = filteredEmployeeRecords;
            }
        }

        private List<EmployeeRecord> GetFilteredEmployeeRecords(int selectedEmployeeNo)
        {
            List<EmployeeRecord> filteredEmployeeRecords = new List<EmployeeRecord>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employees WHERE EmployeeNo = @EmployeeNo";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeNo", selectedEmployeeNo);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EmployeeRecord employee = new EmployeeRecord();
                        employee.EmployeeName = reader.GetString("EmployeeName");
                        employee.EmployeeNo = reader.GetInt32("EmployeeNo");
                        employee.WorkHours = reader.GetDouble("WorkHours");
                        employee.OvertimeHours = reader.GetDouble("OvertimeHours");
                        employee.GrossPay = reader.GetDouble("GrossPay");
                        employee.StatutoryPay = reader.GetDouble("StatutoryPay");
                        employee.Deductions = reader.GetDouble("Deductions");
                        employee.NetSalary = reader.GetDouble("NetSalary");
                        employee.PhoneNumber = reader.GetString("PhoneNumber");
                        employee.EmailAddress = reader.GetString("EmailAddress");
                        employee.MaritalStatus = reader.GetString("MaritalStatus");
                        employee.Status = reader.GetString("Status");

                        filteredEmployeeRecords.Add(employee);
                    }
                }
            }
            return filteredEmployeeRecords;
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            cmbEmployeeNum.SelectedItem = null;
            LoadDataFromDatabase();
        }

        private void ClearFilter2_Click(object sender, RoutedEventArgs e)
        {
            cmbEmployeeNum2.SelectedItem = null;
            LoadDataFromDatabase();
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

        
    }
}

