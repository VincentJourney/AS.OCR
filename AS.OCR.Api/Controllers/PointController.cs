using System;
using AS.OCR.IDAO;
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
        private IApplyPointDAO _applyPointDAO;
        public PointController(ILoggerFactory loggerFactory, IApplyPointDAO applyPointDAO)
        {
            _logger = loggerFactory.CreateLogger(typeof(OCRController));
            _applyPointDAO = applyPointDAO;
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
        /// <param name="UnionId"></param>
        /// <returns></returns>
        [HttpGet("ApplyPointHistory")]
        public IActionResult ApplyPointHistory(string UnionId)
        {
            if (string.IsNullOrWhiteSpace(UnionId))
                return FailRes("参数错误");

            return SuccessRes(_applyPointDAO.GetApplyPointHistory(UnionId));
        }
    }
}