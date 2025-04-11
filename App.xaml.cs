using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ElizadeEHR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
            // Stores logged-in user details
            public static string UserName { get; set; }
            public static string UserEmail { get; set; }
            public static string ProfilePicturePath { get; set; } = "Images/default-profile.png"; // Default profile picture

            /// <summary>
            /// Resets user session data (useful when logging out)
            /// </summary>
            public static void ResetUserSession()
            {
                UserName = string.Empty;
                UserEmail = string.Empty;
                ProfilePicturePath = "Images/default-profile.png";
            }
    }
}
