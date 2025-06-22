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
using ElizadeEHR.Doctor.Records;
using ElizadeEHR.Helpers;

namespace ElizadeEHR.Doctor
{
    /// <summary>
    /// Interaction logic for ReadOnlyConsultationPage.xaml
    /// </summary>
    public partial class ReadOnlyConsultationPage : UserControl
    {
        private Patient _selectedPatient;
        private Consultation _consultation;
        private string _originalMedicalAlerts;

        public ReadOnlyConsultationPage(Patient patient, Consultation consultation)
        {
            InitializeComponent();
            _selectedPatient = patient;
            _consultation = consultation;
            PopulateFields();
        }
        private void PopulateFields()
        {
            // Patient info
            PatientFullNameTextBlock.Text = $"{_selectedPatient.FirstName} {_selectedPatient.LastName}";
            PatientDOBTextBlock.Text = _selectedPatient.DateOfBirth.ToString("dd MMM yyyy");
            PatientGenderTextBlock.Text = _selectedPatient.Gender;
            MedicalAlertsTextBox.Text = _selectedPatient.MedicalAlerts;

            // Consultation info
            VisitDateTextBlock.Text = _consultation.CreatedAt.ToString("dd MMM yyyy");

            // If you have other fields like Diagnosis, Vitals, etc., bind them here as well once their controls are confirmed
            VisitReasonTextBox.Text = _consultation.VisitReason.ToString();
            DiagnosisBox.Text = _consultation.Diagnosis.ToString();
            TreatmentPlanBox.Text = _consultation.TreatmentPlan.ToString();
        }

        public void GoBack()
        {
            var parentWindow = Window.GetWindow(this) as DoctorDashboard;
            if (parentWindow != null)
            {
                // Navigate back to patient details page with the patient ID
                var patientDetailsPage = new PatientRecordsDetailPage(_selectedPatient.PatientID);
                parentWindow.MainContentControl.Content = patientDetailsPage;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }
    }
}
