using CBT.Contracts.Authentication;
using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Examinations;
using CBT.DAL.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Questions
{
    public class SelectQuestion
    {
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string ExaminationId { get; set; }
        public string ExaminationName { get; set; }
        public string CandidateCategoryId { get; set; }
        public string CandidateCategory { get; set; }
        public int Mark { get; set; }
        public string[] Options { get; set; }
        public string[] Answers { get; set; }
        public string[] AnswersValue { get; set; }
        public int QuestionType { get; set; }
        public SelectQuestion(Question question)
        {
            QuestionId = question.QuestionId.ToString();
            QuestionText = question.QuestionText;
            ExaminationId = question.ExaminationId.ToString();
            ExaminationName = question.Examination.ExamName_Subject;
            CandidateCategoryId = question.Examination.CandidateCategoryId_ClassId.ToString();
            CandidateCategory = question.Examination.CandidateCategory_Class;
            Mark = question.Mark;
            Options = !string.IsNullOrEmpty(question.Options) ? question.Options.Split("</option>").ToArray() : Array.Empty<string>();
            Answers = !string.IsNullOrEmpty(question.Answers) ? question.Answers.Split(",").ToArray() : Array.Empty<string>();
            AnswersValue = question.Answers.Split(",").ToArray();
            QuestionType = question.QuestionType;

            string[] arr = new string[AnswersValue.Count()];
            int count = 0;
            foreach (string answer in AnswersValue)
            {
                arr[count] = Options[int.Parse(answer)];
                count++;
            }
            AnswersValue = arr;
        }
    }
}
