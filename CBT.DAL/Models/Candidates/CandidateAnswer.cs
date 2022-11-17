using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.DAL.Models.Candidate
{
    public class CandidateAnswer : CommonEntity
    {
        [Key]
        public Guid AnswerId { get; set; }
        public Guid QuestionId { get; set; }
        public int CandidateId { get; set; }
        public string Answers { get; set; }
        public Guid ClientId { get; set; }
    }
}
