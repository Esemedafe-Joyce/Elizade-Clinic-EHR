using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElizadeEHR
{
    public class AuditLog
    {
        public int LogID { get; set; }
        public int UserID { get; set; }
        public string UserFullName { get; set; }  // New property
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
