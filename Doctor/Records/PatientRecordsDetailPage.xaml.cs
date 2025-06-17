using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ElizadeEHR.Helpers;

namespace ElizadeEHR.Doctor.Records
{
    /// <summary>
    /// Interaction logic for PatientRecordsDetailPage.xaml
    /// </summary>
    public partial class PatientRecordsDetailPage : UserControl
    {
        private Patient _selectedPatient;
        private List<LabFile> _labFiles;

        public List<LabFile> LabFiles
        {
            get { return _labFiles; }
            set { _labFiles = value; }
        }

        public PatientRecordsDetailPage(Patient selectedPatient)
        {
            InitializeComponent();
            _selectedPatient = selectedPatient;
            this.DataContext = this;

            // Load complete patient data with medical alerts (like in ConsultationPage)
            LoadPatientData();

            // Load lab files
            LabFiles = DatabaseHelper.GetLabFilesByPatientId(_selectedPatient.PatientID);
            LabFilesDataGrid.ItemsSource = LabFiles;
        }

        private void LoadPatientData()
        {
            // Get complete patient data including medical alerts
            var patientWithAlerts = DatabaseHelper.GetPatientWithMedicalAlerts(_selectedPatient.PatientID);
            if (patientWithAlerts != null)
            {
                _selectedPatient = patientWithAlerts;
            }

            // Populate patient info
            PatientFullNameTextBlock.Text = $"{_selectedPatient.FirstName} {_selectedPatient.LastName}";
            PatientDOBTextBlock.Text = _selectedPatient.DateOfBirth.ToString("MMMM dd, yyyy");
            PatientGenderTextBlock.Text = _selectedPatient.Gender;
            EmailTextBlock.Text = _selectedPatient.Email;
            PhoneNumberTextBlock.Text = _selectedPatient.Phone;

            // Set medical history
            if (string.IsNullOrEmpty(_selectedPatient.MedicalAlerts))
            {
                MedicalHistoryTextBlock.Text = "No medical history recorded";
            }
            else
            {
                MedicalHistoryTextBlock.Text = _selectedPatient.MedicalAlerts;
            }
        }
        private void ViewLabFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                LabFile labFile = button?.CommandParameter as LabFile;

                string filePath = DatabaseHelper.GetLabFileFullPath(labFile.FileName);

                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File not found: {labFile.FileName}", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DownloadLabFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                LabFile labFile = button?.CommandParameter as LabFile;

                if (labFile == null)
                {
                    MessageBox.Show("Unable to retrieve file information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Try multiple possible locations
                string[] possiblePaths = {
            GetLabFileFullPath(labFile.FilePath),
            GetLabFileFullPath(labFile.FileName),
            labFile.FilePath,  // Maybe it's already a full path
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Elizade Clinic", "LabFiles", labFile.FileName),
            // Add any other possible locations where the file might be
        };

                string sourceFilePath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        sourceFilePath = path;
                        break;
                    }
                }

                if (sourceFilePath == null)
                {
                    // Show all locations we checked
                    string searchedPaths = string.Join("\n", possiblePaths);
                    MessageBox.Show($"File 'internship.jpg' not found in any of these locations:\n\n{searchedPaths}",
                                  "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Show save file dialog
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = labFile.FileName,
                    Filter = GetFileFilter(labFile.FileName),
                    Title = "Save Lab File"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.Copy(sourceFilePath, saveFileDialog.FileName, true);
                    MessageBox.Show($"File downloaded successfully to: {saveFileDialog.FileName}",
                                  "Download Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper method to get the full file path
        private string GetLabFileFullPath(string fileName)
        {
            // Adjust this path based on where your lab files are stored
            string labFilesDirectory = @"C:\LabFiles\"; // or ConfigurationManager.AppSettings["LabFilesPath"]
            return System.IO.Path.Combine(labFilesDirectory, fileName);
        }

        // Helper method to determine file filter for save dialog (C# 7.3 compatible)
        private string GetFileFilter(string fileName)
        {
            string extension = System.IO.Path.GetExtension(fileName).ToLower();

            if (extension == ".pdf")
                return "PDF Files|*.pdf|All Files|*.*";
            else if (extension == ".jpg" || extension == ".jpeg")
                return "JPEG Images|*.jpg;*.jpeg|All Files|*.*";
            else if (extension == ".png")
                return "PNG Images|*.png|All Files|*.*";
            else if (extension == ".doc")
                return "Word Documents|*.doc|All Files|*.*";
            else if (extension == ".docx")
                return "Word Documents|*.docx|All Files|*.*";
            else if (extension == ".txt")
                return "Text Files|*.txt|All Files|*.*";
            else if (extension == ".csv")
                return "CSV Files|*.csv|All Files|*.*";
            else if (extension == ".xlsx" || extension == ".xls")
                return "Excel Files|*.xlsx;*.xls|All Files|*.*";
            else
                return "All Files|*.*";
        }
    }
}
