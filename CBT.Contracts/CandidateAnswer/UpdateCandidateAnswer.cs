using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.CandidateAnswer
{
    public class UpdateCandidateAnswer: CreateCandidateAnswer
    {
        public Guid AnswerId { get; set; }
    }
}
