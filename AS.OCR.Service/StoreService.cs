using AS.OCR.Commom.Configuration;
using AS.OCR.Extension.SDK.TencentOCR;
using AS.OCR.IDAO;
using AS.OCR.IService;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using AS.OCR.Model.Enum;
using AS.OCR.Model.Request;
using AS.OCR.Model.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AS.OCR.Service
{
    public class StoreService : Infrastructure, IStoreService
    {
        private ILogger _logger { get; }

        public StoreService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(OCRService));
        }

    }
}
