using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.DAL.Models.Candidates
{
    public class Candidate : CommonEntity
    {
        [Key]
        public Guid CandidateId { get; set; }
        public string CandidateExamId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PassportPhoto { get; set; }
        public int UserType { get; set; }
        public Guid ClientId { get; set; }
        public Guid CandidateCategoryId { get; set; }

    }
}
