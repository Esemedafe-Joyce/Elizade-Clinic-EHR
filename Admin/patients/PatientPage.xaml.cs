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

namespace ElizadeEHR
{
    /// <summary>
    /// Interaction logic for PatientPage.xaml
    /// </summary>
    public partial class PatientPage : UserControl
    {
        public PatientPage()
        {
            InitializeComponent();
            LoadPatients();
        }
        private void LoadPatients()
        {
            // Fetch all users (staff)
            List<Patient> patient = DatabaseHelper.GetAllPatients();
            PatientsDataGrid.ItemsSource = patient;
        }
        private Patient GetSelectedPatient()
        {
            return PatientsDataGrid.SelectedItem as Patient;
        }

        private void AddPatientButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PatientEditWindow();           // A dialog you create for add/edit
            if (dlg.ShowDialog() == true)
            {
                LoadPatients();
            }
        }

        private void EditPatient_Click(object sender, RoutedEventArgs e)
        {
            var selectedPatient = PatientsDataGrid.SelectedItem as Patient;
            if (selectedPatient == null)
            {
                MessageBox.Show("Please select a patient to edit.");
                return;
            }

            var dlg = new PatientEditWindow(selectedPatient);
            if (dlg.ShowDialog() == true)
            {
                LoadPatients();
            }
        }


        private void DeletePatient_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedPatient();
            if (selected == null)
            {
                MessageBox.Show("Please select a patient first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {selected.FirstName} {selected.LastName}?",
                                         "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DatabaseHelper.DeletePatient(selected.PatientID);
                LoadPatients();
            }
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

        private void GenderFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = RoundedComboBoxStyle.SelectedItem as ComboBoxItem;

            // If "Role" placeholder is selected, nothing is selected, or "All" is selected, clear role filter
            if (selectedItem == null ||
                (selectedItem.Content.ToString() == "Gender" && !selectedItem.IsEnabled) ||
                selectedItem.Content.ToString() == "All")
            {
                selectedGender = string.Empty;
            }
            else
            {
                // Store the selected role
                selectedGender = selectedItem.Content.ToString();
            }
            // Optionally perform search immediately when role changes
            PerformSearch();
        }
    }
}
