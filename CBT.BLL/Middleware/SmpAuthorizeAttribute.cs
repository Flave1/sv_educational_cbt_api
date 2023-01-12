using CBT.BLL.Constants;
using CBT.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SmpAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                StringValues userId= context.HttpContext.Request.Headers["userId"];
                StringValues smsClientId = context.HttpContext.Request.Headers["smsClientId"];
                StringValues productBaseurlSuffix = context.HttpContext.Request.Headers["productBaseurlSuffix"];

                StringValues examinationId = context.HttpContext.Request.Headers["examinationId"];
                StringValues candidateId_regNo = context.HttpContext.Request.Headers["candidateId_regNo"];

                bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
                if (context == null || hasAllowAnonymous)
                {
                    await next();
                    return;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new UnauthorizedObjectResult(new APIResponse<APIResponseMessage> { Message = new APIResponseMessage { FriendlyMessage = "Unauthorized access" } });
                    return;
                }

                if (string.IsNullOrEmpty(smsClientId))
                {
                    context.HttpContext.Items["userType"] = (int)UserType.NonSMSUser;
                }
                else
                {
                    context.HttpContext.Items["userType"] = (int)UserType.SMSUser;
                }
                context.HttpContext.Items["userId"] = userId;
                context.HttpContext.Items["productBaseurlSuffix"] = productBaseurlSuffix;
                context.HttpContext.Items["smsClientId"] = smsClientId;

                context.HttpContext.Items["examinationId"] = examinationId;
                context.HttpContext.Items["candidateId_regNo"] = candidateId_regNo;
                await next();
                return;
            }
            catch (Exception ex)
            {
                context.HttpContext.Response.StatusCode = 500;
                return;
            }
        }
    }
}
