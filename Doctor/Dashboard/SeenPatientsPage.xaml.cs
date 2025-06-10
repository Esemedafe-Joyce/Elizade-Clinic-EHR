using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ElizadeEHR.Doctor
{
    public partial class SeenPatientsPage : UserControl
    {
        public SeenPatientsPage()
        {
            InitializeComponent();
            LoadSeenPatients();
        }

        private void LoadSeenPatients()
        {
            var consultations = DatabaseHelper.GetCompletedConsultationsForDoctor(App.UserID);

            // Project to anonymous type for display (add PatientName)
            var displayList = consultations.Select(c => new
            {
                c.ConsultationID,
                PatientName = GetPatientName(c.PatientID),
                c.VisitReason,
                c.Diagnosis,
                c.DepartureTime
            }).ToList();

            SeenPatientsDataGrid.ItemsSource = displayList;
        }

        private string GetPatientName(int patientId)
        {
            // You may want to optimize this by caching or joining in SQL
            var patient = DatabaseHelper.GetAllPatients().FirstOrDefault(p => p.PatientID == patientId);
            return patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown";
        }

        private void PatientsSeenTodayButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this) as DoctorDashboard;
            if (parentWindow != null)
            {
                parentWindow.MainContentControl.Content = new Doctor.SeenPatientsPage();
            }
        }
    }
}