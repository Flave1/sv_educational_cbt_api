using CBT.Contracts.Student;
using CBT.DAL.Models.Candidates;
using CBT.DAL.Models.Examinations;
using CBT.DAL.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Result
{
    public class SelectAllCandidateResult
    {
        public string CandidateName { get; set; }
        public string CandidateId { get; set; }
        public string CandidateEmail { get; set; }
        public string ExaminationName { get; set; }
        public int TotalScore { get; set; }
        public string Status { get; set; }
        public SelectAllCandidateResult(Candidate candidate, Examination examination, int totalScore)
        {
            CandidateId = candidate.CandidateId;
            CandidateName = $"{candidate.FirstName} {candidate.LastName}";
            ExaminationName = examination.ExamName_Subject;
            CandidateEmail = candidate.Email;
            if (string.IsNullOrEmpty(examination.CandidateIds))
            {
                Status = "Not Taken";
                TotalScore = 0;
            }
            else
            {
                if(examination.CandidateIds.Split(",").Contains(candidate.Id.ToString()))
                {
                    Status = totalScore >= examination.PassMark ? "Passed" : "Failed";
                    TotalScore = totalScore;
                }
                else
                {
                    Status = "Not Taken";
                    TotalScore = 0;
                }
            }
        }
        public SelectAllCandidateResult(StudentData student, Examination examination, int totalScore)
        {
            CandidateId = student.RegistrationNumber;
            CandidateName = $"{student.FirstName} {student.LastName}";
            ExaminationName = examination.ExamName_Subject;
            TotalScore = totalScore;
            if (string.IsNullOrEmpty(examination.CandidateIds))
            {
                Status = "Not Taken";
            }
            else
            {
                Status = examination.CandidateIds.Split(",").Contains(student.RegistrationNumber) ? "Submitted" : "Not Taken";
            }
        }
    }
}
