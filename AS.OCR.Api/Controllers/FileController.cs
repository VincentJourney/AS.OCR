﻿using System;
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
    [Authorize]
    public class FileController : ControllerBase
    {
        private ILogger _logger { get; set; }
        private OCRService oCRService;
        public FileController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(OCRController));
            oCRService = new OCRService(loggerFactory);
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="Base64"></param>
        /// <returns></returns>
        [HttpPost("FileUpLoad")]
        public Result<string> ImageUpLoad([FromBody]string Base64) => oCRService.ImageUpload(Base64);
    }
}