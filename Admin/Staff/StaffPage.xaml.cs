using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ElizadeEHR
{
    public partial class StaffPage : UserControl
    {
        public StaffPage()
        {
            InitializeComponent();
            LoadStaffs();
        }

        private void LoadStaffs()
        {
            // Fetch all users (staff)
            List<User> users = DatabaseHelper.GetAllUsers();
            StaffDataGrid.ItemsSource = users;
        }

        private User GetSelectedUser()
        {
            return StaffDataGrid.SelectedItem as User;
        }

        private void AddStaffButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new StaffEditWindow();           // A dialog you create for add/edit
            if (dlg.ShowDialog() == true)
            {
                LoadStaffs();
            }
        }

        private void EditStaff_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = StaffDataGrid.SelectedItem as User;
            if (selectedUser == null)
            {
                MessageBox.Show("Please select a staff member to edit.");
                return;
            }

            var dlg = new StaffEditWindow(selectedUser);
            if (dlg.ShowDialog() == true)
            {
                LoadStaffs();
            }
        }


        private void DeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedUser();
            if (selected == null)
            {
                MessageBox.Show("Please select a staff member first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {selected.FirstName} {selected.LastName}?",
                                         "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DatabaseHelper.DeleteUser(selected.UserID);
                LoadStaffs();
            }
        }

        // Class-level variables to store search state
        private string currentSearchText = string.Empty;
        private string selectedRole = string.Empty;

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

        // Event handler for the ComboBox SelectionChanged event
        private void RoleFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = RoundedComboBoxStyle.SelectedItem as ComboBoxItem;

            // If "Role" placeholder is selected, nothing is selected, or "All" is selected, clear role filter
            if (selectedItem == null ||
                (selectedItem.Content.ToString() == "Role" && !selectedItem.IsEnabled) ||
                selectedItem.Content.ToString() == "All")
            {
                selectedRole = string.Empty;
            }
            else
            {
                // Store the selected role
                selectedRole = selectedItem.Content.ToString();
            }
            // Optionally perform search immediately when role changes
            PerformSearch();
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
            if (StaffDataGrid != null)
            {
                // Get the original collection
                var allItems = DatabaseHelper.GetAllUsers(); // Replace with your actual data source method

                // Apply filters
                var filteredItems = allItems;

                // Apply text search if there's search text
                // Apply text search if there's search text
                if (!string.IsNullOrEmpty(currentSearchText))
                {
                    string searchLower = currentSearchText.ToLower();
                    filteredItems = filteredItems.Where(user =>
                        user.FirstName.ToLower().Contains(searchLower) ||
                        user.LastName.ToLower().Contains(searchLower) ||
                        (user.FirstName.ToLower() + " " + user.LastName.ToLower()).Contains(searchLower) ||
                        user.Email.ToLower().Contains(searchLower) ||
                        user.Phone.ToLower().Contains(searchLower)).ToList();
                }

                // Apply role filter if a role is selected
                if (!string.IsNullOrEmpty(selectedRole) && selectedRole != "Role")
                {
                    filteredItems = filteredItems.Where(user => user.Role == selectedRole).ToList();
                }

                // Update the DataGrid
                StaffDataGrid.ItemsSource = filteredItems;
            }
        }
    }
}

