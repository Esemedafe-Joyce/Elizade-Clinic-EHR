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
using System.Windows.Shapes;

namespace ElizadeEHR.Doctor
{
    /// <summary>
    /// Interaction logic for PatientSelectionWindow.xaml
    /// </summary>
    public partial class PatientSelectionWindow : Window
    {
        public Patient SelectedPatient { get; private set; }

        public PatientSelectionWindow()
        {
            InitializeComponent();
            LoadPendingPatients();
        }

        private void LoadPendingPatients()
        {
            try
            {
                List<Patient> pendingPatients = DatabaseHelper.GetPendingPatientsForConsultation();
                PatientsDataGrid.ItemsSource = pendingPatients;

                if (pendingPatients.Count == 0)
                {
                    MessageBox.Show("There are currently no patients waiting for consultation.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = false;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading pending patients:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartConsultationButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPatient = PatientsDataGrid.SelectedItem as Patient;

            if (SelectedPatient == null)
            {
                MessageBox.Show("Please select a patient to start consultation.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPatient = PatientsDataGrid.SelectedItem as Patient;

            if (SelectedPatient == null)
            {
                MessageBox.Show("Please select a patient.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
