﻿using CBT.Contracts.Student;
using CBT.DAL.Models.Examinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Result
{
    public class SelectAllStudentResult
    {
        public string CandidateName { get; set; }
        public string CandidateId { get; set; }
        public string ExaminationName { get; set; }
        public int TotalScore { get; set; }
        public string Status { get; set; }
        public SelectAllStudentResult(StudentData student, Examination examination, int totalScore)
        {
            CandidateId = student.RegistrationNumber;
            CandidateName = $"{student.FirstName} {student.LastName}";
            ExaminationName = examination.ExamName_Subject;
            TotalScore = totalScore;
            if(string.IsNullOrEmpty(examination.CandidateIds))
            {
                Status = "Not Taken";
            }
            else
            {
                Status = examination.CandidateIds.Split(",").Contains(student.RegistrationNumber) ? "Submitted" : "Not Taken";
            }
        }
    }
}
