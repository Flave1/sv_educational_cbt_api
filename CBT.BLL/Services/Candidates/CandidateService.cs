using CBT.BLL.Constants;
using CBT.BLL.Services.FileUpload;
using CBT.BLL.Utilities;
using CBT.Contracts;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.Contracts.Examinations;
using CBT.DAL;
using CBT.DAL.Models.Candidates;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CBT.BLL.Services.Candidates
{
    public class CandidateService : ICandidateService
    {
        private readonly DataContext context;
        private readonly IFileUploadService fileUpload;
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor accessor;
        public CandidateService(DataContext context, IFileUploadService fileUpload, IConfiguration config, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.fileUpload = fileUpload;
            this.config = config;
            this.accessor = accessor;
        }
        public async Task<APIResponse<string>> CreateCandidate(CreateCandidate request)
        {
            var res = new APIResponse<string>();
            try
            {
                var result = CandidateExamId.Generate();
                var filePath = fileUpload.UploadImage(request.PassportPhoto);
                var candidate = new Candidate
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    OtherName = request.OtherName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    PassportPhoto = filePath,
                    CandidateNo = result.Keys.First(),
                    CandidateExamId = result.Values.First(),
                    CandidateCategoryId = request.CandidateCategoryId
                };

                context.Candidate.Add(candidate);
                await context.SaveChangesAsync();

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
                var clientId = Guid.Parse(accessor.HttpContext.Items["smsClientId"].ToString());
                var result = await context.Candidate
                    .Where(c => c.Deleted != true && c.ClientId == clientId)
                    .Include(c => c.Category)
                    .OrderByDescending(c => c.CreatedOn)
                    .Select(d => new SelectCandidates(d)).ToListAsync();
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["smsClientId"].ToString());
                var result = await context.Candidate
                    .Where(c => c.Deleted != true && c.CandidateId == Guid.Parse(candidateId) && c.ClientId == clientId)
                    .Include(c => c.Category)
                    .Select(d => new SelectCandidates(d)).FirstOrDefaultAsync();

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

        public async Task<APIResponse<string>> UpdateCandidate(UpdateCandidate request)
        {
            var res = new APIResponse<string>();

            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["smsClientId"].ToString());

                var candidate = await context.Candidate.Where(m => m.CandidateId == Guid.Parse(request.CandidateId) && m.ClientId == clientId).FirstOrDefaultAsync();
                if (candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateId doesn't exist";
                    return res;
                }
                var filePath = fileUpload.UploadImage(request.PassportPhoto);
                candidate.FirstName = request.FirstName;
                candidate.LastName = request.LastName;
                candidate.OtherName = request.OtherName;
                candidate.PhoneNumber = request.PhoneNumber;
                candidate.Email = request.Email;
                candidate.PassportPhoto = filePath;
                candidate.CandidateCategoryId = request.CandidateCategoryId;


                await context.SaveChangesAsync();
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["smsClientId"].ToString());
                var category = await context.Candidate.Where(d => d.Deleted != true && d.CandidateId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.Message.FriendlyMessage = "Candidate does not exist";
                    res.IsSuccessful = true;
                    return res;
                }

                category.Deleted = true;
                await context.SaveChangesAsync();

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
                var candidate = await context.Candidate.FirstOrDefaultAsync(x => x.CandidateExamId == request.CandidateExamId);
                if(candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Invalid CandidateExamId";
                    return res;
                }
                var examination = await context.Examination.Where(x=>x.CandidateCategoryId_ClassId == candidate.CandidateCategoryId.ToString()
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
           
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value));
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
