using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Examination
{
    public class UpdateExamination: CreateExamination
    {
        public Guid ExaminationId { get; set; }
    }
}
