using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Subject
{
    public class SelectSubjects
    {
        public string lookupId { get; set; }
        public string name { get; set; }
        public object gradeLevelId { get; set; }
        public bool isActive { get; set; }
        public object gradeLevelName { get; set; }
    }
}
