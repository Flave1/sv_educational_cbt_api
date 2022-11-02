using CBT.BLL.Constants;
using CBT.BLL.Services.FileUpload;
using CBT.Contracts;
using CBT.Contracts.Candidates;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using CBT.DAL;
using CBT.DAL.Models.Candidates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        public CandidateService(DataContext context, IFileUploadService fileUpload)
        {
            _context = context;
            _fileUpload = fileUpload;
        }
        public async Task<APIResponse<string>> CreateCandidate(CreateCandidate request, Guid clientId, int userType)
        {
            var res = new APIResponse<string>();

            try
            {
                var category = _context.CandidateCategory.Where(m => m.CandidateCategoryId == request.CandidateCategoryId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateCategoryId couldn't be found";
                    return res;
                }
                var filePath = _fileUpload.UploadImage(request.PassportPhoto);
                string candidateExamId = await GenerateExamId(10);
                if(candidateExamId == "")
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Error generating CandidateExamId";
                    return res;
                };

                var candidate = new Candidate
                {
                    CandidateExamId = candidateExamId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    OtherName = request.OtherName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    PassportPhoto = filePath,
                    UserType = userType,
                    ClientId = clientId,
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
                var result = await _context.Candidate
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true).Select(a => new SelectCandidates
                    {
                        CandidateId = a.CandidateId,
                        CandidateExamId = a.CandidateExamId,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        OtherName = a.OtherName,
                        PhoneNumber = a.PhoneNumber,
                        Email = a.Email,
                        PassportPhoto = a.PassportPhoto,
                        CandidateCategoryId = a.CandidateCategoryId,
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

        public async Task<APIResponse<SelectCandidates>> GetCandidate(Guid candidateId)
        {
            var res = new APIResponse<SelectCandidates>();
            try
            {
                var result = await _context.Candidate
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.CandidateId == candidateId).Select(a => new SelectCandidates
                    {
                        CandidateId = a.CandidateId,
                        CandidateExamId = a.CandidateExamId,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        OtherName = a.OtherName,
                        PhoneNumber = a.PhoneNumber,
                        Email = a.Email,
                        PassportPhoto = a.PassportPhoto,
                        CandidateCategoryId = a.CandidateCategoryId,
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

        public async Task<string> GenerateExamId(int length)
        {
            try
            {
                Random random = new();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        public async Task<APIResponse<string>> UpdateCandidate(UpdateCandidate request)
        {
            var res = new APIResponse<string>();

            try
            {
                var category = await _context.CandidateCategory.Where(m => m.CandidateCategoryId == request.CandidateCategoryId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateCategoryId doesn't exist";
                    return res;
                }

                var candidate = await _context.Candidate.Where(m => m.CandidateId == request.CandidateId).FirstOrDefaultAsync();
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
                var category = await _context.Candidate.Where(d => d.Deleted != true && d.CandidateId == Guid.Parse(request.Item)).FirstOrDefaultAsync();
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
