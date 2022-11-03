using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Question
{
    public class UpdateQuestion: CreateQuestion
    {
        public Guid QuestionId { get; set; }
    }
}
