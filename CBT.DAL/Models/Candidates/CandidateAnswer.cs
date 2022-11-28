using CBT.DAL.Models.Questions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBT.DAL.Models.Candidate
{
    public class CandidateAnswer : CommonEntity
    {
        [Key]
        public Guid AnswerId { get; set; }
        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Guid CandidateId { get; set; }
        public string Answers { get; set; }
        public Question Question { get; set; }
    }
}
