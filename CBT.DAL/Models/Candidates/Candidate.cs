using CBT.DAL.Models.Candidate;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBT.DAL.Models.Candidates
{
    public class Candidate : CommonEntity
    {
        [Key]
        public Guid CandidateId { get; set; }
        public string CandidateNo { get; set; }
        public string CandidateExamId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PassportPhoto { get; set; }
        public Guid CandidateCategoryId { get; set; }
        [ForeignKey("CandidateCategoryId")]
        public CandidateCategory Category { get; set; }

    }
}
