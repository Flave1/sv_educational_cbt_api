using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Questions;

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
        public QuestionOptions[] Options { get; set; }
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
            Options = !string.IsNullOrEmpty(question.Options) ? question.Options.Split("</option>").Select(x => new QuestionOptions
            {
                Option = x,
                Index = Array.FindIndex(question.Options.Split("</option>").ToArray(), c => c == x)
            }).ToArray() : Array.Empty<QuestionOptions>();
            Answers = !string.IsNullOrEmpty(candidateAnswers?.Answers) ? candidateAnswers?.Answers.Split(",").ToArray() : Array.Empty<string>();
            QuestionType = question.QuestionType;

            QuestionOptions[] arr = new QuestionOptions[Answers.Count()];
            for (int i = 0; i < Answers.Length; i++)
            {
                arr[i] = Options[int.Parse(Answers[i])];
            }
            //int count = 0;
            //foreach (string answer in Answers)
            //{
            //    arr[count] = Options[int.Parse(answer)];
            //    count++;
            //}
            AnswersValue = arr.Select(x => x.Option).ToArray();
        }
    }

    public class QuestionOptions
    {
        public int Index { get; set; }
        public string Option { get; set; }
    }
}
