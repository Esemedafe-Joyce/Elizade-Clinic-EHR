using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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
            // Change the background and text color of the clicked button
            StartConsultationButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            StartConsultationText.Foreground = Brushes.White;
            ResetDashboardButtonSelection();

            var patientWindow = new PatientSelectionWindow();
            if (patientWindow.ShowDialog() == true && patientWindow.SelectedPatient != null)
            {
                var selectedPatient = patientWindow.SelectedPatient;
                _dashboard.LoadConsultationPage(selectedPatient); // Assuming you pass the patient object
            }

        }
        private void PendingNotesButton_Click(object sender, RoutedEventArgs e)
        {
            PendingNotesButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            PendingNotesText.Foreground = Brushes.White;
            ResetDashboardButtonSelection();

            // Fetch pending consultations
            var pendingNotes = DatabaseHelper.GetPendingConsultationsForDoctor(App.UserID);

            // Project to display-friendly format
            var patients = DatabaseHelper.GetAllPatients();
            var displayList = pendingNotes.Select(c => new
            {
                c.ConsultationID,
                DoctorName = "Dr. " + (App.UserName ?? ""),
                PatientName = patients.FirstOrDefault(p => p.PatientID == c.PatientID) is var pat && pat != null
                    ? $"{pat.FirstName} {pat.LastName}" : "Unknown",
                ArrivalTime = c.CreatedAt.ToShortTimeString(),
                DepartureTime = "", // Not completed
            }).ToList();

            DoctorDataGrid.ItemsSource = displayList;
            DoctorDataGrid.Visibility = Visibility.Visible;
        }
        private void PatientsSeenTodayButton_Click(object sender, RoutedEventArgs e)
        {
            // Change the background and text color of the clicked button
            PatientsSeenTodayButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            PatientsSeenTodayText.Foreground = Brushes.White;
            ResetDashboardButtonSelection();

            // Fetch completed consultations for today
            var seenToday = DatabaseHelper.GetCompletedConsultationsForDoctorToday(App.UserID);

            // Bind to DataGrid
            DoctorDataGrid.ItemsSource = seenToday;
            DoctorDataGrid.Visibility = Visibility.Visible;
        }

        private void ResetDashboardButtonSelection()
        {
            // Reset StartConsultationButton
            StartConsultationButton.Background = Brushes.White;
            StartConsultationText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");

            // Reset PendingNotesButton
            PendingNotesButton.Background = Brushes.White;
            PendingNotesText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");

            // Reset PatientsSeenTodayButton
            PatientsSeenTodayButton.Background = Brushes.White;
            PatientsSeenTodayText.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
        }

        private void EditPendingConsultation_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int consultationId)
            {
                var consultation = DatabaseHelper.GetPendingConsultationsForDoctor(App.UserID)
                    .FirstOrDefault(c => c.ConsultationID == consultationId);
                var patient = DatabaseHelper.GetAllPatients()
                    .FirstOrDefault(p => p.PatientID == consultation?.PatientID);

                if (consultation != null && patient != null)
                {
                    var parentWindow = Window.GetWindow(this) as DoctorDashboard;
                    if (parentWindow != null)
                    {
                        parentWindow.MainContentControl.Content = new Doctor.ConsultationPage(patient, consultation);
                    }
                }
            }
        }
    }
}
