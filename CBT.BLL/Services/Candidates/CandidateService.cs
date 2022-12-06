using CBT.BLL.Constants;
using CBT.BLL.Services.Class;
using CBT.BLL.Services.FileUpload;
using CBT.BLL.Services.Settings;
using CBT.BLL.Utilities;
using CBT.Contracts;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.Contracts.Examinations;
using CBT.Contracts.Settings;
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
        private readonly IClassService classService;

        public CandidateService(DataContext context, IFileUploadService fileUpload, IConfiguration config, 
            IHttpContextAccessor accessor, IClassService classService)
        {
            this.context = context;
            this.fileUpload = fileUpload;
            this.config = config;
            this.accessor = accessor;
            this.classService = classService;
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

                res.Result = candidate.Id.ToString();
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var category = await context.Candidate.Where(d => d.Deleted != true && d.Id == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (category == null)
                {
                    res.Message.FriendlyMessage = "Candidate does not exist";
                    res.IsSuccessful = false;
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

        public async Task<APIResponse<CandidateExamDetails>> LoginByExamId(CandidateLoginExamId request)
        {
            var res = new APIResponse<CandidateExamDetails>();
            try
            {
                var examination = await context.Examination
                    .Where(x=>x.CandidateExaminationId.ToLower() == request.ExaminationId.ToLower())
                    .Include(q => q.Question)
                    .Select(db=> new SelectExamination(db)).FirstOrDefaultAsync();

                if(examination == null || !(examination.Status == (int)ExaminationStatus.InProgress))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "You do not have an active Examination!";
                    return res;
                }

                //var result = new CandidateLoginDetails
                //{
                //    AuthDetails = await GenerateAuthenticationToken(request.ExaminationId),
                //    ExaminationDetails = examination,
                //};

                var result = new CandidateExamDetails
                {
                    ExaminationType = examination.ExamintionType
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


        public async Task<APIResponse<CandidateLoginDetails>> LoginByEmail(CandidateLoginEmail request)
        {
            var res = new APIResponse<CandidateLoginDetails>();
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

                var examination = context.Examination.Where(x => x.CandidateCategoryId_ClassId == candidate.CandidateCategoryId);
                    
                var examinationDetails = await examination.Include(q=>q.Question).Select(db => new SelectExamination(db))
                    .FirstOrDefaultAsync();

                if (examination == null || !(Convert.ToDateTime(examinationDetails.StartTime) <= DateTime.Now && Convert.ToDateTime(examinationDetails.EndTime) > DateTime.Now))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "You do not have an active Examination!";
                    return res;
                }
                var clientId = examination.FirstOrDefault().ClientId;
                var result = new CandidateLoginDetails
                {
                    AuthDetails = await GenerateAuthenticationToken(examinationDetails.CandidateExaminationId),
                    ExaminationDetails = examinationDetails,
                    Settings = await context.Setting?.Where(d => d.Deleted != true && d.ClientId == clientId)
                    .Select(db => new SelectSettings(db)).FirstOrDefaultAsync()
                };

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
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

        public async Task<APIResponse<CandidateLoginDetails>> LoginByRegNo(CandidateLoginRegNo request)
        {
            var res = new APIResponse<CandidateLoginDetails>();
            try
            {
                var examination = context.Examination.Where(x => x.CandidateExaminationId.ToLower() == request.ExaminationId.ToLower());
                var examinationDetails = await examination.Include(q => q.Question).Select(db => new SelectExamination(db))
                   .FirstOrDefaultAsync();

                if (examination == null || !(Convert.ToDateTime(examinationDetails.StartTime) <= DateTime.Now && Convert.ToDateTime(examinationDetails.EndTime) > DateTime.Now))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "You do not have an active Examination!";
                    return res;
                }

                var studentClass = await classService.GetActiveClassByRegNo(request.RegistrationNo, examination.FirstOrDefault().ProductBaseurlSuffix);
                if(studentClass.Result == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = studentClass.Message.FriendlyMessage;
                    return res;
                }

                var clientId = examination.FirstOrDefault().ClientId;
                var result = new CandidateLoginDetails
                {
                    AuthDetails = await GenerateAuthenticationToken(examinationDetails.CandidateExaminationId),
                    ExaminationDetails = examinationDetails,
                    Settings = await context.Setting?.Where(d => d.Deleted != true && d.ClientId == clientId)
                    .Select(db => new SelectSettings(db)).FirstOrDefaultAsync()
                };

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
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
    }
}
