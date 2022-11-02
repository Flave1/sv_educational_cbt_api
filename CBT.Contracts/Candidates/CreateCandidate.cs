using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Candidates
{
    public class CreateCandidate
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public IFormFile PassportPhoto { get; set; }
        public Guid CandidateCategoryId { get; set; }
    }
}
