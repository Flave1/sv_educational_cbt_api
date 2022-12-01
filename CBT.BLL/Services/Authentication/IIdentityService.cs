using CBT.Contracts;
using CBT.Contracts.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Authentication
{
    public interface IIdentityService
    {
        Task<APIResponse<LoginDetails>> LoginAsync(LoginCommand user);
    }
}
