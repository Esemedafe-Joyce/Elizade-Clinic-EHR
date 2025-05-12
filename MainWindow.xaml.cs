using System;
using System.Windows;
using System.Text.RegularExpressions;
//using MySql.Data.MySqlClient;
using System.Windows.Media;
using MySqlConnector;
using Org.BouncyCastle.Asn1.X509;

namespace ElizadeEHR
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            lblErrorMessage.Text = "";
            lblErrorMessage.Visibility = Visibility.Collapsed;

            if (!Regex.IsMatch(username, @"^[a-zA-Z]+\.[a-zA-Z]+$"))
            {
                ShowErrorMessage("Username must be in the format lastname.firstname.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("Password cannot be empty.");
                return;
            }

            if (AuthenticateUser(username, password))
            {
                DatabaseHelper.LogAction(App.UserID, "Logged in");

                if (App.UserRole == "Admin")
                {
                    AdminDashboard adminDashboard = new AdminDashboard(App.UserName, App.UserEmail);
                    adminDashboard.Show();
                }
                else if (App.UserRole == "Doctor")
                {
                    DoctorDashboard doctorDashboard = new DoctorDashboard(App.UserName, App.UserEmail);
                    doctorDashboard.Show();
                }
                else
                {
                    MessageBox.Show($"No dashboard implemented for role: {App.UserRole}", "Unknown Role", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                this.Close();
            }

        }


        private bool AuthenticateUser(string username, string password)
        {
            bool isAuthenticated = false;

            using (MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    conn.Open();
                    string[] nameParts = username.Split('.');
                    string lastName = nameParts[0];
                    string firstName = nameParts[1];

                    string query = "SELECT UserID,FirstName, LastName, Email, ProfilePicture, Role FROM Users WHERE LastName = @lastName AND FirstName = @firstName AND PasswordHash = SHA2(@password, 256)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@lastName", lastName);
                        cmd.Parameters.AddWithValue("@firstName", firstName);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                App.UserID = reader.GetInt32("UserID");
                                App.UserName = $"{reader["FirstName"]} {reader["LastName"]}";
                                App.UserEmail = reader["Email"].ToString();
                                App.ProfilePicturePath = reader["ProfilePicture"].ToString();
                                App.UserRole = reader["Role"].ToString(); // <-- store role here
                                isAuthenticated = true;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return isAuthenticated;
        }
        private void ShowErrorMessage(string message)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.Visibility = Visibility.Visible;
        }

        private void InputField_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnLogin_Click(sender, new RoutedEventArgs());
            }
        }


        private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "lastname.firstname")
            {
                txtUsername.Text = "";
                txtUsername.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = "lastname.firstname";
                txtUsername.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void txtUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }

    internal class DatabaseConfig
    {
        public static string ConnectionString = "server=localhost;database=campusehrconsole;user=root;password=joycedafe3225%;";
    }
}
