using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AS.OCR.IService;
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
    public class AuthController : BaseController
    {
        private IAuthService _authService { get; }
        private ILogger _logger;

        public AuthController(ILoggerFactory loggerFactory, IAuthService authService)
        {
            _logger = loggerFactory.CreateLogger(typeof(AuthController));
            _authService = authService;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="appid">账号id</param>
        /// <param name="appsecret">账号秘钥</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Token")]
        public IActionResult Token(string appid, string appsecret)
        {
            if (string.IsNullOrWhiteSpace(appid) || string.IsNullOrWhiteSpace(appsecret))
                return FailRes("参数错误");

            return SuccessRes(_authService.CreateToken(appid, appsecret));
        }

        /// <summary>
        /// 创建AppId,AppSecret
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("CreateAppIdSecret")]
        public IActionResult CreateAppIdSecret(string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
                return FailRes("参数错误");
            var base64Str1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(name));
            var base64Str2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
            var AppIdSecret = new AppIdSecret { };
            using (var md5 = MD5.Create())
            {
                var result = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(base64Str1))).Replace("-", "");
                var result2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(base64Str2))).Replace("-", "");
                AppIdSecret.appid = result.ToLower();
                AppIdSecret.secret = result2.ToLower();
            }
            return SuccessRes(AppIdSecret);
        }
        public class AppIdSecret
        {
            public string appid { get; set; }
            public string secret { get; set; }
        }



    }
}