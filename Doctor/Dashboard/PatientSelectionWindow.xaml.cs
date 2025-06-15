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
        // Class-level variables to store search state
        private string currentSearchText = string.Empty;
        private string selectedGender = string.Empty;

        // Event handler for the TextBox TextChanged event
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update placeholder visibility
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                SearchPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                SearchPlaceholder.Visibility = Visibility.Collapsed;
            }

            // Store the current search text
            currentSearchText = SearchBox.Text;
        }

        // Event handler for the Search button click
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Perform the actual search and update the DataGrid
            PerformSearch();
        }

        // Method to perform the search and update the DataGrid
        private void PerformSearch()
        {
            // Assuming you have a DataGrid named UsersDataGrid
            if (PatientsDataGrid != null)
            {
                // Get the original collection
                var allItems = DatabaseHelper.GetAllPatients(); // Replace with your actual data source method

                // Apply filters
                var filteredItems = allItems;

                // Apply text search if there's search text
                // Apply text search if there's search text
                if (!string.IsNullOrEmpty(currentSearchText))
                {
                    string searchLower = currentSearchText.ToLower();
                    filteredItems = filteredItems.Where(patient =>
                        patient.FirstName.ToLower().Contains(searchLower) ||
                        patient.LastName.ToLower().Contains(searchLower) ||
                        (patient.FirstName.ToLower() + " " + patient.LastName.ToLower()).Contains(searchLower) ||
                        patient.Email.ToLower().Contains(searchLower) ||
                        patient.Phone.ToLower().Contains(searchLower)).ToList();
                }

                // Apply role filter if a role is selected
                if (!string.IsNullOrEmpty(selectedGender) && selectedGender != "Gender")
                {
                    filteredItems = filteredItems.Where(patient => patient.Gender == selectedGender).ToList();
                }

                // Update the DataGrid
                PatientsDataGrid.ItemsSource = filteredItems;
            }
        }
    }
}
