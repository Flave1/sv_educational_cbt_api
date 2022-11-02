using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.DAL.Models.Candidate
{
    public class CandidateCategory : CommonEntity
    {
        [Key]
        public Guid CandidateCategoryId { get; set; }
        public string Name { get; set; }
        public int UserType { get; set; }
        public Guid ClientId { get; set; }
    }
}
