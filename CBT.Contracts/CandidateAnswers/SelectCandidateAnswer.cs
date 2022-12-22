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
        public int Mark { get; set; }
        public string CandidateId { get; set; }
        public string[] Options { get; set; }
        public string[] Answers { get; set; }
        public string[] CandidateAnswers { get; set; }
        public SelectCandidateAnswer(CandidateAnswer answer)
        {
            AnswerId = answer.AnswerId.ToString();
            QuestionId = answer.QuestionId.ToString();
            QuestionText = answer.Question.QuestionText;
            Mark = answer.Question.Mark;
            CandidateId = answer.CandidateId.ToString();
            Options = !string.IsNullOrEmpty(answer.Question.Options) ? answer.Question.Options.Split("</option>").ToArray() : Array.Empty<string>();
            Answers = !string.IsNullOrEmpty(answer.Question.Answers) ? answer.Question.Answers.Split(",").ToArray() : Array.Empty<string>();
            CandidateAnswers = !string.IsNullOrEmpty(answer.Answers) ? answer.Answers.Split(",").ToArray() : Array.Empty<string>();

            //AnswersValue = answer.Answers.Split(",").ToArray();

            //string[] arr = new string[AnswersValue.Count()];
            //int count = 0;
            //foreach (string item in AnswersValue)
            //{
            //    arr[count] = answer.Question.Options.Split("</option>").ToArray()[int.Parse(item)];
            //    count++;
            //}
            //AnswersValue = arr;
        }
    }
}
