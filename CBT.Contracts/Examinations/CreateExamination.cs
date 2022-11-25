﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Examinations
{
    public class CreateExamination
    {
        public string ExamName_SubjectId { get; set; }
        public string ExamName_Subject { get; set; }
        public string CandidateCategoryId_ClassId { get; set; }
        public string CandidateCategory_Class { get; set; }
        public int ExamScore { get; set; }
        public string Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Instruction { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool UseAsExamScore { get; set; }
        public bool UseAsAssessmentScore { get; set; }
        public int Status { get; set; }
    }
}