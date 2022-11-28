using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.CandidateAnswers
{
    public class CreateCandidateAnswer
    {
        public string QuestionId { get; set; }
        public string CandidateId { get; set; }
        public int[] Answers { get; set; }
    }
}
