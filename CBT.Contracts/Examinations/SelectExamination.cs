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
        public decimal ExamScore { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Instruction { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool UseAsExamScore { get; set; }
        public bool UseAsAssessmentScore { get; set; }
        public string AsExamScoreSessionAndTerm { get; set; }
        public string AsAssessmentScoreSessionAndTerm { get; set; }
        public int Status { get; set; }

        public SelectExamination(Examination examination)
        {
            ExaminationId = examination.ExaminationId.ToString();
            ExamName_SubjectId = examination.ExamName_SubjectId;
            ExamName_Subject = examination.ExamName_Subject;
            CandidateCategoryId_ClassId = examination.CandidateCategoryId_ClassId;
            CandidateCategory_Class = examination.CandidateCategory_Class;
            ExamScore = examination.ExamScore;
            Duration = examination.Duration;
            StartTime = examination.StartTime;
            EndTime = examination.EndTime;
            Instruction = examination.Instruction;
            ShuffleQuestions = examination.ShuffleQuestions;
            UseAsExamScore = examination.UseAsExamScore;
            UseAsAssessmentScore = examination.UseAsAssessmentScore;
            AsExamScoreSessionAndTerm = examination.AsExamScoreSessionAndTerm;
            AsAssessmentScoreSessionAndTerm = examination.AsAssessmentScoreSessionAndTerm;
            Status = examination.Status;
        }
    }
}
