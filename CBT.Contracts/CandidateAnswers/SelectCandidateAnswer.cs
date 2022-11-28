using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.CandidateAnswers
{
    public class SelectCandidateAnswer
    {
        public string AnswerId { get; set; }
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string CandidateId { get; set; }
        public string[] Answers { get; set; }
        public string[] AnswersValue { get; set; }
        public SelectCandidateAnswer(CandidateAnswer answer)
        {
            AnswerId = answer.AnswerId.ToString();
            QuestionId = answer.QuestionId.ToString();
            QuestionText = answer.Question.QuestionText;
            CandidateId = answer.CandidateId.ToString();
            Answers = !string.IsNullOrEmpty(answer.Answers) ? answer.Answers.Split(",").ToArray() : Array.Empty<string>();
            AnswersValue = answer.Answers.Split(",").ToArray();

            string[] arr = new string[AnswersValue.Count()];
            int count = 0;
            foreach (string item in AnswersValue)
            {
                arr[count] = answer.Question.Options.Split("</option>").ToArray()[int.Parse(item)];
                count++;
            }
            AnswersValue = arr;
        }
    }
}
