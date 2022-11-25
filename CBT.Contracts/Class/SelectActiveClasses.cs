using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Class
{
    public class SelectActiveClasses
    {
        public string SessionClassId { get; set; }
        public string SessionId { get; set; }
        public string Session { get; set; }
        public string ClassId { get; set; }
        public string Class { get; set; }
        public string FormTeacherId { get; set; }
        public bool InSession { get; set; }
        public int ExamScore { get; set; }
        public int AssessmentScore { get; set; }
        public int PassMark { get; set; }
    }
}
