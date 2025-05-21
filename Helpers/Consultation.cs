using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElizadeEHR.Helpers
{
    public class Consultation
    {
        public int ConsultationID { get; set; }      // Primary Key (auto-increment)
        public int PatientID { get; set; }           // FK to Patients table
        public int DoctorID { get; set; }            // FK to Users table (for doctors)

        public string VisitReason { get; set; }      // Reason for visit
        public string Diagnosis { get; set; }        // Optional diagnosis text
        public string Vitals { get; set; }           // Optional vitals info
        public string LabSummary { get; set; }       // Summary of lab needs/results

        public bool FollowUpRequired { get; set; }   // TRUE/FALSE toggle
        public DateTime CreatedAt { get; set; }      // Automatically set in DB
    }

}
