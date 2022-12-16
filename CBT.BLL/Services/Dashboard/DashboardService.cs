using CBT.BLL.Constants;
using CBT.Contracts;
using CBT.Contracts.Dashboard;
using CBT.Contracts.Examinations;
using CBT.DAL;
using CBT.DAL.Models.Examinations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public DashboardService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }
        public async Task<APIResponse<GetExaminerDashboardCount>> GetExaminerDashboardCount()
        {
            var res = new APIResponse<GetExaminerDashboardCount>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var totalCandidate = context.Candidate.Where(x => x.Deleted != true && x.ClientId == clientId).AsEnumerable().Count();
                var totalCategory = context.CandidateCategory.Where(x => x.Deleted != true && x.ClientId == clientId).AsEnumerable().Count();
                var activeExams = context.Examination.Where(x => x.Deleted != true && x.ClientId == clientId &&
                x.StartTime <= DateTime.Now && x.EndTime > DateTime.Now).AsEnumerable().Count();
                
                var concludedExams = context.Examination.Where(x => x.Deleted != true && x.ClientId == clientId &&
                x.StartTime < DateTime.Now && x.EndTime < DateTime.Now).AsEnumerable().Count();

                var result = new GetExaminerDashboardCount
                {
                    TotalCandidate = totalCandidate,
                    TotalCategory = totalCategory,
                    ActiveExams = activeExams,
                    ConcludedExams= concludedExams
                };

                res.IsSuccessful = true;
                res.Result = result;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }
    }
}
