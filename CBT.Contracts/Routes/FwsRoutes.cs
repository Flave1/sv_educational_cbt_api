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
        public const string loginByHash = "fws/client/fws/user/api/v1/login-by-hash";
        public const string subjectSelect = "/subject/api/v1/getall/active-subject";
        public const string sessionSubjectSelect = "/class/api/v1/getall/class-subjects-cbt?sessionClassId=";
        public const string classSelect = "/class/api/v1/get-all/session-classes-cbt?clientId=";
        public const string activeSessionSelect = "/session/api/v1/get-active-cbt?examScore=";
        public const string classByRegNoSelect = "/class/api/v1/get/session-class-by-reg-no-cbt?regNo=";
        public const string studentByRegNoSelect = "/student/api/v1/get-student-contact-cbt?studentRegNo=";
        public const string classStudentsSelect = "/student/api/v1/getall-student-contact-cbt?";
        public const string allClassStudentsSelect = "/student/api/v1/getall-students-cbt?";
    }
}
