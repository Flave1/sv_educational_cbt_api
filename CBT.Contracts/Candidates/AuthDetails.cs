using CBT.Contracts.Examinations;
using CBT.Contracts.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Candidates
{
    public class AuthDetails
    {
        public string Token { get; set; }
        public string Expires { get; set; }
    }
    public class CandidateLoginDetails
    {
        public AuthDetails AuthDetails { get; set; }
        public SelectExamination ExaminationDetails { get; set; }
        public SelectSettings Settings { get; set; }

    }

}
