using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.CandidateAnswers
{
    public class SubmitAllAnswers
    {
        public string CandidateId { get; set; }
        public List<Answer> Answers { get; set; }
    }
    public class Answer
    {
        public string QuestionId { get; set; }
        public int[] Answers { get; set; }
    }
}
