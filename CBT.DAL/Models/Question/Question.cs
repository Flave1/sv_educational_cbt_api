using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.DAL.Models.Question
{
    public class Question : CommonEntity
    {
        [Key]
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public Guid ExaminationId { get; set; }
        public int Mark { get; set; }
        public string Options { get; set; }
        public string Answers { get; set; }
        public int QuestionType { get; set; }
        public int UserType { get; set; }
        public Guid ClientId { get; set; }
    }
}
