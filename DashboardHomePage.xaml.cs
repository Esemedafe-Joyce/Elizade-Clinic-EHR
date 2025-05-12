using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
using MySql.Data.MySqlClient;

namespace ElizadeEHR
{
    /// <summary>
    /// Interaction logic for DashboardHomePage.xaml
    /// </summary>
    public partial class DashboardHomePage : UserControl
    {
        public DashboardHomePage()
        {
            InitializeComponent();
        }
        // Set the welcome text after fetching last name
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetWelcomeText();
            DateTextBlock.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy"); // Example: Monday, 08 May 2025
        }

        private void SetWelcomeText()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT LastName FROM Users WHERE Email = @Email";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", App.UserEmail);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string lastName = reader["LastName"].ToString();
                                WelcomeTextBlock.Text = $"Welcome {lastName}";
                            }
                            else
                            {
                                WelcomeTextBlock.Text = "Welcome";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading welcome message: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                WelcomeTextBlock.Text = "Welcome";
            }
        }

        // Event handler for Total Patients Button click
        // Event handler for TotalPatientsButton click
        private void TotalPatientsButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset other buttons and set the clicked button's style
            ResetDashboardButtonSelection();

            // Change the background and text color of the clicked button
            TotalPatientsButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            TotalPatientsTextBlock.Foreground = Brushes.White;

            // Update the DataGrid to show patient data
            DashboardDataGrid.Columns.Clear();

            // Make the DataGrid visible
            DashboardDataGrid.Visibility = Visibility.Visible;

            // Add columns for patient data
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "PatientID", Binding = new Binding("PatientID") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "LASTNAME", Binding = new Binding("LastName") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "FIRSTNAME", Binding = new Binding("FirstName") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "DATE OF BIRTH", Binding = new Binding("DateOfBirth") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "GENDER", Binding = new Binding("Gender") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "PHONE", Binding = new Binding("Phone") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "EMAIL", Binding = new Binding("Email") });

            // Load patient data from the database
            DashboardDataGrid.ItemsSource = DatabaseHelper.GetAllPatients(); // Use your actual data fetching method
        }

        // Event handler for TotalDoctorsButton click
        private void TotalDoctorsButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset other buttons and set the clicked button's style
            ResetDashboardButtonSelection();

            // Change the background and text color of the clicked button
            TotalDoctorsButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            AvailableDoctorsTextBlock.Foreground = Brushes.White;

            // Update the DataGrid to show doctor data
            DashboardDataGrid.Columns.Clear();

            // Make the DataGrid visible
            DashboardDataGrid.Visibility = Visibility.Visible;

            // Add columns for doctor data
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "UserID", Binding = new Binding("UserID") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "LASTNAME", Binding = new Binding("LastName") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "FIRSTNAME", Binding = new Binding("FirstName") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "GENDER", Binding = new Binding("Gender") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "PHONE", Binding = new Binding("Phone") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "EMAIL", Binding = new Binding("Email") });
            DashboardDataGrid.Columns.Add(new DataGridTextColumn { Header = "ROLE", Binding = new Binding("Role") });

            // Load doctor data from the database
            DashboardDataGrid.ItemsSource = DatabaseHelper.GetAllUsers(); // Use your actual data fetching method
        }

        // Event handler for ReportButton click
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            ResetDashboardButtonSelection();
            ReportButton.Background = (Brush)new BrushConverter().ConvertFrom("#26547C");
            ReportTextBlock.Foreground = Brushes.White;

            // Show and populate the grid
            DashboardDataGrid.Visibility = Visibility.Visible;
            DashboardDataGrid.Columns.Clear();

            DashboardDataGrid.Columns.Add(
              new DataGridTextColumn { Header = "Log ID", Binding = new Binding("LogID") });
            DashboardDataGrid.Columns.Add(
              new DataGridTextColumn { Header = "User", Binding = new Binding("UserFullName") });
            DashboardDataGrid.Columns.Add(
              new DataGridTextColumn { Header = "Action", Binding = new Binding("Action") });
            DashboardDataGrid.Columns.Add(
              new DataGridTextColumn { Header = "Timestamp", Binding = new Binding("Timestamp") });

            DashboardDataGrid.ItemsSource = DatabaseHelper.GetAllAuditLogs();

            // Log that the report was viewed
            DatabaseHelper.LogAction(App.UserID, "Viewed audit log report");
        }


        // Reset the background and text color for all buttons
        private void ResetDashboardButtonSelection()
        {
            // Reset TotalPatientsButton
            TotalPatientsButton.Background = Brushes.White;
            TotalPatientsTextBlock.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");

            // Reset TotalDoctorsButton
            TotalDoctorsButton.Background = Brushes.White;
            AvailableDoctorsTextBlock.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");

            // Reset ReportButton
            ReportButton.Background = Brushes.White;
            ReportTextBlock.Foreground = (Brush)new BrushConverter().ConvertFrom("#26547C");
        }
    }
}
