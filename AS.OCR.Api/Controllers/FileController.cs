using System;
using AS.OCR.Commom.Configuration;
using AS.OCR.Commom.Http;
using AS.OCR.IService;
using AS.OCR.Model.Request;
using AS.OCR.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AS.OCR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : BaseController
    {
        private ILogger _logger { get; }
        private IOCRService _oCRService { get; }
        public FileController(ILoggerFactory loggerFactory, IOCRService oCRService)
        {
            _logger = loggerFactory.CreateLogger(typeof(OCRController));
            _oCRService = oCRService;
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="Base64"></param>
        /// <returns></returns>
        [HttpPost("FileUpLoad")]
        public IActionResult ImageUpLoad([FromBody]string Base64)
        {
            ImageRequest request = new ImageRequest()
            {
                fileName = $"{Guid.NewGuid().ToString()}.jpg",
                base64Str = Base64,
                sourceSystem = "crm-ocr",
                fileDescription = "资源上传图片"
            };
            var result = HttpHelper.HttpPost(ConfigurationUtil.FileUploadUrl, JsonConvert.SerializeObject(request));
            if (string.IsNullOrWhiteSpace(result))
                return FailRes("文件上传失败");

            return SuccessRes(JsonConvert.DeserializeObject<ImageResponse>(result));
        }
    }
}