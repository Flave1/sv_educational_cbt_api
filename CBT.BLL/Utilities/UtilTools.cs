using CBT.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CBT.BLL.Utilities
{
    public class UtilTools
    {
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input)
        {
            if (input == null)
                return input;
            return sWhitespace.Replace(input.ToLower(), "");
        }

        public static IDictionary<string, string> GenerateCandidateId()
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
        public static IDictionary<string, string> GenerateExaminationId()
        {
            try
            {
                var dictionary = new Dictionary<string, string>();
                DataContext _context = new DataContext();
                string lastExamintionId = _context.Examination.Max(x => x.ExaminationNo) ?? "0";

                var newExaminationNo = (int.Parse(lastExamintionId) + 1).ToString();
                var newExaminationExamId = number(newExaminationNo);

                Random random = new();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string rndChars = new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                newExaminationExamId = $"{rndChars}-{newExaminationExamId}";

                dictionary.Add(newExaminationNo, newExaminationExamId);
                return dictionary;
            }
            catch (Exception)
            {
                throw new ArgumentException("Unable to Generate ExaminationId");
            }
        }
        private static string number(string Id)
        {
            if (Id.Length == 1)
                return "000000" + Id;
            if (Id.Length == 2)
                return "00000" + Id;
            if (Id.Length == 3)
                return "0000" + Id;
            if (Id.Length == 4)
                return "000" + Id;
            if (Id.Length == 5)
                return "00" + Id;
            if (Id.Length == 6)
                return "0" + Id;
            if (Id.Length == 7)
                return Id;
            else
                return Id;

        }
    }
}
