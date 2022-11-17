using CBT.DAL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Utilities
{
    public static class CandidateExamId
    {
        public static DataContext _context;

        public static void Initialize(DataContext context)
        {
            _context = context;
        }
        public static string Generate(int candidateId)
        {
            try
            {
                var newCandidateExamId = candidateId.ToString();
                newCandidateExamId = number(newCandidateExamId);

                Random random = new();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string rndChars = new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                newCandidateExamId = $"{rndChars}-{newCandidateExamId}";
                return newCandidateExamId;
            }
            catch (Exception)
            {
                throw new ArgumentException("Unable to Generate CandidateExamId");
            }
        }
        private static string number(string examId)
        {
            if (examId.Length == 1)
                return "000000" + examId;
            if (examId.Length == 2)
                return "00000" + examId;
            if (examId.Length == 3)
                return "0000" + examId;
            if (examId.Length == 4)
                return "000" + examId;
            if (examId.Length == 5)
                return "00" + examId;
            if (examId.Length == 6)
                return "0" + examId;
            if (examId.Length == 7)
                return examId;
            else
                return examId;

        }
    }
}
