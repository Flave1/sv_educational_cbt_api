﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Candidates
{
    public class CreateAdmissionCandidate
    {
        public string CandidateCategory { get; set; }
        public List<AdmissionCandidateList> AdmissionCandidateList { get; set; }
    }
    public class AdmissionCandidateList
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}