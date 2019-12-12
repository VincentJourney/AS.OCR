using System;
using System.Threading.Tasks;
using AS.OCR.Extension.SDK.TencentOCR;
using AS.OCR.Model.Business;
using AS.OCR.Model.Request;
using AS.OCR.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BaiDuOCR.Controllers
{
    [Produces("application/json")]
    [Route("api/ImageOCR")]
    public class OCRController : ControllerBase
    {
        private ILogger _logger { get; set; }
        public OCRController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(OCRController));
        }

        /// <summary>
        /// 图片识别  {"mallId":"25e4df19-f956-41fe-b935-0fb2af3501b0","imageUrl":""}
        /// </summary>
        /// <param name="oCRRequest"></param>
        /// <returns></returns>
        [HttpPost("OCR")]
        public async Task<Result> RecognitOCRResult([FromBody]OCRRequest oCRRequest)
        {
            if (oCRRequest == null)
                throw new Exception("参数错误");
            return await OCRVerify.ReceiptOCR(oCRRequest);
        }

        /// <summary>
        /// 积分申请
        /// </summary>
        /// <param name="applyPointRequest"></param>
        /// <returns></returns>
        [HttpPost("ApplyPoint")]
        public Result ApplyPoint([FromBody]ApplyPointRequest applyPointRequest)
        {
            if (applyPointRequest == null ||
                string.IsNullOrWhiteSpace(applyPointRequest.cardId) ||
                 string.IsNullOrWhiteSpace(applyPointRequest.mallId) ||
                applyPointRequest.receiptOCR == null)
                throw new Exception("参数错误");
            return OCRVerify.CreateApplyPoint(applyPointRequest);
        }

        /// <summary>
        /// 积分申请历史记录
        /// </summary>
        /// <param name="applyPointHistoryRequest"></param>
        /// <returns></returns>
        [HttpPost("ApplyPointHistory")]
        public Result ApplyPointHistory([FromBody]ApplyPointHistoryRequest applyPointHistoryRequest)
        {
            if (applyPointHistoryRequest == null)
                throw new Exception("参数错误");
            return OCRVerify.GetApplePointByCardId(applyPointHistoryRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base64"></param>
        /// <returns></returns>
        [HttpPost("ImageUpLoad")]
        public Result ImageUpLoad([FromBody]string Base64) =>
            OCRVerify.ImageUpload(Base64);

        /// <summary>
        /// 测试腾讯云OCR
        /// </summary>
        /// <param name="ImageUrl"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [HttpPost("TencentCloudOCR")]
        public TencentOCRResult TencentCloudOCR([FromBody]TencentCloudOCRRequest tencentCloudOCRRequest)
        {
            if (tencentCloudOCRRequest == null)
            {
                throw new Exception("参数错误");
            }
            return TencentOCR.GeneralAccurateOCR(tencentCloudOCRRequest.Image, tencentCloudOCRRequest.Type);
        }

        /// <summary>
        /// 自动积分并微信推送接口
        /// </summary>
        /// <param name="webPosArg"></param>
        /// <returns></returns>
        [HttpPost("WebPosForPoint")]
        public async Task<Result> WebPosForPoint([FromBody]WebPosRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.arg) || string.IsNullOrWhiteSpace(req.cardId))
                throw new Exception("参数错误");
            return await OCRVerify.CommitApplyPoint(req);
        }
    }
}