using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Candidate
{
    public class SelectCandidateCategory
    {
        public Guid CandidateCategoryId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
