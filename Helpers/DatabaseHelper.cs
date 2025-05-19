using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;

namespace ElizadeEHR
{
    public class DatabaseHelper
    {
        public static string connectionString = "server=localhost;database=campusehrconsole;user=root;password=joycedafe3225%;";
        public static List<Patient> GetAllPatients()
        {
            List<Patient> patients = new List<Patient>();


                using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                string query = "SELECT PatientID, MatricNumber, LastName, FirstName, DateOfBirth, Gender, Phone, Email FROM patients";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        patients.Add(new Patient
                        {
                            PatientID = reader.GetInt32("PatientID"),
                            MatricNumber = reader.IsDBNull(reader.GetOrdinal("MatricNumber")) ? null : reader.GetString("MatricNumber"),
                            LastName = reader.GetString("LastName"),
                            FirstName = reader.GetString("FirstName"),
                            DateOfBirth = reader.GetDateTime("DateOfBirth"),
                            Gender = reader.GetString("Gender"),
                            Phone = reader.GetString("Phone"),
                            Email = reader.GetString("Email"),
                        });
                    }
                }
            
            return patients;
        }
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                string query = @"
            SELECT UserID, LastName, FirstName, Gender, Phone, Email, Role, CreatedAt
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
                        Role = reader.GetString("Role"),
                        CreatedAt = reader.GetDateTime("CreatedAt")
                    });
                }
            }
            return users;
        }


        public static List<AuditLog> GetAllAuditLogs()
        {
            List<AuditLog> logs = new List<AuditLog>();
            using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
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
                INNER JOIN Users u ON a.UserID = u.UserID
                ORDER BY a.Timestamp DESC";

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


        public static void DeleteUser(int userId)
        {
            var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            string query = "DELETE FROM Users WHERE UserID = @id";
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
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

        public static bool SaveUser(User user)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                INSERT INTO Users 
                (LastName, FirstName, Gender, Phone, Email, Role, PasswordHash) 
                VALUES 
                (@LastName, @FirstName, @Gender, @Phone, @Email, @Role, @PasswordHash)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@Gender", user.Gender);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash); // 🛠️ Add this

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}");
                return false;
            }
        }
        public static bool UpdateUser(User user)
        {
            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                string query = user.PasswordHash != null
                    ? "UPDATE Users SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Phone=@Phone, Gender=@Gender, Role=@Role, PasswordHash=@PasswordHash WHERE UserID=@UserID"
                    : "UPDATE Users SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Phone=@Phone, Gender=@Gender, Role=@Role WHERE UserID=@UserID";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Phone", user.Phone);
                cmd.Parameters.AddWithValue("@Gender", user.Gender);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@UserID", user.UserID);

                if (user.PasswordHash != null)
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool SavePatient(Patient patient)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString ))
                {
                    conn.Open();
                    string query = @"
                    INSERT INTO Patients 
                    (LastName, FirstName, Gender, Phone, Email, DateOfBirth, MatricNumber) 
                    VALUES 
                    (@LastName, @FirstName, @Gender, @Phone, @Email, @DateOfBirth, @MatricNumber)";


                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@LastName", patient.LastName);
                    cmd.Parameters.AddWithValue("@FirstName", patient.FirstName);
                    cmd.Parameters.AddWithValue("@Gender", patient.Gender);
                    cmd.Parameters.AddWithValue("@Phone", patient.Phone);
                    cmd.Parameters.AddWithValue("@MatricNumber", patient.MatricNumber);
                    cmd.Parameters.AddWithValue("@Email", patient.Email);
                    cmd.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}");
                return false;
            }
        }
        public static bool UpdatePatient(Patient patient)
        {
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "UPDATE Patients SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Phone=@Phone, Gender=@Gender, DateOfBirth=@DateOfBirth, MatricNumber=@MatricNumber WHERE PatientID=@PatientID";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FirstName", patient.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", patient.LastName);
                    cmd.Parameters.AddWithValue("@Email", patient.Email);
                    cmd.Parameters.AddWithValue("@Phone", patient.Phone);
                    cmd.Parameters.AddWithValue("@Gender", patient.Gender);
                    cmd.Parameters.AddWithValue("@MatricNumber", patient.MatricNumber);
                    cmd.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
                    cmd.Parameters.AddWithValue("@PatientID", patient.PatientID); // ✅ Critical

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update patient: " + ex.Message);
                return false;
            }
        }

        public static void DeletePatient(int patientId)
        {
            var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            string query = "DELETE FROM Patients WHERE PatientId = @id";
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", patientId);
            cmd.ExecuteNonQuery();
        }
    }
}
