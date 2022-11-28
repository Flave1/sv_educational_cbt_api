using CBT.DAL.Models.Examinations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBT.DAL.Models.Questions
{
    public class Question : CommonEntity
    {
        [Key]
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public Guid ExaminationId { get; set; }
        [ForeignKey("ExaminationId")]
        public int Mark { get; set; }
        public string Options { get; set; }
        public string Answers { get; set; }
        public int QuestionType { get; set; }
        public Examination Examination { get; set; }
    }
}
