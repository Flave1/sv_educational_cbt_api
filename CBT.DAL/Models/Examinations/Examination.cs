using CBT.DAL.Models.Questions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.DAL.Models.Examinations
{
    public class Examination : CommonEntity
    {
        [Key]
        public Guid ExaminationId { get; set; }
        public string ExamName_SubjectId { get; set; }
        public string ExamName_Subject { get; set; }
        public string CandidateCategoryId_ClassId { get; set; }
        public string CandidateCategory_Class { get; set; }
        public string ExaminationNo { get; set; }
        public string CandidateExaminationId { get; set; }
        public int ExamScore { get; set; }
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
        public  int ExaminationType { get; set; }
        public int PassMark { get; set; }
        public string ProductBaseurlSuffix { get; set; }
        public string CandidateIds { get; set; }
        public virtual ICollection<Question> Question { get; set; }
    }
}
