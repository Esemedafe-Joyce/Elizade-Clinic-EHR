using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace ElizadeEHR
{
    public class DatabaseHelper
    {
        public static string connectionString = "server=localhost;database=campusehrconsole;user=root;password=joycedafe3225%;";
        public static List<Patient> GetAllPatients()
        {
            List<Patient> patients = new List<Patient>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT PatientID, LastName, FirstName, DateOfBirth, Gender, Phone, Email FROM patients";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    patients.Add(new Patient
                    {
                        PatientID = reader.GetInt32("PatientID"),
                        LastName = reader.GetString("LastName"),
                        FirstName = reader.GetString("FirstName"),
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Gender = reader.GetString("Gender"),
                        Phone = reader.GetString("Phone"),
                        Email = reader.GetString("Email")
                    });
                }
            }
            return patients;
        }
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT UserID, LastName, FirstName, Gender, Phone, Email, Role
            FROM Users";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        UserID = reader.GetInt32("UserID"),
                        LastName = reader.GetString("LastName"),
                        FirstName = reader.GetString("FirstName"),
                        Gender = reader.GetString("Gender"),
                        Phone = reader.GetString("Phone"),
                        Email = reader.GetString("Email"),
                        Role = reader.GetString("Role")
                    });
                }
            }
            return users;
        }


        public static List<AuditLog> GetAllAuditLogs()
        {
            List<AuditLog> logs = new List<AuditLog>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT 
                a.LogID,
                a.UserID,
                CONCAT(u.LastName, ' ', u.FirstName) AS UserFullName,
                a.Action,
                a.Timestamp
            FROM AuditLogs a
            INNER JOIN Users u ON a.UserID = u.UserID";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    logs.Add(new AuditLog
                    {
                        LogID = reader.GetInt32("LogID"),
                        UserID = reader.GetInt32("UserID"),
                        UserFullName = reader.GetString("UserFullName"),
                        Action = reader.GetString("Action"),
                        Timestamp = reader.GetDateTime("Timestamp")
                    });
                }
            }
            return logs;
        }
        public static void LogAction(int userId, string action)
        {
            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                string q = "INSERT INTO AuditLogs (UserID, Action) VALUES (@uid, @act)";
                using (var cmd = new MySqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@act", action);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
