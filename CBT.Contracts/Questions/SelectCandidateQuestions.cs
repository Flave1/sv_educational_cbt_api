using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Questions
{
    public class SelectCandidateQuestions
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
        public SelectCandidateQuestions(Question question, CandidateAnswer candidateAnswers)
        {
            QuestionId = question.QuestionId.ToString();
            QuestionText = question.QuestionText;
            ExaminationId = question.ExaminationId.ToString();
            ExaminationName = question.Examination.ExamName_Subject;
            CandidateCategoryId = question.Examination.CandidateCategoryId_ClassId.ToString();
            CandidateCategory = question.Examination.CandidateCategory_Class;
            Mark = question.Mark;
            Options = !string.IsNullOrEmpty(question.Options) ? question.Options.Split("</option>").ToArray() : Array.Empty<string>();
            Answers = !string.IsNullOrEmpty(candidateAnswers?.Answers) ? candidateAnswers?.Answers.Split(",").ToArray() : Array.Empty<string>();
            QuestionType = question.QuestionType;

            string[] arr = new string[Answers.Count()];
            int count = 0;
            foreach (string answer in Answers)
            {
                arr[count] = Options[int.Parse(answer)];
                count++;
            }
            AnswersValue = arr;
        }
    }
}
