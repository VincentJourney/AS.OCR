using AS.OCR.Model.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Model.Request
{
    public class CreateApplyPointRequest
    {
        public string UnionId { get; set; }
        public ReceiptOCRResponse receiptOCR { get; set; }
    }
}
