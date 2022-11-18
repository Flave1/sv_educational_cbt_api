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
        public static IDictionary<string, string> Generate()
        {
            try
            {
                var dictionary = new Dictionary<string, string>();
                DataContext _context = new DataContext();
                string lastCandidateId = _context.Candidate.Max(x => x.CandidateNo) ?? "0";
               
                var newCandidateNo = (int.Parse(lastCandidateId) + 1).ToString();
                var newCandidateExamId = number(newCandidateNo);

                Random random = new();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string rndChars = new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                newCandidateExamId = $"{rndChars}-{newCandidateExamId}";

                dictionary.Add(newCandidateNo, newCandidateExamId);
                return dictionary;
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
