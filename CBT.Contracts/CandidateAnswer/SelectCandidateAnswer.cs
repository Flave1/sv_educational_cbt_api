using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.CandidateAnswer
{
    public class SelectCandidateAnswer
    {
        public string AnswerId { get; set; }
        public string QuestionId { get; set; }
        public string CandidateId { get; set; }
        public string[] Answers { get; set; }
    }
}
