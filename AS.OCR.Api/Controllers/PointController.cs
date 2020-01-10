using System;
using AS.OCR.IService;
using AS.OCR.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AS.OCR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PointController : BaseController
    {
        private ILogger _logger { get; }
        private IOCRService _oCRService;
        public PointController(ILoggerFactory loggerFactory, IOCRService oCRService)
        {
            _logger = loggerFactory.CreateLogger(typeof(OCRController));
            _oCRService = oCRService;
        }

        /// <summary>
        /// 积分申请
        /// </summary>
        /// <param name="applyPointRequest"></param>
        /// <returns></returns>
        [HttpPost("ApplyPoint")]
        public IActionResult ApplyPoint([FromBody]CreateApplyPointRequest applyPointRequest)
        {
            //if (applyPointRequest == null ||
            //    string.IsNullOrWhiteSpace(applyPointRequest.cardId) ||
            //     string.IsNullOrWhiteSpace(applyPointRequest.mallId) ||
            //    applyPointRequest.receiptOCR == null)
            //    throw new Exception("参数错误");
            //return SuccessRes(oCRService.CreateApplyPoint(applyPointRequest));

            return SuccessRes("");
        }

        /// <summary>
        /// 积分申请历史记录
        /// </summary>
        /// <param name="applyPointHistoryRequest"></param>
        /// <returns></returns>
        [HttpPost("ApplyPointHistory")]
        public IActionResult ApplyPointHistory([FromBody]ApplyPointHistoryRequest applyPointHistoryRequest)
        {
            //if (applyPointHistoryRequest == null) throw new Exception("参数错误");
            //return SuccessRes(oCRService.GetApplePointByCardId(applyPointHistoryRequest));

            return SuccessRes("");
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
            //return SuccessRes(oCRService.CommitApplyPoint(req));
            return SuccessRes("");
        }
    }
}