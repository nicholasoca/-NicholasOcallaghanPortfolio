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
using MySql.Data.MySqlClient;


namespace Payroll_Appplication
{
    /// <summary>
    /// Interaction logic for UserPersonalDetails.xaml
    /// </summary>
    public partial class UserPersonalDetails : Window
    {
        public class MyRecord
        {
            public string PhoneNumber { get; set; }
            public string EmailAddress { get; set; }
            public string MaritalStatus { get; set; }
        }


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

        private int EmployeeNumComp = 1;

        private List<MyRecord> _allMyRecords;


        public int clickCount = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        string connectionString = App.ConnectionString;

        public ICommand OpenUserDashboardCommand { get; }
        public ICommand OpenPersonalCommand { get; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public UserPersonalDetails()
        {
            InitializeComponent();
            EmployeeNumComp = GlobalData.LoginEmployeeID;
            OpenUserDashboardCommand = new RelayCommand(OpenUserDashboardWindow);
            OpenPersonalCommand = new RelayCommand(OpenPersonalDetailsWindow);

            LoadDataFromDatabase();

        }

        private void LoadDataFromDatabase()
        {
            _allMyRecords = new List<MyRecord>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employees WHERE EmployeeNo = " + EmployeeNumComp;
                MySqlCommand command = new MySqlCommand(query, connection);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MyRecord record = new MyRecord();
                        
                        record.PhoneNumber = reader.GetString("PhoneNumber");
                        record.EmailAddress = reader.GetString("EmailAddress");
                        record.MaritalStatus = reader.GetString("MaritalStatus");

                        _allMyRecords.Add(record);

                        // Updating TextBlocks with record data
                        txtPhone.Text = record.PhoneNumber;
                        txtEmail.Text = record.EmailAddress;
                        txtmarried.Text = record.MaritalStatus;
                        
                    }


                }
            }
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
        private void UpdateDetails_Click(object sender, RoutedEventArgs e)
        {
            UpdatePersonalDetails(EmployeeNumComp);
        }

        private bool UpdatePersonalDetails(int employeeNumber)
        {
            
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                
                string query = @"
            UPDATE Employees 
            SET PhoneNumber = @PhoneNumber, 
                EmailAddress = @EmailAddress, 
                MaritalStatus = @MaritalStatus 
            WHERE EmployeeNo = @EmployeeNo";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@EmployeeNo", employeeNumber);
                    command.Parameters.AddWithValue("@PhoneNumber", txtPhone.Text);
                    command.Parameters.AddWithValue("@EmailAddress", txtEmail.Text); 
                    command.Parameters.AddWithValue("@MaritalStatus", txtmarried.Text);

                    
                    connection.Open();

                    
                    int rowsAffected = command.ExecuteNonQuery();

                    // Return true if at least one row was updated, false otherwise
                    return rowsAffected > 0;
                }
            }
        }




        private void OpenUserDashboardWindow()
        {
            UserLogin userLogin = new UserLogin();
            userLogin.Show();
            this.Hide();
        }

        private void OpenPersonalDetailsWindow()
        {
            UserPersonalDetails userPersonal = new UserPersonalDetails();
            userPersonal.Show();
            this.Hide();
        }

    }

}
