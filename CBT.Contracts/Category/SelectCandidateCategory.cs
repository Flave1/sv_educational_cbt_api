using CBT.DAL.Models.Candidates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Category
{
    public class SelectCandidateCategory
    {
        public string CandidateCategoryId { get; set; }
        public string Name { get; set; }
        public string DateCreated { get; set; }
        public SelectCandidateCategory(CandidateCategory category)
        {
            CandidateCategoryId = category.CandidateCategoryId.ToString();
            Name = category.Name;
            DateCreated = category.CreatedOn.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
