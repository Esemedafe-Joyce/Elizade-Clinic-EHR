using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MySqlConnector;
using System.Windows.Media;
using System.Windows.Data;
using System.IO;
using System.Collections.Generic;
using ElizadeEHR.Helpers;

namespace ElizadeEHR
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard(string fullName, string email)
        {
            InitializeComponent();
            LoadDashboardData(); // Call to load initial data on load

            LoadProfilePicture();

            // Show the name and email in the UI
            NameTextBlock.Text = fullName;
            EmailTextBlock.Text = email;
        }

        // Method to load initial data
        private void LoadDashboardData()
        {
            // Set the welcome message
            SetWelcomeText();

            // Set the current date
            DateTextBlock.Text = DateTime.Now.ToString("dddd MMMM dd, yyyy");
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

        private void ProfileSection_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Logout Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void LoadProfilePicture()
        {
            if (!string.IsNullOrEmpty(App.ProfilePicturePath) && File.Exists(App.ProfilePicturePath))
            {
                ProfileImage.Source = new BitmapImage(new Uri(App.ProfilePicturePath));
            }
            else
            {
                // Optional: set a default profile image
                ProfileImage.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/default-avatar.png"));
            }
        }


        //private Button selectedButton;

        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            ResetSidebarSelection();

            if (sender == DashboardButton)
            {
                DashboardButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                DashboardText.Foreground = Brushes.White;
                DashboardIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\Source\\Repos\\Elizade-Clinic-EHR\\white icons\\icons8-dashboard-24.png"));
            }
            else if (sender == DoctorButton)
            {
                DoctorButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                DoctorText.Foreground = Brushes.White;
                DoctorIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\Source\\Repos\\Elizade-Clinic-EHR\\white icons\\icons8-doctor-50.png"));
            }
            else if (sender == PatientButton)
            {
                PatientButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                PatientText.Foreground = Brushes.White;
                PatientIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\Source\\Repos\\Elizade-Clinic-EHR\\white icons\\icons8-patient-50.png"));
            }
            else if (sender == SettingsButton)
            {
                SettingsButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                SettingsText.Foreground = Brushes.White;
                SettingsIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\Source\\Repos\\Elizade-Clinic-EHR\\white icons\\icons8-settings-48.png"));
            }
            else if (sender == SupportButton)
            {
                SupportButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                SupportText.Foreground = Brushes.White;
                SupportIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\Source\\Repos\\Elizade-Clinic-EHR\\white icons\\icons8-customer-service-50 (1).png"));
            }
        }

        private void ResetSidebarSelection()
        {
            // Reset Dashboard
            DashboardButton.Background = Brushes.White;
            DashboardText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            DashboardIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/icons8-dashboard-48.png"));

            // Reset Doctor
            DoctorButton.Background = Brushes.White;
            DoctorText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            DoctorIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/icons8-doctor-50.png"));

            // Reset Patient
            PatientButton.Background = Brushes.White;
            PatientText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            PatientIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/icons8-patient-50.png"));

            // Reset Settings
            SettingsButton.Background = Brushes.White;
            SettingsText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            SettingsIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/icons8-settings-50.png"));

            // Reset Support
            SupportButton.Background = Brushes.White;
            SupportText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            SupportIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/icons8-support-50.png"));
        }

        // Set the welcome text after fetching last name
        private void SetWelcomeText()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT LastName FROM Users WHERE Email = @Email";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", App.UserEmail);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string lastName = reader["LastName"].ToString();
                                WelcomeTextBlock.Text = $"Welcome {lastName}";
                            }
                            else
                            {
                                WelcomeTextBlock.Text = "Welcome";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading welcome message: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                WelcomeTextBlock.Text = "Welcome";
            }
        }

        // Event handler for Total Patients Button click
        // Event handler for TotalPatientsButton click
        private void TotalPatientsButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset other buttons and set the clicked button's style
            ResetDashboardButtonSelection();

            // Change the background and text color of the clicked button
            TotalPatientsButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            TotalPatientsTextBlock.Foreground = Brushes.White;

            // Update the DataGrid to show patient data
            DashboardDataGrid.Columns.Clear();

            // Make the DataGrid visible
            DashboardDataGrid.Visibility = Visibility.Visible;

            // Add columns for patient data
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "PatientID", Binding = new Binding("PatientID") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "LASTNAME", Binding = new Binding("LastName") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "FIRSTNAME", Binding = new Binding("FirstName") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "DATE OF BIRTH", Binding = new Binding("DateOfBirth") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "GENDER", Binding = new Binding("Gender") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "PHONE", Binding = new Binding("Phone") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "EMAIL", Binding = new Binding("Email") });

            // Load patient data from the database
            DashboardDataGrid.ItemsSource = DatabaseHelper.GetAllPatients(); // Use your actual data fetching method
        }

        // Event handler for TotalDoctorsButton click
        private void TotalDoctorsButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset other buttons and set the clicked button's style
            ResetDashboardButtonSelection();

            // Change the background and text color of the clicked button
            TotalDoctorsButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            AvailableDoctorsTextBlock.Foreground = Brushes.White;

            // Update the DataGrid to show doctor data
            DashboardDataGrid.Columns.Clear();

            // Make the DataGrid visible
            DashboardDataGrid.Visibility = Visibility.Visible;

            // Add columns for doctor data
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "UserID", Binding = new Binding("UserID") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "LASTNAME", Binding = new Binding("LastName") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "FIRSTNAME", Binding = new Binding("FirstName") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "GENDER", Binding = new Binding("Gender") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "PHONE", Binding = new Binding("Phone") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "EMAIL", Binding = new Binding("Email") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "ROLE", Binding = new Binding("Role") });

            // Load doctor data from the database
            DashboardDataGrid.ItemsSource = DatabaseHelper.GetAllUsers(); // Use your actual data fetching method
        }

        // Event handler for ReportButton click
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            ResetDashboardButtonSelection();
            ReportButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            ReportTextBlock.Foreground = Brushes.White;

            // Show and populate the grid
            DashboardDataGrid.Visibility = Visibility.Visible;
            DashboardDataGrid.Columns.Clear();

            DashboardDataGrid.Columns.Add(
              new DataGridTextColumn { Header = "Log ID", Binding = new Binding("LogID") });
            DashboardDataGrid.Columns.Add(
              new DataGridTextColumn { Header = "User", Binding = new Binding("UserFullName") });
            DashboardDataGrid.Columns.Add(
              new DataGridTextColumn { Header = "Action", Binding = new Binding("Action") });
            DashboardDataGrid.Columns.Add(
              new DataGridTextColumn { Header = "Timestamp", Binding = new Binding("Timestamp") });

            DashboardDataGrid.ItemsSource = DatabaseHelper.GetAllAuditLogs();

            // Log that the report was viewed
            DatabaseHelper.LogAction(App.UserID, "Viewed audit log report");
        }


        // Reset the background and text color for all buttons
        private void ResetDashboardButtonSelection()
        {
            // Reset TotalPatientsButton
            TotalPatientsButton.Background = Brushes.White;
            TotalPatientsTextBlock.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");

            // Reset TotalDoctorsButton
            TotalDoctorsButton.Background = Brushes.White;
            AvailableDoctorsTextBlock.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");

            // Reset ReportButton
            ReportButton.Background = Brushes.White;
            ReportTextBlock.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
        }


    }
}
