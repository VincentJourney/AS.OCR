﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            //=> SuccessRes(_oCRService.ImageUpload(Base64));
            => SuccessRes("");
    }
}