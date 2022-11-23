using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Candidates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Candidates
{
    public class SelectCandidates
    {
        public string CandidateId { get; set; }
        public string CandidateExamId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PassportPhoto { get; set; }
        public string CandidateCategoryId { get; set; }
        public string CandidateCategory { get; set; }
        public string DateCreated { get; set; }

        public SelectCandidates(Candidate candidate, CandidateCategory category)
        {
            CandidateId = candidate.CandidateId.ToString();
            CandidateExamId = candidate.CandidateExamId;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            OtherName = candidate.OtherName;
            PhoneNumber = candidate.PhoneNumber;
            Email = candidate.Email;
            PassportPhoto = candidate.PassportPhoto;
            CandidateCategoryId = candidate.CandidateCategoryId.ToString();
            CandidateCategory = category.Name;
            DateCreated = candidate.CreatedOn.ToString();
        }
    }
}
