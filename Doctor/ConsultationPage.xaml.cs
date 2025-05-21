using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public ConsultationPage(Patient selectedPatient)
        {
            InitializeComponent();
            _selectedPatient = selectedPatient;

            // Populate UI fields manually
            PatientFullNameTextBlock.Text = $"{_selectedPatient.FirstName} {_selectedPatient.LastName}";
            PatientDOBTextBlock.Text = _selectedPatient.DateOfBirth.ToString("MMMM dd, yyyy");
            PatientGenderTextBlock.Text = _selectedPatient.Gender;
            VisitDateTextBlock.Text = DateTime.Now.ToShortDateString();
            
        }


        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*",
                Title = "Select Lab Result File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string sourcePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(sourcePath);
                string destinationDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LabFiles");
                Directory.CreateDirectory(destinationDir); // ensure directory exists

                string destinationPath = System.IO.Path.Combine(destinationDir, fileName);
                File.Copy(sourcePath, destinationPath, overwrite: true);

                // Show file name in UI
                UploadedFileNameTextBlock.Text = $"Uploaded: {fileName}";

                // Create a new Consultation record
                var newConsultation = new Consultation
                {
                    PatientID = _selectedPatient.PatientID,
                    DoctorID = App.UserID, // Assuming current user is a doctor
                    VisitReason = "Lab result upload", // Replace as needed
                    Diagnosis = "", // Can be filled later
                    Vitals = "",    // Can be filled later
                    LabSummary = $"Uploaded file: {fileName}",
                    FollowUpRequired = false
                };

                // Save and get the ConsultationID
                int currentConsultationId = DatabaseHelper.SaveConsultationAndGetId(newConsultation);

                // Save lab result to DB
                DatabaseHelper.SaveLabFile(new LabFile
                {
                    FileName = fileName,
                    FilePath = destinationPath,
                    PatientID = _selectedPatient.PatientID,
                    ConsultationID = currentConsultationId,
                    UploadedBy = App.UserID
                });
            }
        }

        private static readonly Regex _allowedInput = new Regex(@"^[0-9./\- ]+$");

        private void VitalsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_allowedInput.IsMatch(e.Text);
        }

        private void VitalsTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Allow common keys like Backspace, Delete, Tab, etc.
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

        }
    }
}
