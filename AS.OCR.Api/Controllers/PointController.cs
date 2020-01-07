﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AS.OCR.Model.Business;
using AS.OCR.Model.Request;
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
    [Authorize]
    public class PointController : BaseController
    {
        private ILogger _logger { get; set; }
        private OCRService oCRService;
        public PointController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(OCRController));
            oCRService = new OCRService(loggerFactory);
        }

        /// <summary>
        /// 积分申请
        /// </summary>
        /// <param name="applyPointRequest"></param>
        /// <returns></returns>
        [HttpPost("ApplyPoint")]
        public IActionResult ApplyPoint([FromBody]CreateApplyPointRequest applyPointRequest)
        {
            if (applyPointRequest == null ||
                string.IsNullOrWhiteSpace(applyPointRequest.cardId) ||
                 string.IsNullOrWhiteSpace(applyPointRequest.mallId) ||
                applyPointRequest.receiptOCR == null)
                throw new Exception("参数错误");
            return SuccessRes(oCRService.CreateApplyPoint(applyPointRequest));
        }

        /// <summary>
        /// 积分申请历史记录
        /// </summary>
        /// <param name="applyPointHistoryRequest"></param>
        /// <returns></returns>
        [HttpPost("ApplyPointHistory")]
        public IActionResult ApplyPointHistory([FromBody]ApplyPointHistoryRequest applyPointHistoryRequest)
        {
            if (applyPointHistoryRequest == null) throw new Exception("参数错误");
            return SuccessRes(oCRService.GetApplePointByCardId(applyPointHistoryRequest));
        }

        /// <summary>
        /// 自动积分并微信推送接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("WebPosForPoint")]
        public IActionResult WebPosForPoint([FromBody]WebPosRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.arg) || string.IsNullOrWhiteSpace(req.cardId))
                throw new Exception("参数错误");
            return SuccessRes(oCRService.CommitApplyPoint(req));
        }
    }
}