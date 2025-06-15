using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElizadeEHR
{
   public class Patient{
            public int PatientID { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string MatricNumber { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
        public string MedicalAlerts { get; set; } // Allergies, chronic conditions
        public DateTime CreatedAt { get; set; }
    }
}
