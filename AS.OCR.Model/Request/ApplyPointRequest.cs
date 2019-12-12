using AS.OCR.Model.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Model.Request
{
    public class ApplyPointRequest
    {
        public string cardId { get; set; }
        public string mallId { get; set; }
        public ReceiptOCR receiptOCR { get; set; }
    }
}
