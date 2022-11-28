using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Examinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Examinations
{
    public class SelectExamination
    {
        public string ExaminationId { get; set; }
        public string ExamName_SubjectId { get; set; }
        public string ExamName_Subject { get; set; }
        public string CandidateCategoryId_ClassId { get; set; }
        public string CandidateCategory_Class { get; set; }
        public int ExamScore { get; set; }
        public string CandidateExaminationId { get; set; }
        public TimeSpan Duration { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Instruction { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool UseAsExamScore { get; set; }
        public bool UseAsAssessmentScore { get; set; }
        public string AsExamScoreSessionAndTerm { get; set; }
        public string AsAssessmentScoreSessionAndTerm { get; set; }
        public int Status { get; set; }
        public int ExamintionType { get; set; }

        public SelectExamination(Examination examination)
        {
            ExaminationId = examination.ExaminationId.ToString();
            ExamName_SubjectId = examination.ExamName_SubjectId;
            ExamName_Subject = examination.ExamName_Subject;
            CandidateCategoryId_ClassId = examination.CandidateCategoryId_ClassId;
            CandidateCategory_Class = examination.CandidateCategory_Class;
            ExamScore = examination.ExamScore;
            CandidateExaminationId = examination.CandidateExaminationId;
            Duration = examination.Duration;
            StartTime = examination.StartTime.ToString("dd-MM-yyyy hh:mm");
            EndTime = examination.EndTime.ToString("dd-MM-yyyy hh:mm");
            Instruction = examination.Instruction;
            ShuffleQuestions = examination.ShuffleQuestions;
            UseAsExamScore = examination.UseAsExamScore;
            UseAsAssessmentScore = examination.UseAsAssessmentScore;
            AsExamScoreSessionAndTerm = examination.AsExamScoreSessionAndTerm;
            AsAssessmentScoreSessionAndTerm = examination.AsAssessmentScoreSessionAndTerm;
            ExamintionType = examination.ExaminationType;
            if(examination.StartTime <= DateTime.Now && examination.EndTime > DateTime.Now)
            {
                Status = 1;
            }
            if (examination.StartTime < DateTime.Now && examination.EndTime < DateTime.Now)
            {
                Status = 2;
            }
        }
    }
}
