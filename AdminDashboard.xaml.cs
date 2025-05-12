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
            //LoadDashboardData(); // Call to load initial data on load

            LoadProfilePicture();
            MainContentControl.Content = new DashboardHomePage();

            // Show the name and email in the UI
            NameTextBlock.Text = fullName;
            EmailTextBlock.Text = email;
        }

        // Method to load initial data
        //private void LoadDashboardData()
        //{
        //    //// Set the welcome message
        //    //SetWelcomeText();

        //    //// Set the current date
        //    //DateTextBlock.Text = DateTime.Now.ToString("dddd MMMM dd, yyyy");
        //}

       
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
                ProfileImage.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\Icons\\user.png"));
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
                MainContentControl.Content = new DashboardHomePage();
            }
            else if (sender == StaffButton)
            {
                StaffButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                DoctorText.Foreground = Brushes.White;
                DoctorIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\Source\\Repos\\Elizade-Clinic-EHR\\white icons\\icons8-doctor-50.png"));
                // ✅ Load the StaffPage into the main content area
                MainContentControl.Content = new StaffPage();
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
            StaffButton.Background = Brushes.White;
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
        private void StaffsButton_Click(object sender, RoutedEventArgs e)
        {
            // Load the StaffPage UserControl into a container (like a ContentControl or Grid)
            StaffPage staffPage = new StaffPage();
            MainContentControl.Content = staffPage; // Make sure you have this ContentControl in your XAML
        }

    }
}
