using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElizadeEHR.Helpers
{
    public class Prescription
    {
        public int PrescriptionID { get; set; }
        public int ConsultationID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }
        public DateTime DatePrescribed { get; set; }
        public bool SentToPharmacy { get; set; }
    }

}
