using AS.OCR.Commom.Util;
using AS.OCR.Model.Response;
using Exceptionless;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AS.OCR.Api.Middleware
{
    /// <summary>
    /// 全局
    /// </summary>
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _log;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _log = loggerFactory.CreateLogger(typeof(CustomExceptionHandlerMiddleware));
        }

        public async Task Invoke(HttpContext contexts)
        {
            try
            {
                await _next(contexts);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(contexts, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            if (e == null) return;

            var info = context.GetRequestInfo(ExceptionlessClient.Default.Configuration);
            var postData = info?.PostData?.ToString();
            var data = postData.Replace("\n", "").Replace(" ", "");
            var mes = $@"【本机IP : {info?.Host} 】| 【接口路由 : {info?.Path}】 | 【客户端IP地址 : {info?.ClientIpAddress}】 | 【入参 : {data}】 | 【出参 : 】";

            var exceptionMes = $"{mes} | 【异常信息 ： {e.Message}】 | 【堆栈信息 ： {e.StackTrace}】";

            if (ConfigurationUtil.Exceptionless_Enabled)
                e.ToExceptionless().AddObject(mes, "HttpContextInfo").Submit();
            else
                _log.LogError(exceptionMes);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context
                .Response
                .WriteAsync(JsonConvert.SerializeObject(Result<string>.ErrorRes(e.Message)))
                .ConfigureAwait(false);
        }
    }
}
