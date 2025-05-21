using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElizadeEHR.Helpers
{
    public class LabFile
    {
        public int FileID { get; set; }
        public int PatientID { get; set; }
        public int ConsultationID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
        public int UploadedBy { get; set; }
    }

}
