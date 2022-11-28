using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.DAL.Models.Candidates
{
    public class CandidateCategory : CommonEntity
    {
        [Key]
        public Guid CandidateCategoryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Candidate> Candidate { get; set; }
    }
}
