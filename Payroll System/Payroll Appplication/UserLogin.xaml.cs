using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;
using System;

namespace Payroll_Appplication
{
    public class MyRecord
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



    public partial class UserLogin : Window, INotifyPropertyChanged
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

        public UserLogin()
        {
            InitializeComponent();
            DataContext = this;
            OpenUserDashboardCommand = new RelayCommand(OpenUserDashboardWindow);
            OpenPersonalCommand = new RelayCommand(OpenPersonalDetailsWindow);
            EmployeeNumComp = GlobalData.LoginEmployeeID;
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
                        record.EmployeeName = reader.GetString("EmployeeName");
                        record.EmployeeNo = reader.GetInt32("EmployeeNo");
                        record.WorkHours = reader.GetDouble("WorkHours");
                        record.OvertimeHours = reader.GetDouble("OvertimeHours");
                        record.GrossPay = reader.GetDouble("GrossPay");
                        record.StatutoryPay = reader.GetDouble("StatutoryPay");
                        record.Deductions = reader.GetDouble("Deductions");
                        record.NetSalary = reader.GetDouble("NetSalary");
                        record.PhoneNumber = reader.GetString("PhoneNumber");
                        record.EmailAddress = reader.GetString("EmailAddress");
                        record.MaritalStatus = reader.GetString("MaritalStatus");
                        record.Status = reader.GetString("Status");

                        _allMyRecords.Add(record);

                        // Updating TextBlocks with record data
                        NameTextBlock.Text = record.EmployeeName;
                        IDTextBlock.Text = record.EmployeeNo.ToString();
                        TotalWorkHoursTextBlock.Text = $"{record.WorkHours} HRS";
                        OvertimeTextBlock.Text = $"{record.OvertimeHours} HRS";
                        TotalGrossPay.Text = $"R {record.GrossPay}";
                        TotalStatutoryPayTextBlock.Text = $"R {record.StatutoryPay}";
                        TotalDeductionsTextBlock.Text = $"R {record.Deductions}";
                        TotalNetSallaryTextBlock.Text = $"R {record.NetSalary}";
                        PhoneNumberTextBlock.Text = record.PhoneNumber;
                        EmailAressTextBlock.Text = record.EmailAddress;
                        MaritalStatusTextBlock.Text = record.MaritalStatus;
                        StatusTextBlock.Text = record.Status;
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

           
        
    

