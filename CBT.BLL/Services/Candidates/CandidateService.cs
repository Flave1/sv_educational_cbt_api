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
                var result = UtilTools.GenerateCandidateId();
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
                    CandidateId = result.Values.First(),
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
                    .Where(c => c.Deleted != true && c.Id == Guid.Parse(candidateId) && c.ClientId == clientId)
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
                var candidate = await context.Candidate.Where(m => m.Id == Guid.Parse(request.CandidateId) && m.ClientId == clientId).FirstOrDefaultAsync();
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
                var category = await context.Candidate.Where(d => d.Deleted != true && d.Id == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
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

        public async Task<APIResponse<CandidateLoginDetails>> LoginByExamId(CandidateLoginExamId request)
        {
            var res = new APIResponse<CandidateLoginDetails>();
            try
            {
                var examination = await context.Examination.Where(x=>x.CandidateExaminationId.ToLower() == request.ExaminationId.ToLower())
                    .Select(db=> new SelectExamination(db)).FirstOrDefaultAsync();

                if(examination == null || !(examination.Status == (int)ExaminationStatus.InProgress))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "You do not have an active Examination!";
                    return res;
                }

                var result = new CandidateLoginDetails
                {
                    AuthDetails = await GenerateAuthenticationToken(request.ExaminationId),
                    ExaminationDetails = examination,
                };

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = result;
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


        public async Task<APIResponse<SelectCandidates>> LoginByEmail(CandidateLoginEmail request)
        {
            var res = new APIResponse<SelectCandidates>();
            try
            {
                var candidate = await context.Candidate
                    .Where(x => x.Email.ToLower() == request.Email.ToLower())
                    .Include(c=>c.Category)
                    .Select(db => new SelectCandidates(db)).FirstOrDefaultAsync();

                if (candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Invalid email address!";
                    return res;
                }

                var examination = await context.Examination.Where(x=> x.CandidateCategoryId_ClassId == candidate.CandidateCategoryId).FirstOrDefaultAsync();

                if (examination == null || !(examination.StartTime <= DateTime.Now && examination.EndTime > DateTime.Now))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "You do not have an active Examination!";
                    return res;
                }

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = candidate;
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

        public async Task<AuthDetails> GenerateAuthenticationToken(string examinationId)
        {
            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, examinationId),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Email, examinationId)
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
