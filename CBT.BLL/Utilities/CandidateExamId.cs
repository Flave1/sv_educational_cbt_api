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
        public static string Generate()
        {
            try
            {
                var lastCandidateExamId = _context.Candidate.Max(d => d.CandidateExamId) ?? "1";
                var newCandidateExamId = (lastCandidateExamId == "1" ? 1 : long.Parse(lastCandidateExamId) + 1).ToString();

                newCandidateExamId = number(newCandidateExamId);

                return newCandidateExamId;
            }
            catch (Exception)
            {
                throw new ArgumentException("Unable to Generate CandidateExamId");
            }
        }
        private static string number(string regNo)
        {
            if (regNo.Length == 1)
                return "000000" + regNo;
            if (regNo.Length == 2)
                return "00000" + regNo;
            if (regNo.Length == 3)
                return "0000" + regNo;
            if (regNo.Length == 4)
                return "000" + regNo;
            if (regNo.Length == 5)
                return "00" + regNo;
            if (regNo.Length == 6)
                return "0" + regNo;
            if (regNo.Length == 7)
                return regNo;
            else
                return regNo;

        }
    }
}
