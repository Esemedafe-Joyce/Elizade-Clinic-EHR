using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElizadeEHR
{
    public class User
    {
        public int UserID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }            // Admin, Doctor, Nurse, etc.
        public DateTime CreatedAt { get; set; }     // Account creation date
        public string ProfilePicture { get; set; }  // Path to profile image
        public string Password { get; set; }
        public string PasswordHash { get; set; }

    }

}
