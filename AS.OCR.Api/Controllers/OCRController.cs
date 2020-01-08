﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AS.OCR.Extension.SDK.TencentOCR;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using AS.OCR.Model.Request;
using AS.OCR.Model.Response;
using AS.OCR.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AS.OCR.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class OCRController : BaseController
    {
        private ILogger _logger { get; set; }
        private OCRService oCRService;
        public OCRController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(OCRController));
            oCRService = new OCRService(loggerFactory);
        }

        /// <summary>
        /// 图片识别 
        /// </summary>
        /// <param name="receiptRequest"></param>
        /// <returns></returns>
        [HttpPost("Receipt")]
        public async Task<IActionResult> Receipt([FromBody]ReceiptRequest receiptRequest)
        {
            if (receiptRequest == null) throw new Exception("参数错误");
            return SuccessRes(await oCRService.ReceiptOCR(receiptRequest));
        }

        /// <summary>
        /// 测试腾讯云OCR
        /// </summary>
        /// <param name="tencentCloudOCRRequest"></param>
        /// <returns></returns>
        [HttpPost("TencentCloudOCR")]
        public IActionResult TencentCloudOCR([FromBody]TencentCloudOCRRequest tencentCloudOCRRequest)
        {
            if (tencentCloudOCRRequest == null) throw new Exception("参数错误");
            return SuccessRes(TencentOCR.GeneralAccurateOCR(tencentCloudOCRRequest.Image, tencentCloudOCRRequest.Type));
        }

        [HttpGet("values")]
        public IActionResult Get()
        {
            var identity = HttpContext.User;
            var id = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.UserData).Value;
            var AppId = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Sid).Value;
            var AppSecret = identity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid).Value;
            return SuccessRes(new { id = id, appid = AppId, AppSecret = AppSecret });
        }
    }
}