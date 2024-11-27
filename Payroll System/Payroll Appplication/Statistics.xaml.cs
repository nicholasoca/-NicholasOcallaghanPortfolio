using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using LiveCharts.Defaults;
using System.Data.SqlClient;
using System.Linq;

namespace Payroll_Appplication
{
    public partial class Statistics : Window, INotifyPropertyChanged
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

        public Func<double, string> Formatter { get; set; }
        public SeriesCollection GrossVsNetSalarySeries { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OpenLoginCommand { get; }
        public ICommand OpenEmployeesCommand { get; }
        public ICommand OpenEditCommand { get; }
        public ICommand OpenRegisterCommand { get; }
        public ICommand OpenStatisticsCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenDashboardCommand { get; }
        public ICommand OpenPayrollHandlingCommand { get; }

        public SeriesCollection SalaryDistributionSeries { get; set; }
        public Func<double, string> Formatter2 { get; set; }
        public string[] Labels { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Statistics()
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
            LoadPayrollData();
            LoadData();
            LoadData2();
            Formatter = value => value.ToString("C");
            Formatter2 = value => value.ToString("N0");
            DataContext = this;
        }

        private void LoadPayrollData()
        {
            string query = "SELECT EmployeeName, GrossPay, StatutoryPay, Deductions FROM Employees";
            Dictionary<string, double> payrollData = new Dictionary<string, double>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string employeeName = reader.GetString("EmployeeName");
                        double grossPay = reader.GetDouble("GrossPay");
                        double statutoryPay = reader.GetDouble("StatutoryPay");
                        double deductions = reader.GetDouble("Deductions");
                        double netPay = grossPay - statutoryPay - deductions;

                        if (!payrollData.ContainsKey("Gross Pay"))
                        {
                            payrollData["Gross Pay"] = 0;
                        }
                        payrollData["Gross Pay"] += grossPay;

                        if (!payrollData.ContainsKey("Statutory Pay"))
                        {
                            payrollData["Statutory Pay"] = 0;
                        }
                        payrollData["Statutory Pay"] += statutoryPay;

                        if (!payrollData.ContainsKey("Deductions"))
                        {
                            payrollData["Deductions"] = 0;
                        }
                        payrollData["Deductions"] += deductions;

                        if (!payrollData.ContainsKey("Net Pay"))
                        {
                            payrollData["Net Pay"] = 0;
                        }
                        payrollData["Net Pay"] += netPay;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            SeriesCollection series = new SeriesCollection();
            foreach (var item in payrollData)
            {
                series.Add(new PieSeries
                {
                    Title = item.Key,
                    Values = new ChartValues<double> { item.Value },
                    DataLabels = true
                });
            }

            PayrollPieChart.Series = series;
        }

        private void LoadData()
        {
            var grossPayValues = new List<double>();
            var netSalaryValues = new List<double>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT GrossPay, NetSalary FROM Employees", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        grossPayValues.Add(reader.GetDouble("GrossPay"));
                        netSalaryValues.Add(reader.GetDouble("NetSalary"));
                    }
                }
            }

            GrossVsNetSalarySeries = new SeriesCollection
            {
                new ScatterSeries
                {
                    Title = "Gross Pay vs. Net Salary",
                    Values = new ChartValues<ObservablePoint>()
                }
            };

            for (int i = 0; i < grossPayValues.Count; i++)
            {
                GrossVsNetSalarySeries[0].Values.Add(new ObservablePoint(grossPayValues[i], netSalaryValues[i]));
            }
        }

        private void LoadData2()
        {
            
            var netSalaryValues = new List<double>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT NetSalary FROM Employees", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        netSalaryValues.Add(reader.GetDouble("NetSalary"));
                    }
                }
            }

            var groupedData = netSalaryValues.GroupBy(s => Math.Floor(s / 1000) * 1000)
                                             .Select(g => new { Range = g.Key, Count = g.Count() })
                                             .OrderBy(g => g.Range);

            var labels = groupedData.Select(g => $"{g.Range:C0} - {(g.Range + 999):C0}").ToArray();
            var values = groupedData.Select(g => (double)g.Count).ToArray();

            SalaryDistributionSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Net Salary Distribution",
                    Values = new ChartValues<double>(values)
                }
            };

            Labels = labels;
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
