using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts
{
    public class APIResponse<T>
    {
        public APIResponse()
        {
            Message = new APIResponseMessage();
        }
        public bool IsSuccessful { get; set; }
        public T Result { get; set; }
        public string Status { get; set; }
        public APIResponseMessage Message { get; set; }
    }

    public class APIResponseMessage
    {
        public string FriendlyMessage { get; set; }
        public string TechnicalMessage { get; set; }
    }

    public class ErrorModel
    {
        public string FieldName { get; set; }
        public APIResponse<ErrorModel> Status { get; set; }
    }
}
