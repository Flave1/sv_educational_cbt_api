using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Student
{
    public class GetStudentDetails
    {
        public string StudentAccountId { get; set; }
        public int UserType { get; set; }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Photo { get; set; }
        public string HomePhone { get; set; }
        public string EmergencyPhone { get; set; }
        public string ParentOrGuardianName { get; set; }
        public string ParentOrGuardianRelationship { get; set; }
        public string ParentOrGuardianPhone { get; set; }
        public string ParentOrGuardianEmail { get; set; }
        public string HomeAddress { get; set; }
        public string CityId { get; set; }
        public string StateId { get; set; }
        public string CountryId { get; set; }
        public string ZipCode { get; set; }
        public string RegistrationNumber { get; set; }
        public string UserAccountId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
