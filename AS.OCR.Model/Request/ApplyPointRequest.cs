using AS.OCR.Model.Business;
using AS.OCR.Model.Response;

namespace AS.OCR.Model.Request
{
    public class CreateApplyPointRequest
    {
        public string UnionId { get; set; }
        public ReceiptOCRResponse receiptOCR { get; set; }
    }
}
