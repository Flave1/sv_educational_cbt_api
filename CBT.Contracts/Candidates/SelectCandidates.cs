using CBT.DAL.Models.Candidates;

namespace CBT.Contracts.Candidates
{
    public class SelectCandidates
    {
        public string Id { get; set; }
        public string CandidateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PassportPhoto { get; set; }
        public string CandidateCategoryId { get; set; }
        public string CandidateCategory { get; set; }
        public string DateCreated { get; set; }

        public SelectCandidates(Candidate candidate)
        {
            Id = candidate.Id.ToString();
            CandidateId = candidate.CandidateId;
            FirstName = candidate.FirstName;
            LastName = candidate.LastName;
            OtherName = candidate.OtherName;
            PhoneNumber = candidate.PhoneNumber;
            Email = candidate.Email;
            PassportPhoto = candidate.PassportPhoto;
            CandidateCategoryId = candidate.CandidateCategoryId.ToString();
            CandidateCategory = candidate?.Category?.Name;
            DateCreated = candidate.CreatedOn.ToString("yyyy-MM-dd HH:mm"); ;
        }
    }
}
