using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace ElizadeEHR
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
            // Set the user email and profile picture after login
            if (!string.IsNullOrEmpty(App.UserEmail))
            {
                // Display email
                EmailTextBlock.Text = App.UserEmail;

                // Display full name (already combined in App.UserName)
                NameTextBlock.Text = App.UserName;

                // Display profile picture (if it exists)
                if (!string.IsNullOrEmpty(App.ProfilePicturePath))
                {
                    ProfileImage.Source = new BitmapImage(new Uri(App.ProfilePicturePath));
                }
            }

            // Set the current date
            DateTextBlock.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

            // Load the total patient count
            LoadTotalPatients();
            LoadDashboardData();
        }

        // Load the total number of patients from the database
        private void LoadTotalPatients()
        {
            int totalPatients = GetTotalPatientsFromDatabase();
            TotalPatientsTextBlock.Text = "Total Patients: " + totalPatients;
        }

        // Fetch total number of patients from the database
        private int GetTotalPatientsFromDatabase()
        {
            int patientCount = 0;
            string query = "SELECT COUNT(*) FROM Patients";  // Assuming you have a 'Patients' table

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    patientCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching patient count: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return patientCount;
        }

        private void LoadDashboardData()
        {
            // Example for fetching and displaying available doctors count
            string availableDoctors = GetAvailableDoctorsCount(); // Fetch from the backend
            AvailableDoctorsTextBlock.Text = "Total Doctors: " + availableDoctors;
        }

        private string GetAvailableDoctorsCount()
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString)) 
            {
                try
                {
                    conn.Open();
                    // Query to count doctors with role 'Doctor' (assuming availability is defined in the role itself or another flag)
                    string query = "SELECT COUNT(*) FROM Users WHERE Role = 'Doctor'"; // Example query for doctors
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        return cmd.ExecuteScalar().ToString(); // Returns the count of doctors
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching doctor count: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return "0"; // In case of error, return 0
                }
            }
        }



        private void ProfileImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Profile Picture",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                ProfileImage.Source = new BitmapImage(new Uri(selectedFilePath));

                // Update the Profile Picture in the database
                UpdateProfilePicture(selectedFilePath);
            }
        }

        private void UpdateProfilePicture(string filePath)
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE Users SET ProfilePicture = @ProfilePicture WHERE Email = @Email";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProfilePicture", filePath);
                        cmd.Parameters.AddWithValue("@Email", App.UserEmail);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating profile picture: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ProfileSection_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Logout Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Add any logic for button click if needed
        }

       

        internal class DatabaseConfig
        {
            public static string ConnectionString = "server=localhost;database=campusehrconsole;user=root;password=joycedafe3225%;";
        }

    }
}
