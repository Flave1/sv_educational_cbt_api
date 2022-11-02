using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Category
{
    public class SelectCandidateCategory
    {
        public Guid CandidateCategoryId { get; set; }
        public string Name { get; set; }
        public string DateCreated { get; set; }
    }
}
