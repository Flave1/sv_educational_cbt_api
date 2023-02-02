using CBT.BLL.Constants;
using CBT.BLL.Filters;
using CBT.BLL.Services.Class;
using CBT.BLL.Services.Pagination;
using CBT.BLL.Services.Student;
using CBT.BLL.Utilities;
using CBT.BLL.Wrappers;
using CBT.Contracts;
using CBT.Contracts.Authentication;
using CBT.Contracts.CandidateAnswers;
using CBT.Contracts.Result;
using CBT.DAL;
using CBT.DAL.Models.Examinations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace CBT.BLL.Services.Result
{
    public class ResultService : IResultService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IStudentService studentService;
        private readonly IPaginationService paginationService;
        private readonly IUtilityService utilityService;
        private readonly DateTime localTime;

        public ResultService(DataContext context, IHttpContextAccessor accessor,
            IStudentService studentService, IPaginationService paginationService,
            IUtilityService utilityService)
        {
            this.context = context;
            this.accessor = accessor;
            this.studentService = studentService;
            this.paginationService = paginationService;
            this.localTime = utilityService.GetCurrentLocalDateTime();
        }

        public async Task<APIResponse<PagedResponse<List<SelectAllCandidateResult>>>> GetAllCandidateResult(PaginationFilter filter, string examinationId)
        {
            var res = new APIResponse<PagedResponse<List<SelectAllCandidateResult>>>();
            try
            {
                var examination = await context.Examination?.Where(x => x.ExaminationId == Guid.Parse(examinationId))?.FirstOrDefaultAsync();

                var questionIds = context.Question?.Where(x => x.ExaminationId == examination.ExaminationId)
                    .Select(x => x.QuestionId)
                    .ToList();

                if (examination.ExaminationType == (int)ExaminationType.ExternalExam)
                {
                    var query = context.Candidate.Where(x => x.CandidateCategoryId == Guid.Parse(examination.CandidateCategoryId_ClassId));

                    var totalRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(x => new SelectAllCandidateResult(x, examination, GetTotalScore(questionIds, x.Id.ToString()))).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                }

                if (examination.ExaminationType == (int)ExaminationType.InternalExam)
                {
                    var students = await studentService.GetAllStudentDetails(filter.PageNumber, filter.PageSize, examination.CandidateCategoryId_ClassId, examination.ProductBaseurlSuffix);
                    if(students.Result != null)
                    {
                        var totalRecord = students.Result.TotalRecords;
                        var result = students.Result.Data.Select(x => new SelectAllCandidateResult(x, examination, GetTotalScore(questionIds, x.RegistrationNumber.ToString()))).ToList();
                        res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                        res.Result.TotalPages = students.Result.TotalPages;
                    }
                    
                }

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

        public async Task<APIResponse<SelectResult>> GetCandidateResult()
        {
            var res = new APIResponse<SelectResult>();
            try
            {
                var examinationId = Guid.Parse(accessor.HttpContext.Items["examinationId"].ToString());
                var candidateId_regNo = accessor.HttpContext.Items["candidateId_regNo"].ToString();

                var examination = await context.Examination?.Where(x => x.ExaminationId == examinationId)?.FirstOrDefaultAsync();

                var questionIds = await context.Question?.Where(x => x.ExaminationId == examinationId)
                    .Select(x => x.QuestionId)
                    .ToListAsync();

                int totalScore = 0;
                foreach (var item in questionIds)
                {
                    var answer = await context.Question?.Where(x => x.QuestionId == item)?.FirstOrDefaultAsync();
                    var candidateAnswer = await context.CandidateAnswer?.Where(x => x.QuestionId == item && x.CandidateId == candidateId_regNo)?.FirstOrDefaultAsync();

                    if (answer?.Answers == candidateAnswer?.Answers)
                    {
                        totalScore += answer.Mark;
                    }

                }

                string status = "";
                string candidateName = "";

                if(examination.ExaminationType == (int)ExaminationType.InternalExam)
                {
                    status = totalScore >= examination.ExamScore ? "Passed" : "Failed";
                    var student = await studentService.GetStudentDetails(candidateId_regNo, examination.ProductBaseurlSuffix);
                    if(student.Result != null)
                    {
                        candidateName = $"{student.Result.FirstName} {student.Result.LastName}";
                    }
                }
                else
                {
                    status = totalScore >= examination.PassMark ? "Passed" : "Failed";
                    var candidate = await context.Candidate?.Where(x => x.Id == Guid.Parse(candidateId_regNo))?.FirstOrDefaultAsync();
                    candidateName = $"{ candidate?.FirstName} {candidate?.LastName}";
                }

                var result = new SelectResult
                {
                    CandidateName = candidateName,
                    CandidateId = candidateId_regNo,
                    ExaminationName = examination.ExamName_Subject,
                    TotalScore = totalScore,
                    Status = status
                };

                res.IsSuccessful = true;
                res.Result = result;
                res.Message.FriendlyMessage = Messages.GetSuccess;
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

        private static int GetTotalScore(List<Guid> questionIds, string candidateId_regNo)
        {
            int totalScore = 0;

            foreach (var item in questionIds)
            {
                var dataContext = new DataContext();
                var answer = dataContext.Question?.Where(x => x.QuestionId == item)?.FirstOrDefault();
                var candidateAnswer = dataContext.CandidateAnswer?.Where(x => x.QuestionId == item && x.CandidateId.ToLower() == candidateId_regNo.ToLower())?.FirstOrDefault();

                if (answer?.Answers == candidateAnswer?.Answers)
                {
                    totalScore += answer.Mark;
                }

            }

            return totalScore;
        }

        public async Task<APIResponse<byte[]>> DownloadCandidateResult(string examinationId)
        {
            var res = new APIResponse<byte[]>();
            try
            {
                byte[] resultFile = new byte[0];
                var examination = await context.Examination?.Where(x => x.ExaminationId == Guid.Parse(examinationId) && x.Deleted != true)?.FirstOrDefaultAsync();

                var questionIds = context.Question?.Where(x => x.ExaminationId == examination.ExaminationId && x.Deleted != true)
                    .Select(x => x.QuestionId)
                    .ToList();

                var candidatesResult = new List<SelectAllCandidateResult>();
                if(examination.ExaminationType == (int)ExaminationType.ExternalExam)
                {
                    candidatesResult = await context.Candidate.Where(x => x.CandidateCategoryId == Guid.Parse(examination.CandidateCategoryId_ClassId) && x.Deleted != true)
                    .Select(x => new SelectAllCandidateResult(x, examination, GetTotalScore(questionIds, x.Id.ToString()))).ToListAsync();
                }
                else
                {
                    var students = await studentService.GetAllClassStudentDetails(examination.CandidateCategoryId_ClassId, examination.ProductBaseurlSuffix);
                    if (students.Result != null)
                    {
                        candidatesResult = students.Result.Select(x => new SelectAllCandidateResult(x, examination, GetTotalScore(questionIds, x.RegistrationNumber.ToString()))).ToList(); 
                    }
                }

                DataTable resultColumn = new DataTable();
                resultColumn.Columns.Add("CandidateId");
                resultColumn.Columns.Add("CandidateName");
                resultColumn.Columns.Add("ExaminationName");
                resultColumn.Columns.Add("TotalScore");
                resultColumn.Columns.Add("Status");

                if (candidatesResult.Count() > 0)
                {
                    foreach (var itemRow in candidatesResult)
                    {
                        var row = resultColumn.NewRow();
                        row["CandidateId"] = itemRow.CandidateId;
                        row["CandidateName"] = itemRow.CandidateName;
                        row["ExaminationName"] = itemRow.ExaminationName;
                        row["TotalScore"] = itemRow.TotalScore;
                        row["Status"] = itemRow.Status;
                        resultColumn.Rows.Add(row);
                    }

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage excelPck = new ExcelPackage())
                    {
                        ExcelWorksheet workSheet = excelPck.Workbook.Worksheets.Add(examination.ExamName_Subject);
                        workSheet.DefaultColWidth = 20;
                        workSheet.Cells["A1"].LoadFromDataTable(resultColumn, true, OfficeOpenXml.Table.TableStyles.None);
                        resultFile = excelPck.GetAsByteArray();
                    }
                    
                }
                res.IsSuccessful = true;
                res.Result = resultFile;
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
        public async Task<APIResponse<PagedResponse<List<SelectCandidateAnswer>>>> GetCandidateAnswers(PaginationFilter filter, string examinationId, string candidateId_regNo, string candidateEmail)
        {
            var res = new APIResponse<PagedResponse<List<SelectCandidateAnswer>>>();
            try
            {
                var questions = await context.Question.Where(x => x.ExaminationId == Guid.Parse(examinationId)).Select(x => x.QuestionId).ToListAsync();
                
                if(string.IsNullOrEmpty(candidateEmail))
                {
                    var candidate = await context.Candidate.FirstOrDefaultAsync(x => x.CandidateId.ToLower() == candidateId_regNo.ToLower());

                    if (candidate == null)
                    {
                        res.Message.FriendlyMessage = "Candidate Id doesn't exist!";
                        return res;
                    }
                    var query = context.CandidateAnswer
                        .Where(d => d.Deleted != true && questions.Contains(d.QuestionId) && d.CandidateId == candidate.Id.ToString())
                        .Include(q => q.Question)
                        .OrderByDescending(s => s.CreatedOn);

                    var totalRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(db => new SelectCandidateAnswer(db)).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                }
                
                if(string.IsNullOrEmpty(candidateId_regNo))
                {
                    var candidate = await context.Candidate.FirstOrDefaultAsync(x => x.Email.ToLower() == candidateEmail.ToLower());

                    if (candidate == null)
                    {
                        res.Message.FriendlyMessage = "Candidate Email doesn't exist!";
                        return res;
                    }
                    var query = context.CandidateAnswer
                        .Where(d => d.Deleted != true && questions.Contains(d.QuestionId) && d.CandidateId == candidate.Id.ToString())
                        .Include(q => q.Question)
                        .OrderByDescending(s => s.CreatedOn);

                    var totalRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(db => new SelectCandidateAnswer(db)).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                }
                
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

        public async Task<APIResponse<List<SelectAdmissionCandidateResult>>> GetAdmissionCandidateResult(string candidateCategoryId)
        {
            var res = new APIResponse<List<SelectAdmissionCandidateResult>>();
            try
            {
                var examination = await context.Examination?.Where(x => x.CandidateCategoryId_ClassId.ToLower() == candidateCategoryId.ToLower() && x.Deleted != true)?.OrderByDescending(x=>x.CreatedOn).FirstOrDefaultAsync();

                if(examination == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Examination has not been created for this category. Kindly create Examination.";
                    return res;
                }
                if ((DateTime.Compare(examination.StartTime, localTime) == -1 || DateTime.Compare(examination.StartTime, localTime) == 0) && DateTime.Compare(examination.EndTime, localTime) == 1)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = $"This examination is in progress and will be concluded on {examination.EndTime}";
                    return res;
                }
                if ((DateTime.Compare(examination.StartTime, localTime) == 1))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = $"This examination is yet to commence. Examination commences on {examination.EndTime}";
                    return res;
                }
                if (examination.Status == (int)ExaminationStatus.Cancelled)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = $"This examination has been cancelled";
                    return res;
                }

                var questionIds = context.Question?.Where(x => x.ExaminationId == examination.ExaminationId)
                    .Select(x => x.QuestionId)
                    .ToList();

                var result = await context.Candidate.Where(x => x.CandidateCategoryId == Guid.Parse(examination.CandidateCategoryId_ClassId))
                    .Select(x => new SelectAdmissionCandidateResult(x, examination, GetTotalScore(questionIds, x.Id.ToString()))).ToListAsync();

                res.Result = result;
                
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

        public async Task<APIResponse<bool>> ResetResult(string examinationId, string candidateId_regNo)
        {
            var res = new APIResponse<bool>();
            try
            {
                var examination = await context.Examination?.Where(x => x.ExaminationId == Guid.Parse(examinationId))?.FirstOrDefaultAsync();
                var candidate = await context.Candidate.Where(x => x.CandidateId.ToLower() == candidateId_regNo.ToLower()).FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(examination.CandidateIds))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Candidate has not taken examination";
                    return res;
                }

                var candidateIds = examination.CandidateIds.Split(",").ToList();
                var candidateIndex = candidateIds.IndexOf(candidate.Id.ToString());

                if (candidateIndex == -1)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Candidate has not taken examination";
                    return res;
                }

                candidateIds.RemoveAt(candidateIndex);
                examination.CandidateIds = string.Join(",", candidateIds);

                var questionIds = await context.Question?.Where(x => x.ExaminationId == Guid.Parse(examinationId))
                  .Select(x => x.QuestionId)
                  .ToListAsync();

                foreach (var item in questionIds)
                {
                    var candidateAnswer = await context.CandidateAnswer?.Where(x => x.QuestionId == item && x.CandidateId == candidate.Id.ToString())?.FirstOrDefaultAsync();
                    if(candidateAnswer != null)
                        context.CandidateAnswer.Remove(candidateAnswer);

                }
                await context.SaveChangesAsync();

                res.Result = true;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = "Successful";
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

        public async Task<APIResponse<SelectResult>> GetCandidateResult(string examinationId, string candidateId_regNo)
        {
            var res = new APIResponse<SelectResult>();
            try
            {
                var examination = await context.Examination?.Where(x => x.ExaminationId == Guid.Parse(examinationId))?.FirstOrDefaultAsync();

                var questionIds = await context.Question?.Where(x => x.ExaminationId == Guid.Parse(examinationId))
                    .Select(x => x.QuestionId)
                    .ToListAsync();

                int totalScore = 0;
                foreach (var item in questionIds)
                {
                    var answer = await context.Question?.Where(x => x.QuestionId == item)?.FirstOrDefaultAsync();
                    var candidateAnswer = await context.CandidateAnswer?.Where(x => x.QuestionId == item && x.CandidateId == candidateId_regNo)?.FirstOrDefaultAsync();

                    if (answer?.Answers == candidateAnswer?.Answers)
                    {
                        totalScore += answer.Mark;
                    }

                }

                string status = "";
                string candidateName = "";

                if (examination.ExaminationType == (int)ExaminationType.InternalExam)
                {
                    status = totalScore >= examination.ExamScore ? "Passed" : "Failed";
                    var student = await studentService.GetStudentDetails(candidateId_regNo, examination.ProductBaseurlSuffix);
                    if (student.Result != null)
                    {
                        candidateName = $"{student.Result.FirstName} {student.Result.LastName}";
                    }
                }
                else
                {
                    status = totalScore >= examination.PassMark ? "Passed" : "Failed";
                    var candidate = await context.Candidate?.Where(x => x.Id == Guid.Parse(candidateId_regNo))?.FirstOrDefaultAsync();
                    candidateName = $"{candidate?.FirstName} {candidate?.LastName}";
                }

                var result = new SelectResult
                {
                    CandidateName = candidateName,
                    CandidateId = candidateId_regNo,
                    ExaminationName = examination.ExamName_Subject,
                    TotalScore = totalScore,
                    Status = status
                };

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
    }
}
