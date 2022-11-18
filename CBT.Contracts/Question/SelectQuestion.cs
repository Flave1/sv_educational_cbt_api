using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Question
{
    public class SelectQuestion
    {
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string ExaminationId { get; set; }
        public int Mark { get; set; }
        public string[] Options { get; set; }
        public string[] Answers { get; set; }
        public int QuestionType { get; set; }
    }
}
