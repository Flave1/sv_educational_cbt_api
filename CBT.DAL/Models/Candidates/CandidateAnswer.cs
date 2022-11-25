using System;
using System.ComponentModel.DataAnnotations;

namespace CBT.DAL.Models.Candidate
{
    public class CandidateAnswer : CommonEntity
    {
        [Key]
        public Guid AnswerId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid CandidateId { get; set; }
        public string Answers { get; set; }
    }
}
