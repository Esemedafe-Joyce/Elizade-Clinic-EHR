using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ElizadeEHR.Helpers;
using MySqlConnector;

namespace ElizadeEHR
{
    public class DatabaseHelper
    {
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

        public static bool SavePatient(Patient patient)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
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
        // Update the GetPendingPatientsForConsultation method to include MedicalAlerts
        public static List<Patient> GetPendingPatientsForConsultation()
        {
            var patients = new List<Patient>();

            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                string query = @"
        SELECT PatientID, FirstName, LastName, Phone, Gender, MatricNumber, Email, DateOfBirth, MedicalAlerts
        FROM patients";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        patients.Add(new Patient
                        {
                            PatientID = reader.GetInt32("PatientID"),
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName"),
                            DateOfBirth = reader.GetDateTime("DateOfBirth"),
                            Gender = reader.GetString("Gender"),
                            MatricNumber = reader.IsDBNull(reader.GetOrdinal("MatricNumber")) ? null : reader.GetString("MatricNumber"),
                            Email = reader.GetString("Email"),
                            Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString("Phone"),
                            MedicalAlerts = reader.IsDBNull(reader.GetOrdinal("MedicalAlerts")) ? null : reader.GetString("MedicalAlerts")
                        });
                    }
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

        public static void DeleteUser(int userId)
        {
            var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            string query = "DELETE FROM Users WHERE UserID = @id";
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
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

        public static int SaveConsultationAndGetId(Consultation consultation)
        {
            int newId = 0;

            // Ensure CreatedAt has a valid value
            if (consultation.CreatedAt == DateTime.MinValue || consultation.CreatedAt == default(DateTime))
            {
                consultation.CreatedAt = DateTime.Now;
            }
            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                string sql = @"
            INSERT INTO Consultations 
            (PatientID, DoctorID, VisitReason, Diagnosis, Vitals, LabSummary, FollowUpRequired, CreatedAt, IsCompleted, DepartureTime)
            VALUES 
            (@PatientID, @DoctorID, @VisitReason, @Diagnosis, @Vitals, @LabSummary, @FollowUpRequired, @CreatedAt, @IsCompleted, @DepartureTime);
            SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PatientID", consultation.PatientID);
                    cmd.Parameters.AddWithValue("@DoctorID", consultation.DoctorID);
                    cmd.Parameters.AddWithValue("@VisitReason", consultation.VisitReason);
                    cmd.Parameters.AddWithValue("@Diagnosis", consultation.Diagnosis);
                    cmd.Parameters.AddWithValue("@Vitals", consultation.Vitals);
                    cmd.Parameters.AddWithValue("@LabSummary", (object)consultation.LabSummary ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FollowUpRequired", consultation.FollowUpRequired);
                    cmd.Parameters.AddWithValue("@CreatedAt", consultation.CreatedAt);
                    cmd.Parameters.AddWithValue("@IsCompleted", consultation.IsCompleted);
                    cmd.Parameters.AddWithValue("@DepartureTime", (object)consultation.DepartureTime ?? DBNull.Value);

                    // ExecuteScalar returns the first column of the first row in the result set
                    newId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return newId;
        }

        public static List<dynamic> GetCompletedConsultationsForDoctorToday(int doctorId)
        {
            var consultations = new List<dynamic>();
            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                string query = @"
            SELECT c.ConsultationID, c.PatientID, c.DoctorID, c.DepartureTime,
                   p.FirstName AS PatientFirstName, p.LastName AS PatientLastName,
                   u.FirstName AS DoctorFirstName, u.LastName AS DoctorLastName
            FROM Consultations c
            INNER JOIN Patients p ON c.PatientID = p.PatientID
            INNER JOIN Users u ON c.DoctorID = u.UserID
            WHERE c.DoctorID = @DoctorID AND c.IsCompleted = 1
              AND DATE(c.DepartureTime) = CURDATE()
            ORDER BY c.DepartureTime DESC";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            consultations.Add(new
                            {
                                ConsultationID = reader.GetInt32("ConsultationID"),
                                DoctorName = $"{reader["DoctorFirstName"]} {reader["DoctorLastName"]}",
                                PatientName = $"{reader["PatientFirstName"]} {reader["PatientLastName"]}",
                                ArrivalTime = reader["DepartureTime"] is DBNull ? "" : ((DateTime)reader["DepartureTime"]).ToShortTimeString(),
                                DepartureTime = reader["DepartureTime"] is DBNull ? "" : ((DateTime)reader["DepartureTime"]).ToString("g")
                            });
                        }
                    }
                }
            }
            return consultations;
        }


        public static List<Consultation> GetPendingConsultationsForDoctor(int doctorId)
        {
            var pending = new List<Consultation>();
            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                string query = @"
            SELECT c.ConsultationID, c.PatientID, c.DoctorID, c.VisitReason, c.Diagnosis, c.Vitals, c.LabSummary,
                   c.FollowUpRequired, c.CreatedAt, c.IsCompleted, c.DepartureTime
            FROM Consultations c
            WHERE c.DoctorID = @DoctorID AND c.IsCompleted = 0
            ORDER BY c.CreatedAt DESC";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pending.Add(new Consultation
                            {
                                ConsultationID = reader.GetInt32("ConsultationID"),
                                PatientID = reader.GetInt32("PatientID"),
                                DoctorID = reader.GetInt32("DoctorID"),
                                VisitReason = reader["VisitReason"]?.ToString(),
                                Diagnosis = reader["Diagnosis"]?.ToString(),
                                Vitals = reader["Vitals"]?.ToString(),
                                LabSummary = reader["LabSummary"]?.ToString(),
                                FollowUpRequired = reader.GetBoolean(reader.GetOrdinal("FollowUpRequired")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                                DepartureTime = reader.IsDBNull(reader.GetOrdinal("DepartureTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DepartureTime"))
                            });
                        }
                    }
                }
            }
            return pending;
        }


        public static void SavePrescriptions(List<Prescription> prescriptions)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                foreach (var prescription in prescriptions)
                {
                    using (var command = new MySqlCommand(@"
                INSERT INTO Prescriptions 
                    (ConsultationID, PatientID, DoctorID, MedicationName, Dosage, Instructions, SentToPharmacy)
                VALUES 
                    (@ConsultationID, @PatientID, @DoctorID, @MedicationName, @Dosage, @Instructions, @SentToPharmacy)", connection))
                    {
                        command.Parameters.AddWithValue("@ConsultationID", prescription.ConsultationID);
                        command.Parameters.AddWithValue("@PatientID", prescription.PatientID);
                        command.Parameters.AddWithValue("@DoctorID", prescription.DoctorID);
                        command.Parameters.AddWithValue("@MedicationName", prescription.MedicationName);
                        command.Parameters.AddWithValue("@Dosage", prescription.Dosage);
                        command.Parameters.AddWithValue("@Instructions", prescription.Instructions);
                        command.Parameters.AddWithValue("@SentToPharmacy", false);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public static void SaveLabFile(LabFile labFile)
        {
            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                string query = @"
    INSERT INTO lab_results (PatientID, ConsultationID, FileName, FilePath, UploadedAt, UploadedBy)
    VALUES (@PatientID, @ConsultationID, @FileName, @FilePath, @UploadedAt, @UploadedBy)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PatientID", labFile.PatientID);
                    cmd.Parameters.AddWithValue("@ConsultationID", labFile.ConsultationID);
                    cmd.Parameters.AddWithValue("@FileName", labFile.FileName);
                    // Store only filename, not full path
                    cmd.Parameters.AddWithValue("@FilePath", labFile.FileName);
                    cmd.Parameters.AddWithValue("@UploadedAt", labFile.UploadedAt);
                    cmd.Parameters.AddWithValue("@UploadedBy", labFile.UploadedBy);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static string GetLabFilesDirectory()
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string labFilesFolder = Path.Combine(documentsFolder, "Elizade Clinic", "LabFiles");

            if (!Directory.Exists(labFilesFolder))
                Directory.CreateDirectory(labFilesFolder);

            return labFilesFolder;
        }
        public static string GetLabFileFullPath(string fileName)
        {
            return Path.Combine(GetLabFilesDirectory(), fileName);
        }
        public static List<LabFile> GetLabFilesByPatientId(int patientId)
        {
            var labFiles = new List<LabFile>();
            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                string query = "SELECT FileName, UploadedAt FROM lab_results WHERE PatientId = @PatientId";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        labFiles.Add(new LabFile
                        {
                            FileName = reader.IsDBNull(0) ? null : reader.GetString(0),
                            // FilePath will be the same as FileName since we store only filename
                            FilePath = reader.IsDBNull(0) ? null : reader.GetString(0),
                            UploadedAt = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1)
                        });
                    }
                }
            }
            return labFiles;
        }
        // Get patient with medical alerts
        public static Patient GetPatientWithMedicalAlerts(int patientId)
        {
            Patient patient = null;
            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                string query = "SELECT PatientID, FirstName, LastName, DateOfBirth, Gender, MatricNumber, Phone, Email, MedicalAlerts, CreatedAt FROM Patients WHERE PatientID = @PatientID";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        patient = new Patient
                        {
                            PatientID = reader.GetInt32("PatientID"),
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName"),
                            DateOfBirth = reader.GetDateTime("DateOfBirth"),
                            Gender = reader.GetString("Gender"),
                            MatricNumber = reader.IsDBNull(reader.GetOrdinal("MatricNumber")) ? null : reader.GetString("MatricNumber"),
                            Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString("Phone"),
                            Email = reader.GetString("Email"),
                            MedicalAlerts = reader.IsDBNull(reader.GetOrdinal("MedicalAlerts")) ? null : reader.GetString("MedicalAlerts"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        };
                    }
                }
            }
            return patient;
        }

        // Update patient medical alerts only
        public static bool UpdatePatientMedicalAlerts(int patientId, string medicalAlerts)
        {
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "UPDATE Patients SET MedicalAlerts = @MedicalAlerts WHERE PatientID = @PatientID";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MedicalAlerts", medicalAlerts);
                    cmd.Parameters.AddWithValue("@PatientID", patientId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating medical alerts: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
