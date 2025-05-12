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
using MySqlConnector;

namespace ElizadeEHR
{
    /// <summary>
    /// Interaction logic for DoctorDashboard.xaml
    /// </summary>
    public partial class DoctorDashboard : Window
    {
        public DoctorDashboard(string fullName, string email)
        {
            InitializeComponent();
            //LoadDashboardData(); // Call to load initial data on load

            //LoadProfilePicture();
            MainContentControl.Content = new DoctorDashboardHomePage();

            //// Show the name and email in the UI
            //NameTextBlock.Text = fullName;
            //EmailTextBlock.Text = email;
        }
        
    }
}
