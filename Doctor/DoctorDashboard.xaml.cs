using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ElizadeEHR.Doctor;
using ElizadeEHR.Helpers;
using Microsoft.Win32;
using MySqlConnector;

namespace ElizadeEHR
{
    /// <summary>
    /// Interaction logic for DoctorDashboard.xaml
    /// </summary>
    public partial class DoctorDashboard : Window
    {
        //private Patient selectedPatient;

        public DoctorDashboard(string fullName, string email)
        {
            InitializeComponent();
            //LoadDashboardData(); // Call to load initial data on load

            LoadProfilePicture();
            MainContentControl.Content = new DoctorDashboardHomePage(this);
 


            //// Show the name and email in the UI
            NameTextBlock.Text = fullName;
            EmailTextBlock.Text = email;

            //var consultPage = new Doctor.ConsultationPage(selectedPatient);
            //consultPage.OnConsultationFinished += () =>
            //{
            //    MainContentControl.Content = new DashboardHomePage();
            //};
            //MainContentControl.Content = consultPage;

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
                ProfileImage.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\Icons\\user.png"));
            }
        }
        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            ResetSidebarSelection();

            if (sender == DashboardButton)
            {
                DashboardButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                DashboardText.Foreground = Brushes.White;
                DashboardIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\Source\\Repos\\Elizade-Clinic-EHR\\white icons\\icons8-dashboard-24.png"));
                MainContentControl.Content = new DoctorDashboardHomePage();
            }
            //else if (sender == PatientQueueButton)
            //{
            //    PatientQueueButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            //    PatientQueueText.Foreground = Brushes.White;
            //    QueueIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\white icons\\icons8-queue-50 (1).png"));

            //}
            else if (sender == PatientRecordsButton)
            {
                PatientRecordsButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                PatientRecordsText.Foreground = Brushes.White;
                RecordsIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\white icons\\icons8-records-50 (1).png"));
                MainContentControl.Content = new PatientRecordsPage();
            }
            else if (sender == ConsultationNotesButton)
            {
                ConsultationNotesButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                ConsultationNotesText.Foreground = Brushes.White;
                NotesIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\white icons\\icons8-notes-50 (1).png"));

            }
            else if (sender == TreatmentPlanButton)
            {
                TreatmentPlanButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                TreatmentPlanText.Foreground = Brushes.White;
                PlanIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\white icons\\icons8-treatment-50 (1).png"));

            }
            else if (sender == SettingsButton)
            {
                SettingsButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
                SettingsText.Foreground = Brushes.White;
                SettingsIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\Source\\Repos\\Elizade-Clinic-EHR\\white icons\\icons8-settings-48.png"));
            }
        }

        private void ResetSidebarSelection()
        {
            // Reset Dashboard
            DashboardButton.Background = Brushes.White;
            DashboardText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            DashboardIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\Icons\\icons8-dashboard-48.png"));

            //PatientQueueButton.Background = Brushes.White;
            //PatientQueueText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            //QueueIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\Icons\\icons8-queue-50.png"));

            PatientRecordsButton.Background = Brushes.White;
            PatientRecordsText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            RecordsIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\Icons\\icons8-records-50.png"));

            ConsultationNotesButton.Background = Brushes.White;
            ConsultationNotesText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            NotesIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\Icons\\icons8-notes-50.png"));

            TreatmentPlanButton.Background = Brushes.White;
            TreatmentPlanText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            PlanIcon.Source = new BitmapImage(new Uri("C:\\Users\\Joyce\\source\\repos\\Elizade-Clinic-EHR\\Icons\\icons8-treatment-50.png"));

            // Reset Settings
            SettingsButton.Background = Brushes.White;
            SettingsText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
            SettingsIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/icons8-settings-50.png"));
        }

        public void LoadConsultationPage(Patient selectedPatient)
        {
            MainContentControl.Content = new Doctor.ConsultationPage(selectedPatient);
        }

    }
}
