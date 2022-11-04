using CBT.Contracts.Candidates;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Subject;
using CBT.Contracts.Class;

namespace CBT.BLL.Services.Class
{
    public interface IClassService
    {
        Task<APIResponse<List<SelectActiveClasses>>> GetActiveClasses(string productBaseurlSuffix);
    }
}
