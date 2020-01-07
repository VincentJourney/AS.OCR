using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AS.OCR.Model.Response;
using AS.OCR.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AS.OCR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AuthController : ControllerBase
    {
        private static readonly AuthService authService = new AuthService();
        private ILogger logger;
        public AuthController(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger(typeof(AuthController));
        }
        //[HttpGet("CreateAppIdSecret")]
        //public Result<AppIdSecret> CreateAppIdSecret(string name, string password)
        //{
        //    var base64Str1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(name));
        //    var base64Str2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        //    var AppIdSecret = new AppIdSecret { };
        //    using (var md5 = MD5.Create())
        //    {
        //        var result = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(base64Str1))).Replace("-", "");
        //        var result2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(base64Str2))).Replace("-", "");
        //        AppIdSecret.appid = result.ToLower();
        //        AppIdSecret.secret = result2.ToLower();
        //    }
        //    return Result<AppIdSecret>.SuccessRes(data: AppIdSecret);
        //}
        //public class AppIdSecret
        //{
        //    public string appid { get; set; }
        //    public string secret { get; set; }
        //}


        [AllowAnonymous]
        [HttpGet("Token")]
        public IActionResult Token(string appid, string appsecret)
        {
            if (string.IsNullOrWhiteSpace(appid) || string.IsNullOrWhiteSpace(appsecret))
                return BadRequest(Result<string>.ErrorRes("参数错误"));

            return Ok(authService.CreateToken(appid, appsecret));
        }
    }
}