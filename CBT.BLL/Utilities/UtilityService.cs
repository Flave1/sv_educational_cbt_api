using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Utilities
{
    public class UtilityService : IUtilityService
    {
        public DateTime GetCurrentLocalDateTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "W. Central Africa Standard Time");
            return localTime;
        }
    }
}
