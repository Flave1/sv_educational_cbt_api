using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Routes
{
    public class FwsRoutes
    {
        public const string login = "fws/client/fws/user/api/v1/login";
        public const string subjectSelect = "/subject/api/v1/getall/active-subject";
        public const string classSelect = "/class/api/v1/get-all/session-classes-cbt";
        public const string activeSessionSelect = "/session/api/v1/get-active-cbt?examScore=";
        public const string classByRegNoSelect = "/class/api/v1/get-all/session-class-by-reg-no-cbt?regNo=";
    }
}
