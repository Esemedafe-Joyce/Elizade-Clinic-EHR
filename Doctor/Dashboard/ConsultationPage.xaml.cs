using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ElizadeEHR.Helpers;
using Microsoft.Win32;

namespace ElizadeEHR.Doctor
{
    /// <summary>
    /// Interaction logic for ConsultationPage.xaml
    /// </summary>
    public partial class ConsultationPage : UserControl
    {
        private Patient _selectedPatient;
        private Consultation _currentConsultation;
        private int _currentConsultationId;
        private string _pendingLabFileName = null;
        private string _pendingLabFilePath = null;

        private string _originalMedicalAlerts;
        private bool _medicalAlertsChanged = false;

        public ObservableCollection<Prescription> Prescriptions { get; set; } = new ObservableCollection<Prescription>();

        public ConsultationPage(Patient selectedPatient)
        {
            InitializeComponent();
            _selectedPatient = selectedPatient;

            this.DataContext = this;
            Prescriptions.Add(new Prescription());

            // Load patient data with medical alerts
            LoadPatientData();
        }

        public ConsultationPage(Patient selectedPatient, Consultation consultationToEdit)
        {
            InitializeComponent();
            _selectedPatient = selectedPatient;
            _currentConsultation = consultationToEdit;
            _currentConsultationId = consultationToEdit.ConsultationID;

            this.DataContext = this;

            // Load patient data first
            LoadPatientData();

            // Populate UI fields with consultationToEdit data
            VisitReasonTextBox.Text = consultationToEdit.VisitReason;
            // Populate other fields as needed...
        }

        private void LoadPatientData()
        {
            // Get complete patient data including medical alerts
            var patientWithAlerts = DatabaseHelper.GetPatientWithMedicalAlerts(_selectedPatient.PatientID);
            if (patientWithAlerts != null)
            {
                _selectedPatient = patientWithAlerts;
            }

            // Populate basic patient info
            PatientFullNameTextBlock.Text = $"{_selectedPatient.FirstName} {_selectedPatient.LastName}";
            PatientDOBTextBlock.Text = _selectedPatient.DateOfBirth.ToString("MMMM dd, yyyy");
            PatientGenderTextBlock.Text = _selectedPatient.Gender;
            VisitDateTextBlock.Text = DateTime.Now.ToShortDateString();

            // Load medical alerts
            _originalMedicalAlerts = _selectedPatient.MedicalAlerts ?? string.Empty;
            MedicalAlertsTextBox.Text = _originalMedicalAlerts;

            // Update status
            UpdateMedicalAlertsStatus();
        }

        private void MedicalAlertsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _medicalAlertsChanged = MedicalAlertsTextBox.Text.Trim() != _originalMedicalAlerts;
            UpdateMedicalAlertsStatus();
        }

        private void UpdateMedicalAlertsStatus()
        {
            if (_medicalAlertsChanged)
            {
                MedicalAlertsStatusTextBlock.Text = "Medical flags modified - will be saved to patient profile";
                MedicalAlertsStatusTextBlock.Foreground = new SolidColorBrush(Colors.Orange);
            }
            else
            {
                MedicalAlertsStatusTextBlock.Text = "Changes will be saved to patient profile";
                MedicalAlertsStatusTextBlock.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("No patient selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*",
                Title = "Select Lab Result File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string sourcePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(sourcePath);
                    string saveDirectory = GetLabFilesDirectory();
                    string destinationPath = Path.Combine(saveDirectory, fileName);

                    // Copy file to destination
                    File.Copy(sourcePath, destinationPath, true);

                    // Store file info for later database save
                    _pendingLabFileName = fileName;
                    _pendingLabFilePath = fileName; // or full path if needed

                    // Update UI
                    UploadedFileNameTextBlock.Text = $"Ready to save: {fileName}";

                    DatabaseHelper.LogAction(App.UserID, "Selected Lab File for upload");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading file: {ex.Message}", "Upload Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static string GetLabFilesDirectory()
        {
            string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string labFilesFolder = System.IO.Path.Combine(baseFolder, "Elizade Clinic", "LabFiles");

            if (!Directory.Exists(labFilesFolder))
                Directory.CreateDirectory(labFilesFolder);

            return labFilesFolder;
        }

        private static readonly Regex _allowedInput = new Regex(@"^[0-9./\- ]+$");

        private void VitalsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_allowedInput.IsMatch(e.Text);
        }

        private void VitalsTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right)
                e.Handled = false;
        }

