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
using MySqlConnector;

namespace ElizadeEHR
{
    /// <summary>
    /// Interaction logic for DoctorDashboardHomePage.xaml
    /// </summary>
    public partial class DoctorDashboardHomePage : UserControl
    {
        public DoctorDashboardHomePage()
        {
            InitializeComponent();
        }
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
    }
}
