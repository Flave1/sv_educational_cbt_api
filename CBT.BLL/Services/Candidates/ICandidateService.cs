﻿using CBT.Contracts.Category;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;

namespace CBT.BLL.Services.Candidates
{
    public interface ICandidateService
    {
        Task<APIResponse<string>> CreateCandidate(CreateCandidate request);
        Task<APIResponse<List<SelectCandidates>>> GetAllCandidates();
        Task<APIResponse<SelectCandidates>> GetCandidate(string candidateId);
        //Task<string> GenerateExamId();
        Task<APIResponse<string>> UpdateCandidate(UpdateCandidate request);
        Task<APIResponse<bool>> DeleteCandidate(SingleDelete request);
        Task<APIResponse<object>> Login(CandidateLogin request);
        Task<AuthDetails> GenerateAuthenticationToken(string candidateExamId, string candidateEmail);
    }
}
