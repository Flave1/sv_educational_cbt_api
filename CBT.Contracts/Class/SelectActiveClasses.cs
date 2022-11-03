using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Class
{
    public class SelectActiveClasses
    {
        public string SessionClassId { get; set; }
        public string SessionId { get; set; }
        public string Session { get; set; }
        public string ClassId { get; set; }
        public string Class { get; set; }
        public string FormTeacherId { get; set; }
        public string FormTeacher { get; set; }
        public string ClassCaptainId { get; set; }
        public string ClassCaptain { get; set; }
        public bool InSession { get; set; }
        public int ExamScore { get; set; }
        public int AssessmentScore { get; set; }
        public int PassMark { get; set; }
        public int SubjectCount { get; set; }
        public int StudentCount { get; set; }
        public int AttendanceCount { get; set; }
        public int AssessmentCount { get; set; }
        public ClassSubjects[] ClassSubjects { get; set; } = new ClassSubjects[0];
    }
    public class ClassSubjects
    {
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectTeacherId { get; set; }
        public string SubjectTeacherName { get; set; }
        public int ExamSCore { get; set; }
        public int Assessment { get; set; }
    }
}
