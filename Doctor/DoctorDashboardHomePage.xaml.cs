using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ElizadeEHR.Doctor;
using ElizadeEHR.Helpers;
using MySqlConnector;

namespace ElizadeEHR
{
    /// <summary>
    /// Interaction logic for DoctorDashboardHomePage.xaml
    /// </summary>
    public partial class DoctorDashboardHomePage : UserControl
    {
        public DoctorDashboardHomePage()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetWelcomeText();
            DateTextBlock.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy"); // Example: Monday, 08 May 2025
        }
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
                                WelcomeTextBlock.Text = $"Welcome Dr.{lastName}";
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

        private DoctorDashboard _dashboard;

        public DoctorDashboardHomePage(DoctorDashboard dashboard)
        {
            InitializeComponent();
            _dashboard = dashboard;
        }


        private void StartConsultationButton_Click(object sender, RoutedEventArgs e)
        {
            var patientWindow = new PatientSelectionWindow();
            if (patientWindow.ShowDialog() == true && patientWindow.SelectedPatient != null)
            {
                var selectedPatient = patientWindow.SelectedPatient;
                _dashboard.LoadConsultationPage(selectedPatient); // Assuming you pass the patient object
            }

        }
        private void PendingNotesButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void PatientsSeenTodayButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
