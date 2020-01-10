using AS.OCR.Commom.Configuration;
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
    /// 全局异常
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
            var postData = info?.PostData?.ToString().Replace("\n", "").Replace(" ", "");
            var QueryString = JsonConvert.SerializeObject(info.QueryString);
            var mes = $@"【本机IP : {info?.Host} 】
【客户端IP地址 : {info?.ClientIpAddress}】 
【接口路由 : {info?.Path}】
【QueryString : {QueryString}】
【PostData : {postData}】
【异常信息 ：{e.Message}】
【堆栈信息 ：{e.StackTrace}】";
            if (ConfigurationUtil.Exceptionless_Enabled)
                e.ToExceptionless().AddObject(mes, "HttpContextInfo").Submit();
            else
                _log.LogError(mes);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response
                .WriteAsync(JsonConvert.SerializeObject(Result<string>.ErrorRes(e.Message)))
                .ConfigureAwait(false);
        }
    }
}
