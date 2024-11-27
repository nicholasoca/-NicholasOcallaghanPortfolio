using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;
using System.Windows.Controls;

namespace Payroll_Appplication
{
    public partial class Profile : Window, INotifyPropertyChanged
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

        public Profile()
        {
            InitializeComponent();
            LoadPendingRequests();
            DataContext = this;

            ProfileName.Text = GlobalData.AdminUsername;

            OpenLoginCommand = new RelayCommand(OpenLoginWindow);
            OpenEmployeesCommand = new RelayCommand(OpenEmployeesWindow);
            OpenEditCommand = new RelayCommand(OpenEmployeeEditWindow);
            OpenRegisterCommand = new RelayCommand(OpenRegisterEditWindow);
            OpenStatisticsCommand = new RelayCommand(OpenStatisticsWindow);
            OpenProfileCommand = new RelayCommand(OpenProfileWindow);
            OpenDashboardCommand = new RelayCommand(OpenDashboardWindow);
            OpenPayrollHandlingCommand = new RelayCommand(OpenPayrollHandlingWindow);
        }

        public class RegistrationRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPendingRequests();
        }

        private void LoadPendingRequests()
        {
            string query = "SELECT Username, Password FROM RegistrationRequests WHERE Status = 'Pending'";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string username = reader.GetString("Username");
                        string password = reader.GetString("Password");

                        // Create a new RegistrationRequest object
                        RegistrationRequest request = new RegistrationRequest
                        {
                            Username = username,
                            Password = password
                        };

                        // Add the username to the list for display
                        PendingRequestsList.Items.Add(request);

                        // Add to the output box
                        OutputBoxRequests.Text += username + " has requested access to admin rights.\n";
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            // Cast the selected item to RegistrationRequest
            RegistrationRequest selectedRequest = (RegistrationRequest)PendingRequestsList.SelectedItem;
            if (selectedRequest == null)
            {
                MessageBox.Show("Please select a request to approve.");
                return;
            }

            string selectedUsername = selectedRequest.Username;
            string selectedPassword = selectedRequest.Password;

            string queryApprove = "UPDATE RegistrationRequests SET Status = 'Approved' WHERE Username = @Username";
            string queryAddToAdmin = "INSERT INTO Admin (Username, Password) VALUES (@Username, @Password)";
            string queryDeleteRequest = "DELETE FROM RegistrationRequests WHERE Username = @Username";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Approve the request
                    MySqlCommand commandApprove = new MySqlCommand(queryApprove, connection);
                    commandApprove.Parameters.AddWithValue("@Username", selectedUsername);
                    commandApprove.ExecuteNonQuery();

                    // Add the user to the Admin table with the password
                    MySqlCommand commandAddToAdmin = new MySqlCommand(queryAddToAdmin, connection);
                    commandAddToAdmin.Parameters.AddWithValue("@Username", selectedUsername);
                    commandAddToAdmin.Parameters.AddWithValue("@Password", selectedPassword);
                    commandAddToAdmin.ExecuteNonQuery();

                    // Delete the request from the RegistrationRequests table
                    MySqlCommand commandDeleteRequest = new MySqlCommand(queryDeleteRequest, connection);
                    commandDeleteRequest.Parameters.AddWithValue("@Username", selectedUsername);
                    commandDeleteRequest.ExecuteNonQuery();

                    MessageBox.Show("Request approved.");
                    OutputBoxRequests.Text = OutputBoxRequests.Text.Replace(selectedUsername + " has requested access to admin rights.\n", "");
                    PendingRequestsList.Items.Remove(selectedRequest);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }



        private void PendingRequestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (PendingRequestsList.SelectedItem != null)
            {
                string selectedUsername = PendingRequestsList.SelectedItem.ToString();
            }
        }

        private void LoadAuditLogs()
        {
            var logs = AuditLogger.GetAuditLogs(GlobalData.AdminUsername);
            //AuditLogListView.ItemsSource = logs;
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
