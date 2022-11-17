using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.CandidateAnswer
{
    public class CreateCandidateAnswer
    {
        public Guid QuestionId { get; set; }
        public int CandidateId { get; set; }
        public int[] Answers { get; set; }
    }
}
