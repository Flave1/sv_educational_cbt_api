using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Routes
{
    public class FwsRoutes
    {
        public const string countrySelect = "fws/lookups/api/v1/get/country-select";
        public const string login = "fws/user/api/v1/login";
        public const string subjectSelect = "/subject/api/v1/getall/active-subject";
        public const string classSelect = "/class/api/v1/get-all/session-classes2";
    }
}
