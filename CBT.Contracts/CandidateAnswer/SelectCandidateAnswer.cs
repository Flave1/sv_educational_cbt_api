using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.CandidateAnswer
{
    public class SelectCandidateAnswer
    {
        public Guid AnswerId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid CandidateId { get; set; }
        public string[] Answers { get; set; }
    }
}
