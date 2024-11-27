using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Payroll_Appplication
{
    public static class GlobalData
    {
        public static string AdminUsername { get; set; }

        public static int LoginEmployeeID { get; set; }
    }

    public static class AuditLogger
    {
        private static string connectionString = App.ConnectionString;

        public static void LogAudit(string username, string action, string additionalInfo = "")
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO AuditLog (Username, ActionDescription, Timestamp, AdditionalInfo) " +
                                   "VALUES (@Username, @ActionDescription, @Timestamp, @AdditionalInfo)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@ActionDescription", action);
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                    command.Parameters.AddWithValue("@AdditionalInfo", additionalInfo);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error logging audit: {ex.Message}");
            }
        }

        public class AuditLogEntry
        {
            public string ActionDescription { get; set; }
            public DateTime Timestamp { get; set; }
            public string AdditionalInfo { get; set; }

            // Optionally, you can add a constructor for easy initialization
            public AuditLogEntry(string actionDescription, DateTime timestamp, string additionalInfo)
            {
                ActionDescription = actionDescription;
                Timestamp = timestamp;
                AdditionalInfo = additionalInfo;
            }

            // Default constructor
            public AuditLogEntry() { }
        }

        public static List<AuditLogEntry> GetAuditLogs(string username)
        {
            List<AuditLogEntry> logs = new List<AuditLogEntry>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ActionDescription, Timestamp, AdditionalInfo FROM AuditLog WHERE Username = @Username ORDER BY Timestamp DESC";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        logs.Add(new AuditLogEntry
                        {
                            ActionDescription = reader["ActionDescription"].ToString(),
                            Timestamp = Convert.ToDateTime(reader["Timestamp"]),
                            AdditionalInfo = reader["AdditionalInfo"].ToString()
                        });
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading audit logs: {ex.Message}");
            }

            return logs;
        }

    }
}
