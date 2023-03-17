using CBT.BLL.Constants;
using CBT.BLL.Filters;
using CBT.BLL.Services.Class;
using CBT.BLL.Services.FileUpload;
using CBT.BLL.Services.Pagination;
using CBT.BLL.Services.Settings;
using CBT.BLL.Utilities;
using CBT.BLL.Wrappers;
using CBT.Contracts;
using CBT.Contracts.Authentication;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.Contracts.Examinations;
using CBT.Contracts.Settings;
using CBT.DAL;
using CBT.DAL.Models.Candidates;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        private readonly IPaginationService paginationService;
        private readonly DateTime localTime;

        public CandidateService(DataContext context, IFileUploadService fileUpload, IConfiguration config, 
            IHttpContextAccessor accessor, IClassService classService, IPaginationService paginationService, 
            IUtilityService utilityService)
        {
            this.context = context;
            this.fileUpload = fileUpload;
            this.config = config;
            this.accessor = accessor;
            this.classService = classService;
            this.paginationService = paginationService;
            localTime = utilityService.GetCurrentLocalDateTime();
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
        public async Task<APIResponse<PagedResponse<List<SelectCandidates>>>> GetAllCandidates(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<SelectCandidates>>>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var query = context.Candidate
                    .Where(c => c.Deleted != true && c.ClientId == clientId)
                    .Include(c => c.Category)
                    .OrderByDescending(c => c.CreatedOn);

                 var totalRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectCandidates(d)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);

                res.IsSuccessful = true;
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
                    .Where(x=>x.CandidateExaminationId.ToLower() == request.ExaminationId.ToLower() && x.Deleted != true)
                    .Include(q => q.Question)
                    .Select(db=> new SelectExamination(db, localTime)).FirstOrDefaultAsync();

                if(examination == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "No record found for examinationId!";
                    return res;
                }
                if (examination.Status == (int)ExaminationStatus.Waiting)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = $"This examination is yet to commence, please check back by {examination.StartTime}!";
                    return res;
                }

                if (examination.Status == (int)ExaminationStatus.Concluded)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = $"This examination was concluded on {examination.EndTime}!";
                    return res;
                }

                if (examination.Status == (int)ExaminationStatus.Cancelled)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = $"This examination has been cancelled!";
                    return res;
                }

                var result = new CandidateExamDetails
                {
                    ExaminationType = examination.ExamintionType,
                    ExaminationStatus = examination.Status
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
                    .Where(x => x.Email.ToLower() == request.Email.ToLower() && x.Deleted != true)
                    .Include(c=>c.Category)
                    .Select(db => new SelectCandidates(db)).FirstOrDefaultAsync();

                if (candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Invalid email address!";
                    return res;
                }

                var examination = context.Examination?.Where(x => x.CandidateExaminationId.ToLower() == request.ExaminationId.ToLower() && x.CandidateCategoryId_ClassId == candidate.CandidateCategoryId && x.Deleted != true);
                var examinationDetails = await examination.Include(q=>q.Question).Select(db => new SelectExamination(db, localTime))
                    .FirstOrDefaultAsync();

                if (examinationDetails == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "You're not authorized to access examination!";
                    return res;
                }
                if(!string.IsNullOrEmpty(examinationDetails.CandidateIds) && examinationDetails.CandidateIds.Split(",").Contains(candidate.Id))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "This examination has been previously submitted!";
                    return res;
                }
                var clientId = examination.FirstOrDefault().ClientId;

                string[] questionsId = Array.Empty<string>();
                if(examinationDetails.ShuffleQuestions)
                {
                    questionsId = context.Question?.Where(x => x.ExaminationId == Guid.Parse(examinationDetails.ExaminationId) && x.Deleted != true)
                        .OrderBy(x=> Guid.NewGuid()).Select(x=>x.QuestionId.ToString()).ToArray();
                }
                else
                {
                    questionsId = context.Question?.Where(x => x.ExaminationId == Guid.Parse(examinationDetails.ExaminationId) && x.Deleted != true)
                        .OrderByDescending(x=>x.CreatedOn).Select(x => x.QuestionId.ToString()).ToArray();
                }
                var result = new CandidateLoginDetails
                {
                    AuthDetails = await GenerateAuthenticationToken(examinationDetails.ExaminationId, candidate.Id, examination.FirstOrDefault().SmsClientId),
                    ExaminationDetails = examinationDetails,
                    Settings = await context.Setting?.Where(d => d.Deleted != true && d.ClientId == clientId)
                    .Select(db => new SelectSettings(db)).FirstOrDefaultAsync(),
                    QuestionsId = questionsId
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

        private async Task<AuthDetails> GenerateAuthenticationToken(string examinationId, string candidateId_regNo, string smsClientId)
        {
            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, candidateId_regNo),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Email, candidateId_regNo),
               new Claim("examinationId", examinationId),
               new Claim("candidateId_regNo", candidateId_regNo),
               new Claim("smsClientId", smsClientId)
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
                var examination = context.Examination.Where(x => x.CandidateExaminationId.ToLower() == request.ExaminationId.ToLower() && x.Deleted != true);
                var examinationDetails = await examination.Include(q => q.Question).Select(db => new SelectExamination(db, localTime))
                   .FirstOrDefaultAsync();
                if(examinationDetails == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Invalid Examination Id!";
                    return res;
                }

                var studentClass = await classService.GetActiveClassByRegNo(request.RegistrationNo, examination.FirstOrDefault().SmsClientId);
                if(studentClass.Result == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = studentClass.Message.FriendlyMessage;
                    return res;
                }

                if(Guid.Parse(studentClass.Result.SessionClassId) != Guid.Parse(examinationDetails.CandidateCategoryId_ClassId))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "You're not authorized to access this examination!";
                    return res;
                }
                if (!string.IsNullOrEmpty(examinationDetails.CandidateIds) && examinationDetails.CandidateIds.Split(",").Contains(request.RegistrationNo))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "This examination has been previously submitted!";
                    return res;
                }
                var clientId = examination.FirstOrDefault().ClientId;
                string[] questionsId = Array.Empty<string>();
                if (examinationDetails.ShuffleQuestions)
                {
                    questionsId = context.Question?.Where(x => x.ExaminationId == Guid.Parse(examinationDetails.ExaminationId) && x.Deleted != true)
                        .OrderBy(x => Guid.NewGuid()).Select(x => x.QuestionId.ToString()).ToArray();
                }
                else
                {
                    questionsId = context.Question?.Where(x => x.ExaminationId == Guid.Parse(examinationDetails.ExaminationId) && x.Deleted != true)
                        .OrderByDescending(x => x.CreatedOn).Select(x => x.QuestionId.ToString()).ToArray();
                }
                var result = new CandidateLoginDetails
                {
                    AuthDetails = await GenerateAuthenticationToken(examinationDetails.ExaminationId, request.RegistrationNo, examination.FirstOrDefault().SmsClientId),
                    ExaminationDetails = examinationDetails,
                    Settings = await context.Setting?.Where(d => d.Deleted != true && d.ClientId == clientId)
                    .Select(db => new SelectSettings(db)).FirstOrDefaultAsync(),
                    QuestionsId = questionsId
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

        public async Task<APIResponse<SMPCbtCreateCandidateResponse>> CreateAdmissionCandidate(CreateAdmissionCandidate request)
        {
            var res = new APIResponse<SMPCbtCreateCandidateResponse>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                CandidateCategory category = new CandidateCategory();
                if (string.IsNullOrEmpty(request.CandidateCategory))
                    category = await CreateCategoryAsync(request.CategoryName);
                else
                    category = await UpdateCategoryAsync(request.CategoryName, request.CandidateCategory, clientId);

                foreach (var item in request.AdmissionCandidateList)
                {
                    var candidate = await context.Candidate.FirstOrDefaultAsync(x => x.Email.ToLower() == item.Email.ToLower());
                    if (candidate == null)
                    {
                        var result = UtilTools.GenerateCandidateId();
                        candidate = new Candidate
                        {
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            OtherName = item.OtherName,
                            PhoneNumber = item.PhoneNumber,
                            Email = item.Email,
                            CandidateNo = result.Keys.First(),
                            CandidateId = result.Values.First(),
                            CandidateCategoryId = category.CandidateCategoryId
                        };
                        context.Candidate.Add(candidate);
                    }
                    else
                    {
                        candidate.FirstName = item.FirstName;
                        candidate.LastName = item.LastName;
                        candidate.OtherName = item.OtherName;
                        candidate.PhoneNumber = item.PhoneNumber;
                    }
                    await context.SaveChangesAsync();
                };
                var response = new SMPCbtCreateCandidateResponse();
                response.CategoryName = category.Name;
                response.CategoryId = category.CandidateCategoryId.ToString();
                res.Result = response;


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
        private async Task<CandidateCategory> CreateCategoryAsync(string name)
        {
            var newCategory = new CandidateCategory
            {
                Name = name,
            };
            context.CandidateCategory.Add(newCategory);
            await context.SaveChangesAsync();
            return newCategory;
        }

        private async Task<CandidateCategory> UpdateCategoryAsync(string name, string id, Guid clientId)
        {
            var categery = context.CandidateCategory.FirstOrDefault(cc => cc.CandidateCategoryId == Guid.Parse(id) && cc.Deleted == false && cc.ClientId == clientId);
            if (categery == null)
            {
                throw new ArgumentException(nameof(categery));
            }
            categery.Name = name;
            await context.SaveChangesAsync();
            return categery;
        }

        public async Task<APIResponse<PagedResponse<List<SelectCandidates>>>> GetAllCandidateByCategory(PaginationFilter filter, string categoryId)
        {
            var res = new APIResponse<PagedResponse<List<SelectCandidates>>>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var query = context.Candidate
                    .Where(c => c.Deleted != true && c.ClientId == clientId && c.CandidateCategoryId == Guid.Parse(categoryId))
                    .Include(c => c.Category)
                    .OrderByDescending(c => c.CreatedOn);

                var totalRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(d => new SelectCandidates(d)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);

                res.IsSuccessful = true;
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
    }

    
}
