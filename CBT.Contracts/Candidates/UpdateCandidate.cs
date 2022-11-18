using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Candidates
{
    public class UpdateCandidate : CreateCandidate
    {
        public string CandidateId { get; set; }
    }
}
