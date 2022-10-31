using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.DAL.Models.Examination
{
    public class Examination : CommonEntity
    {
        [Key]
        public Guid ExaminationId { get; set; }
        public string ExamName_SubjectId { get; set; }
        public string ExamName_Subject { get; set; }
        public string CandidateCategoryId_ClassId { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Instruction { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool UseAsExamScore { get; set; }
        public bool UseAsAssessmentScore { get; set; }
        public string AsExamScoreSessionAndTerm { get; set; }
        public string AsAssessmentScoreSessionAndTerm { get; set; }
        public int UserType { get; set; }
        public int Status { get; set; }
        public Guid ClientId { get; set; }
    }
}
