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
        public static List<Doctor> GetAllDoctors()
        {
            List<Doctor> doctors = new List<Doctor>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT UserID AS DoctorID, LastName, FirstName, Gender, Phone, Email
            FROM Users
            WHERE Role = 'Doctor'";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    doctors.Add(new Doctor
                    {
                        DoctorID = reader.GetInt32("DoctorID"),
                        LastName = reader.GetString("LastName"),
                        FirstName = reader.GetString("FirstName"),
                        Gender = reader.GetString("Gender"),
                        Phone = reader.GetString("Phone"),
                        Email = reader.GetString("Email")
                    });
                }
            }
            return doctors;
        }

    }
}
