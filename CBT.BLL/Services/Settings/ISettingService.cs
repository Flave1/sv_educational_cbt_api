using CBT.Contracts.Category;
using CBT.Contracts.Common;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Settings;

namespace CBT.BLL.Services.Settings
{
    public interface ISettingService
    {
        Task<APIResponse<CreateSettings>> CreateSettings(CreateSettings request);
        Task<APIResponse<SelectSettings>> GetSettings();
        Task<APIResponse<bool>> DeleteSettings(SingleDelete request);
    }
}
