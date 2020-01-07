using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AS.OCR.Api.Middleware
{
    public class AuthorizationFilter : IAuthorizationFilter, IActionFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var identity = context.HttpContext.User;
            var id = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.UserData)?.Value;
            var AppId = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid)?.Value;
            var AppSecret = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value;
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
            var identity = context.HttpContext.User;
            var id = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.UserData)?.Value;
            var AppId = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid)?.Value;
            var AppSecret = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var identity = context.HttpContext.User;
            var id = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.UserData)?.Value;
            var AppId = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid)?.Value;
            var AppSecret = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value;
        }
    }
}