        private void VitalsTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!_allowedInput.IsMatch(text))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveConsultation(isCompleted: true);
        }

        private void SaveProgressButton_Click(object sender, RoutedEventArgs e)
        {
            SaveConsultation(isCompleted: false);
        }

        private void SaveConsultation(bool isCompleted)
        {
            // Validation for completed consultation
            if (isCompleted)
            {
                string visitReason = VisitReasonTextBox.Text.Trim();
                string diagnosis = new TextRange(DiagnosisBox.Document.ContentStart, DiagnosisBox.Document.ContentEnd).Text.Trim();
                string temp = TempTextBox.Text.Trim();
                string bp = BpTextBox.Text.Trim();
                string weight = WeightTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(visitReason))
                {
                    MessageBox.Show("Visit reason is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(diagnosis))
                {
                    MessageBox.Show("Diagnosis is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(temp))
                {
                    MessageBox.Show("Temperature is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(bp))
                {
                    MessageBox.Show("Blood Pressure is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(weight))
                {
                    MessageBox.Show("Weight is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var validPrescriptions = PrescriptionDataGrid.Items.OfType<Prescription>()
                    .Where(p =>
                        !string.IsNullOrWhiteSpace(p.MedicationName) &&
                        !string.IsNullOrWhiteSpace(p.Dosage) &&
                        !string.IsNullOrWhiteSpace(p.Instructions))
                    .ToList();

                if (validPrescriptions.Count == 0)
                {
                    MessageBox.Show("At least one prescription with all fields filled is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                // Save medical alerts if changed
                if (_medicalAlertsChanged)
                {
                    string newMedicalAlerts = MedicalAlertsTextBox.Text.Trim();
                    bool alertsUpdated = DatabaseHelper.UpdatePatientMedicalAlerts(_selectedPatient.PatientID, newMedicalAlerts);

                    if (alertsUpdated)
                    {
                        _selectedPatient.MedicalAlerts = newMedicalAlerts;
                        _originalMedicalAlerts = newMedicalAlerts;
                        _medicalAlertsChanged = false;
                        UpdateMedicalAlertsStatus();

                        DatabaseHelper.LogAction(App.UserID, $"Updated medical flags for {_selectedPatient.FirstName} {_selectedPatient.LastName}");
                    }
                    else
                    {
                        MessageBox.Show("Failed to update medical flags. Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                // Build the consultation object
                var consultation = new Consultation
                {
                    PatientID = _selectedPatient.PatientID,
                    DoctorID = App.UserID,
                    VisitReason = VisitReasonTextBox.Text.Trim(),
                    Diagnosis = new TextRange(DiagnosisBox.Document.ContentStart, DiagnosisBox.Document.ContentEnd).Text.Trim(),
                    TreatmentPlan = TreatmentPlanBox.Text.Trim(),
                    Vitals = $"Temp: {TempTextBox.Text.Trim()}, BP: {BpTextBox.Text.Trim()}, Weight: {WeightTextBox.Text.Trim()}",
                    LabSummary = !string.IsNullOrEmpty(_pendingLabFileName) ? $"Lab file: {_pendingLabFileName}" : null,
                    FollowUpRequired = FollowUpCheckBox.IsChecked == true,
                    CreatedAt = DateTime.Now,
                };

                if (isCompleted)
                {
                    consultation.IsCompleted = true;
                    consultation.DepartureTime = DateTime.Now;
                }
                else
                {
                    consultation.IsCompleted = false;
                    consultation.DepartureTime = null;
                }

                // Save consultation and get ID
                int consultationId = DatabaseHelper.SaveConsultationAndGetId(consultation);

                // NOW save the lab file with the consultation ID
                if (!string.IsNullOrEmpty(_pendingLabFileName))
                {
                    DatabaseHelper.SaveLabFile(new LabFile
                    {
                        FileName = _pendingLabFileName,
                        FilePath = _pendingLabFilePath,
                        PatientID = _selectedPatient.PatientID,
                        ConsultationID = consultationId, // Now we have the ID!
                        UploadedBy = App.UserID,
                        UploadedAt = DateTime.Now
                    });

                    // Update UI to show it's actually saved
                    UploadedFileNameTextBlock.Text = $"Uploaded: {_pendingLabFileName}";
                    DatabaseHelper.LogAction(App.UserID, "Saved Lab File to database");
                }

                // Save prescriptions (existing code)
                var prescriptions = PrescriptionDataGrid.Items.OfType<Prescription>()
                    .Where(p =>
                        !string.IsNullOrWhiteSpace(p.MedicationName) &&
                        !string.IsNullOrWhiteSpace(p.Dosage) &&
                        !string.IsNullOrWhiteSpace(p.Instructions))
                    .ToList();

                foreach (var prescription in prescriptions)
                {
                    //prescription.PrescriptionID = prescription.PrescriptionID;
                    prescription.ConsultationID = consultationId;
                    prescription.PatientID = _selectedPatient.PatientID;
                    prescription.DoctorID = App.UserID;
                    prescription.DatePrescribed = DateTime.Now;
                }

                if (prescriptions.Count > 0)
                {
                    DatabaseHelper.SavePrescriptions(prescriptions);
                }

                MessageBox.Show(isCompleted
                    ? "Consultation completed successfully."
                    : "Progress saved. You can complete the consultation later.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                DatabaseHelper.LogAction(App.UserID, $"{(isCompleted ? "Completed" : "Saved progress for")} consultation with {_selectedPatient.FirstName} {_selectedPatient.LastName}");

                // Return to Doctor Dashboard homepage
                var parentWindow = Window.GetWindow(this) as DoctorDashboard;
                if (parentWindow != null)
                {
                    parentWindow.MainContentControl.Content = new DoctorDashboardHomePage(parentWindow);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving consultation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPendingConsultation_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is int consultationId)
            {
                MessageBox.Show($"Edit consultation with ID: {consultationId}", "Edit Consultation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}