using CBT.DAL.Models.Examinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Candidates
{
    public class CandidateExamDetails
    {
        public int ExaminationType { get; set; }
        public int ExaminationStatus { get; set; }
    }
}
