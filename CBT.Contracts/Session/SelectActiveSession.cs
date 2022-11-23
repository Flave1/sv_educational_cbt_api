using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Session
{
    public class SelectActiveSession
    {
        public string SessionId { get; set; }
        public string Session { get; set; }
        public string SessionTermId { get; set; }
        public string SessionTerm { get; set; }
    }
}
