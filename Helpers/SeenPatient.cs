using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElizadeEHR.Helpers
{
    public class SeenPatient
    {
        public string PatientName { get; set; }
        public string SeenTime { get; set; }
        public string Status { get; set; } = "Seen";
    }

}
