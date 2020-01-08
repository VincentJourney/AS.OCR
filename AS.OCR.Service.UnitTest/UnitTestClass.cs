using AS.OCR.Service;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AS.OCR.Model.Request;

namespace AS.OCR.Service.Tests
{

    [TestClass()]
    public class UnitTestClass
    {
        private OCRService oCRService;
        public UnitTestClass(ILoggerFactory loggerFactory)
        {
            oCRService = new OCRService(loggerFactory);
        }
        [TestMethod()]
        public void ReceiptOCRTestOCR()
        {
            //var req = new ReceiptRequest
            //{
            //    imageUrl = "https://crm.kingkeybanner.com/FileServer/Files/2019/12/13/14/qwe123.jpg",
            //    mallId = "25E4DF19-F956-41FE-B935-0FB2AF3501B0",
            //    Type = 0
            //};
            //oCRService.ReceiptOCR(req);
        }

        //{
        //  "mallId": "25E4DF19-F956-41FE-B935-0FB2AF3501B0",
        //  "imageUrl": "https://crm.kingkeybanner.com/FileServer/Files/2019/12/13/14/qwe123.jpg",
        //  "type": 0
        //}

    }
}

