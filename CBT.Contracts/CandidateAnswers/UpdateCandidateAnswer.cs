using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.CandidateAnswers
{
    public class UpdateCandidateAnswer: CreateCandidateAnswer
    {
        public Guid AnswerId { get; set; }
    }
}
