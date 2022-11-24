using CBT.BLL.Constants;
using CBT.BLL.Services.Category;
using CBT.BLL.Services.FileUpload;
using CBT.BLL.Utilities;
using CBT.Contracts;
using CBT.Contracts.Candidates;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using CBT.Contracts.Examinations;
using CBT.DAL;
using CBT.DAL.Models.Authentication;
using CBT.DAL.Models.Candidates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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

        public CandidateService(DataContext context, IFileUploadService fileUpload, IConfiguration config, IHttpContextAccessor accessor)
        {
            _context = context;
            _fileUpload = fileUpload;
            _config = config;
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
                    .OrderByDescending(c => c.CreatedOn)
                    .Where(c => c.Deleted != true && c.ClientId == clientId)
                    .Select(d => new SelectCandidates(d, _context.CandidateCategory.FirstOrDefault(x => x.CandidateCategoryId == d.CandidateCategoryId))).ToListAsync();
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
                    .OrderByDescending(c => c.CreatedOn)
                    .Where(c => c.Deleted != true && c.CandidateId == Guid.Parse(candidateId) && c.ClientId == clientId)
                    .Select(d => new SelectCandidates(d, _context.CandidateCategory.FirstOrDefault(x => x.CandidateCategoryId == d.CandidateCategoryId))).FirstOrDefaultAsync();

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

        public async Task<APIResponse<object>> Login(CandidateLogin request)
        {
            var res = new APIResponse<object>();
            try
            {
                var candidate = await _context.Candidate.FirstOrDefaultAsync(x => x.CandidateExamId == request.CandidateExamId);
                if(candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Invalid CandidateExamId";
                    return res;
                }
                var examination = await _context.Examination.Where(x=>x.CandidateCategoryId_ClassId == candidate.CandidateCategoryId.ToString()
                && (x.StartTime <= DateTime.Now && x.EndTime > DateTime.Now)).ToListAsync();

                if(!examination.Any())
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "You do not have an active Examination!";
                    return res;
                }

                if(candidate.Email.ToLower() != request.CandidateEmail.ToLower())
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Invalid Email address";
                    return res;
                }

                var result = examination.Select(x => new SelectExamination(x));

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = new { AuthDetails = await GenerateAuthenticationToken(request.CandidateExamId, request.CandidateEmail), result };
                return res;

            }
            catch(Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<AuthDetails> GenerateAuthenticationToken(string candidateExamId, string candidateEmail)
        {
            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, candidateEmail),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Email, candidateEmail),
               new Claim("candidateExamId", candidateExamId)
            };
           
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthDetails { Token = tokenHandler.WriteToken(token), Expires = tokenDescriptor.Expires.ToString() };
        }
    }
}
