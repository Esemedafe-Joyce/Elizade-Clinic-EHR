using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Shapes;

namespace ElizadeEHR
{
    /// <summary>
    /// Interaction logic for PatientEditWindow.xaml
    /// </summary>
    public partial class PatientEditWindow : Window
    {
        public Patient SelectedPatient { get; set; }
        public PatientEditWindow()
        {
            InitializeComponent();
        }

        internal class DatabaseConfig
        {
            public static string ConnectionString = "server=localhost;database=campusehrconsole;user=root;password=joycedafe3225%;";
        }

        private Patient _patientBeingEdited;

        public PatientEditWindow(Patient patientToEdit = null)
        {
            InitializeComponent();

            if (patientToEdit != null)
            {
                _patientBeingEdited = patientToEdit;
                PopulateFields(patientToEdit);
            }
        }

        private void PopulateFields(Patient patient)
        {
            FirstNameTextBox.Text = patient.FirstName;
            LastNameTextBox.Text = patient.LastName;
            EmailTextBox.Text = patient.Email;
            PhoneTextBox.Text = patient.Phone;
            DateOfBirthPicker.SelectedDate = patient.DateOfBirth;
            MatricNumberTextBox.Text = patient.MatricNumber;
            GenderComboBox.SelectedItem = GetComboBoxItemByContent(GenderComboBox, patient.Gender);
        }

        //Helper method
        private ComboBoxItem GetComboBoxItemByContent(ComboBox comboBox, string content)
        {
            return comboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == content);
        }

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FirstNamePlaceholder.Visibility = string.IsNullOrEmpty(FirstNameTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LastNamePlaceholder.Visibility = string.IsNullOrEmpty(LastNameTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        private void MatricNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MatricNumberPlaceholder.Visibility = string.IsNullOrEmpty(MatricNumberTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholder.Visibility = string.IsNullOrEmpty(EmailTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PhonePlaceholder.Visibility = string.IsNullOrEmpty(PhoneTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Validate phone input to only allow digits
        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Your gender selection logic here
            ComboBoxItem selectedItem = GenderComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null && selectedItem.Content.ToString() != "Gender")
            {
                // Handle gender selection
                string selectedGender = selectedItem.Content.ToString();
                // Your logic here
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate all fields
            if (string.IsNullOrEmpty(FirstNameTextBox.Text))
            {
                MessageBox.Show("Please enter first name.");
                return;
            }

            if (string.IsNullOrEmpty(LastNameTextBox.Text))
            {
                MessageBox.Show("Please enter last name.");
                return;
            }
            if (string.IsNullOrEmpty(MatricNumberTextBox.Text))
            {
                MessageBox.Show("Please enter matric number.");
                return;
            }

            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                MessageBox.Show("Please enter email.");
                return;
            }

            if (string.IsNullOrEmpty(PhoneTextBox.Text))
            {
                MessageBox.Show("Please enter phone number.");
                return;
            }

            ComboBoxItem genderItem = GenderComboBox.SelectedItem as ComboBoxItem;
            if (genderItem == null || genderItem.Content.ToString() == "Gender")
            {
                MessageBox.Show("Please select a gender.");
                return;
            }

            // All validations passed, save user data

            bool isEdit = SelectedPatient != null;
            int patientId = isEdit ? SelectedPatient.PatientID : 0;
            SavePatientData(isEdit, patientId);
        }

        private void SavePatientData(bool isEdit = false, int patientID = 0)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string email = EmailTextBox.Text;
            string matricNumber = MatricNumberTextBox.Text;
            string phone = PhoneTextBox.Text;
            string gender = ((ComboBoxItem)GenderComboBox.SelectedItem)?.Content.ToString();

            // Validate date of birth
            if (!DateOfBirthPicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date of birth.");
                return;
            }

            DateTime dateOfBirth = DateOfBirthPicker.SelectedDate.Value;

            Patient patient = new Patient
            {
                PatientID = _patientBeingEdited?.PatientID ?? 0,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                MatricNumber = matricNumber,
                Gender = gender,
                DateOfBirth = dateOfBirth  // ✅ Storing DOB
            };

            bool success = _patientBeingEdited == null
                ? DatabaseHelper.SavePatient(patient)
                : DatabaseHelper.UpdatePatient(patient);

            if (success)
            {
                string action = _patientBeingEdited == null ? "Created" : "Edited";
                MessageBox.Show($"User {action.ToLower()} successfully.");
                DatabaseHelper.LogAction(App.UserID, $"{action} user {lastName} {firstName}");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to save user. Please try again.");
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void DateOfBirthPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
