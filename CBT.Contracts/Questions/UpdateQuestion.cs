using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Questions
{
    public class UpdateQuestion: CreateQuestion
    {
        public Guid QuestionId { get; set; }
    }
}
