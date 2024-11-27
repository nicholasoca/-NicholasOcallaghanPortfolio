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
    public class EmployeeRecord
    {
        public string Employee { get; set; }
        public int EmployeeNo { get; set; }
        public double WorkHours { get; set; }
        public double OvertimeHours { get; set; }
        public double GrossPay { get; set; }
        public double StatutoryPay { get; set; }
        public double Deductions { get; set; }
        public double NetSalary { get; set; }
        public string Status { get; set; }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
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

        private string _filterText;
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        private List<EmployeeRecord> _allEmployeeRecords;
        private List<EmployeeRecord> _filteredEmployeeRecords;
        public List<EmployeeRecord> FilteredEmployeeRecords
        {
            get { return _filteredEmployeeRecords; }
            set
            {
                _filteredEmployeeRecords = value;
                OnPropertyChanged();
            }
        }

        public int clickCount = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        string connectionString = App.ConnectionString;

        public ICommand OpenLoginCommand { get; }
        public ICommand OpenEmployeesCommand { get; }
        public ICommand OpenEditCommand { get; }
        public ICommand OpenRegisterCommand { get; }
        public ICommand OpenStatisticsCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenDashboardCommand { get; }
        public ICommand OpenPayrollHandlingCommand { get; }
        public ICommand OpenUserLoginCommand { get; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
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
            OpenUserLoginCommand = new RelayCommand(OpenUserLoginWindow);

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            _allEmployeeRecords = new List<EmployeeRecord>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employees";
                MySqlCommand command = new MySqlCommand(query, connection);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    double totalWorkHours = 0;
                    double totalDeductions = 0;
                    double totalStatutoryPay = 0;
                    double totalPayrollCost = 0;
                    double totalNetSallary = 0;
                    double totalEmployees = 0;

                    while (reader.Read())
                    {
                        EmployeeRecord record = new EmployeeRecord();
                        record.Employee = reader.GetString("EmployeeName");
                        record.EmployeeNo = reader.GetInt32("EmployeeNo");
                        record.WorkHours = reader.GetDouble("WorkHours");
                        record.OvertimeHours = reader.GetDouble("OvertimeHours");
                        record.GrossPay = reader.GetDouble("GrossPay");
                        record.StatutoryPay = reader.GetDouble("StatutoryPay");
                        record.Deductions = reader.GetDouble("Deductions");
                        record.NetSalary = reader.GetDouble("NetSalary");
                        record.Status = reader.GetString("Status");

                        totalWorkHours += record.WorkHours;
                        totalDeductions += record.Deductions;
                        totalStatutoryPay += record.StatutoryPay;
                        totalNetSallary += record.NetSalary;
                        totalEmployees += 1;
                        totalPayrollCost += record.GrossPay;

                        _allEmployeeRecords.Add(record);
                    }

                    TotalWorkHoursTextBlock.Text = $"{totalWorkHours} HRS";
                    TotalPayrollCostTextBlock.Text = $"R {totalPayrollCost}";
                    TotalDeductionsTextBlock.Text = $"R {totalDeductions}";
                    TotalStatutoryPayTextBlock.Text = $"R {totalStatutoryPay}";
                    TotalNetSallaryTextBlock.Text = $"R {totalNetSallary}";
                    TotalEmployeesTextBlock.Text = $"{totalEmployees}";
                }
            }

            FilteredEmployeeRecords = new List<EmployeeRecord>(_allEmployeeRecords);
            EmployeeListView.ItemsSource = FilteredEmployeeRecords;
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                FilteredEmployeeRecords = new List<EmployeeRecord>(_allEmployeeRecords);
            }
            else
            {
                FilteredEmployeeRecords = _allEmployeeRecords.Where(e =>
                    e.Employee.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.EmployeeNo.ToString().IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.WorkHours.ToString().IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.OvertimeHours.ToString().IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.GrossPay.ToString().IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.StatutoryPay.ToString().IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.Deductions.ToString().IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.NetSalary.ToString().IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.Status.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            EmployeeListView.ItemsSource = FilteredEmployeeRecords;
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

        private void OpenUserLoginWindow()
        {
            UserLogin UserLoginWindow = new UserLogin();
            UserLoginWindow.Show();
            this.Hide();
        }
    }

}
