using AS.OCR.Commom.Configuration;
using AS.OCR.IService;
using AS.OCR.Model.Business;
using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;

namespace AS.OCR.Api.Filter
{
    public class CustomActionFilter : IAuthorizationFilter, IActionFilter
    {
        //private IExceptionLessLogger _logger { get; }
        private ILogger _Mlogger { get; }
        private IAccountService _accountService { get; }
        public CustomActionFilter(ILoggerFactory loggerFactory, IAccountService accountService)
        {
            _Mlogger = loggerFactory.CreateLogger(typeof(CustomActionFilter));
            _accountService = accountService;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                .Where(s => s.GetType() == typeof(AllowAnonymousAttribute))
                .FirstOrDefault() != null;

            if (!isAllowAnonymous)
            {
                var identity = context.HttpContext.User;
                var AccountId = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.UserData)?.Value;
                var AppId = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid)?.Value;
                var AppSecret = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value;
                if (string.IsNullOrWhiteSpace(AccountId))
                    throw new Exception("找不到账号");

                var account = _accountService.Get(Guid.Parse(AccountId));
                if (account == null || account.Status == 0)
                    throw new Exception("账号不存在或账号未启用");

                AccountInfo.Account = account;
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                var isAllowAnonymous = context.ActionDescriptor.EndpointMetadata
              .Where(s => s.GetType() == typeof(AllowAnonymousAttribute))
              .FirstOrDefault() == null;
                var AppId = context.HttpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid)?.Value;
                var info = context.HttpContext.GetRequestInfo(ExceptionlessClient.Default.Configuration);
                var postData = info?.PostData?.ToString().Replace("\n", "").Replace(" ", "");
                var QueryString = JsonConvert.SerializeObject(info.QueryString);
                var Result = JsonConvert.SerializeObject(context.Result);
                var mes = $@"【本机IP : {info?.Host} 】
【客户端IP : {info?.ClientIpAddress}】 
【接口路由 : {info?.Path}】 
【是否需要认证 ：{isAllowAnonymous}】
【AppID ：{AppId}】
【QueryString : {QueryString}】 
【PostData : {postData}】 
【出参 : {Result}】";
                if (ConfigurationUtil.Exceptionless_Enabled)
                    ExceptionlessClient.Default.CreateLog(mes, Exceptionless.Logging.LogLevel.Info).Submit();
                else
                    _Mlogger.LogInformation(mes);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //var identity = context.HttpContext.User;
            //var id = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.UserData)?.Value;
            //var AppId = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid)?.Value;
            //var AppSecret = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value;
        }
    }
}
