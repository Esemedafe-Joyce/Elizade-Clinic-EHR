using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElizadeEHR
{
    public partial class StaffEditWindow : Window
    {
        public User SelectedUser { get; set; }

        public StaffEditWindow()
        {
            InitializeComponent();
        }

        internal class DatabaseConfig
        {
            public static string ConnectionString = "server=localhost;database=campusehrconsole;user=root;password=joycedafe3225%;";
        }
        private User _userBeingEdited;

        public StaffEditWindow(User userToEdit = null)
        {
            InitializeComponent();

            if (userToEdit != null)
            {
                _userBeingEdited = userToEdit;
                PopulateFields(userToEdit);
            }
        }

        private void PopulateFields(User user)
        {
            FirstNameTextBox.Text = user.FirstName;
            LastNameTextBox.Text = user.LastName;
            EmailTextBox.Text = user.Email;
            PhoneTextBox.Text = user.Phone;
            PasswordTextBox.Text = ""; // Leave empty unless you allow editing password
            RoleComboBox.SelectedItem = GetComboBoxItemByContent(RoleComboBox, user.Role);
            GenderComboBox.SelectedItem = GetComboBoxItemByContent(GenderComboBox, user.Gender);
        }

        //Helper method
        private ComboBoxItem GetComboBoxItemByContent(ComboBox comboBox, string content)
        {
            return comboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == content);
        }


        //public StaffEditWindow(User user) : this()
        //{
        //    SelectedUser = user;

        //    // Prepopulate fields for editing
        //    FirstNameTextBox.Text = user.FirstName;
        //    LastNameTextBox.Text = user.LastName;
        //    EmailTextBox.Text = user.Email;
        //    RoleComboBox.SelectedItem = user.Role;
        //    SetComboBoxItem(RoleComboBox, user.Role);
        //    SetComboBoxItem(GenderComboBox, user.Gender);
        //}

        //private void SetComboBoxItem(ComboBox comboBox, string value)
        //{
        //    foreach (ComboBoxItem item in comboBox.Items)
        //    {
        //        if (item.Content.ToString() == value)
        //        {
        //            comboBox.SelectedItem = item;
        //            break;
        //        }
        //    }
        //}

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

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Your role selection logic here
            ComboBoxItem selectedItem = RoleComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null && selectedItem.Content.ToString() != "Role")
            {
                // Handle role selection
                string selectedRole = selectedItem.Content.ToString();
                // Your logic here
            }
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

            if (string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                MessageBox.Show("Please enter password.");
                return;
            }

            ComboBoxItem roleItem = RoleComboBox.SelectedItem as ComboBoxItem;
            if (roleItem == null || roleItem.Content.ToString() == "Role")
            {
                MessageBox.Show("Please select a role.");
                return;
            }

            ComboBoxItem genderItem = GenderComboBox.SelectedItem as ComboBoxItem;
            if (genderItem == null || genderItem.Content.ToString() == "Gender")
            {
                MessageBox.Show("Please select a gender.");
                return;
            }

            // All validations passed, save user data
           
            bool isEdit = SelectedUser != null;
            int userId = isEdit ? SelectedUser.UserID : 0;
            SaveUserData(isEdit, userId);
        }

        private void SaveUserData(bool isEdit = false, int userId = 0)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string email = EmailTextBox.Text;
            string phone = PhoneTextBox.Text;
            string password = PasswordTextBox.Text;
            string role = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Content.ToString();
            string gender = ((ComboBoxItem)GenderComboBox.SelectedItem)?.Content.ToString();

            string passwordHash = !string.IsNullOrEmpty(password)
       ? ComputeSha256Hash(password)
       : null;

            User user = new User
            {
                UserID = _userBeingEdited?.UserID ?? 0,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Gender = gender,
                Role = role,
                PasswordHash = passwordHash
            };

            bool success = _userBeingEdited == null
                ? DatabaseHelper.SaveUser(user)
                : DatabaseHelper.UpdateUser(user);

            if (success)
            {
                string action = _userBeingEdited == null ? "Created" : "Edited";
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


        // SHA-256 hashing method



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }


        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
    }
