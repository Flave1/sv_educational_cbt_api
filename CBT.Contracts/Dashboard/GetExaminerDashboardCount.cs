namespace CBT.Contracts.Dashboard
{
    public class GetExaminerDashboardCount
    {
        public long TotalCandidate { get; set; }
        public long TotalCategory { get; set; }
        public long ActiveExams { get; set; }
        public long ConcludedExams { get; set; }
    }
}
