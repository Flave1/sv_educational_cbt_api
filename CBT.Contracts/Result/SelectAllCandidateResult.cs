using CBT.DAL.Models.Candidates;
using CBT.DAL.Models.Examinations;
using CBT.DAL.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Result
{
    public class SelectAllCandidateResult
    {
        public string CandidateName { get; set; }
        public string CandidateId { get; set; }
        public string ExaminationName { get; set; }
        public int TotalScore { get; set; }
        public string Status { get; set; }
    }
}
