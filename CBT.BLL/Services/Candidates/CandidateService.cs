using CBT.BLL.Constants;
using CBT.BLL.Services.Category;
using CBT.BLL.Services.FileUpload;
using CBT.BLL.Utilities;
using CBT.Contracts;
using CBT.Contracts.Candidates;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using CBT.DAL;
using CBT.DAL.Models.Candidates;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Candidates
{
    public class CandidateService : ICandidateService
    {
        private readonly DataContext _context;
        private readonly IFileUploadService _fileUpload;
        private readonly IConfiguration _config;
        private readonly ICandidateCategoryService _candidateCategoryService;
        private readonly IHttpContextAccessor _accessor;

        public CandidateService(DataContext context, IFileUploadService fileUpload, IConfiguration config, ICandidateCategoryService candidateCategoryService, IHttpContextAccessor accessor)
        {
            _context = context;
            _fileUpload = fileUpload;
            _config = config;
            _candidateCategoryService = candidateCategoryService;
            _accessor = accessor;
        }
        public async Task<APIResponse<string>> CreateCandidate(CreateCandidate request)
        {
            var res = new APIResponse<string>();

            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());
                var userType = int.Parse(_accessor.HttpContext.Items["userType"].ToString());

                var category = await _context.CandidateCategory.Where(m => m.CandidateCategoryId == request.CandidateCategoryId && m.ClientId == clientId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateCategoryId couldn't be found";
                    return res;
                }

                var result = CandidateExamId.Generate();

                var filePath = _fileUpload.UploadImage(request.PassportPhoto);
                var candidate = new Candidate
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    OtherName = request.OtherName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    PassportPhoto = filePath,
                    UserType = userType,
                    ClientId = clientId,
                    CandidateNo = result.Keys.First(),
                    CandidateExamId = result.Values.First(),
                    CandidateCategoryId = request.CandidateCategoryId
                };

                _context.Candidate.Add(candidate);
                await _context.SaveChangesAsync();

                res.Result = candidate.CandidateId.ToString();
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
        public async Task<APIResponse<List<SelectCandidates>>> GetAllCandidates()
        {
            var res = new APIResponse<List<SelectCandidates>>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var result = await _context.Candidate
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.ClientId == clientId).Select(a => new SelectCandidates
                    {
                        CandidateId = a.CandidateId.ToString(),
                        CandidateExamId = a.CandidateExamId,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        OtherName = a.OtherName,
                        PhoneNumber = a.PhoneNumber,
                        Email = a.Email,
                        PassportPhoto = a.PassportPhoto,
                        CandidateCategoryId = a.CandidateCategoryId.ToString(),
                        DateCreated = a.CreatedOn.ToString()
                    }).ToListAsync();

                res.IsSuccessful = true;
                res.Result = result;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<SelectCandidates>> GetCandidate(string candidateId)
        {
            var res = new APIResponse<SelectCandidates>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var result = await _context.Candidate
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.CandidateId == Guid.Parse(candidateId) && d.ClientId == clientId ).Select(a => new SelectCandidates
                    {
                        CandidateId = a.CandidateId.ToString(),
                        CandidateExamId = a.CandidateExamId,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        OtherName = a.OtherName,
                        PhoneNumber = a.PhoneNumber,
                        Email = a.Email,
                        PassportPhoto = a.PassportPhoto,
                        CandidateCategoryId = a.CandidateCategoryId.ToString(),
                        DateCreated = a.CreatedOn.ToString()
                    }).FirstOrDefaultAsync();

                if (result == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                }
                else
                {
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                }

                res.IsSuccessful = true;
                res.Result = result;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        //public async Task<string> GenerateExamId()
        //{
        //    try
        //    {

        //        string candidateId = CandidateExamId.Generate();
        //        Random random = new();
        //        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        //        string newCandidateId = new string(Enumerable.Repeat(chars, 5)
        //            .Select(s => s[random.Next(s.Length)]).ToArray());

        //        newCandidateId = $"{newCandidateId}-{candidateId}";
        //        return newCandidateId;
        //    }
        //    catch(Exception ex)
        //    {
        //        return "";
        //    }
        //}

        public async Task<APIResponse<string>> UpdateCandidate(UpdateCandidate request)
        {
            var res = new APIResponse<string>();

            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var category = await _context.CandidateCategory.Where(m => m.CandidateCategoryId == request.CandidateCategoryId && m.ClientId == clientId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateCategoryId doesn't exist";
                    return res;
                }

                var candidate = await _context.Candidate.Where(m => m.CandidateId == Guid.Parse(request.CandidateId) && m.ClientId == clientId).FirstOrDefaultAsync();
                if (candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateId doesn't exist";
                    return res;
                }
                var filePath = _fileUpload.UploadImage(request.PassportPhoto);
                candidate.FirstName = request.FirstName;
                candidate.LastName = request.LastName;
                candidate.OtherName = request.OtherName;
                candidate.PhoneNumber = request.PhoneNumber;
                candidate.Email = request.Email;
                candidate.PassportPhoto = filePath;
                candidate.CandidateCategoryId = request.CandidateCategoryId;


                await _context.SaveChangesAsync();
                res.Result = candidate.CandidateId.ToString();
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Updated;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<bool>> DeleteCandidate(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());
                var category = await _context.Candidate.Where(d => d.Deleted != true && d.CandidateId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.Message.FriendlyMessage = "Candidate does not exist";
                    res.IsSuccessful = true;
                    return res;
                }

                category.Deleted = true;
                await _context.SaveChangesAsync();

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
                res.Result = true;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }
}
