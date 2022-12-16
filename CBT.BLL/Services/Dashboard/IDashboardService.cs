using CBT.Contracts;
using CBT.Contracts.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<APIResponse<GetExaminerDashboardCount>> GetExaminerDashboardCount();
    }
}
