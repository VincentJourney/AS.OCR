using System;

namespace AS.OCR.Model.Business
{
    public class WebPosArg
    {
        public string companyID { get; set; }
        public string orgID { get; set; }
        public string storeID { get; set; }
        public string cashierID { get; set; }
        public decimal discountPercentage { get; set; }
        public string cardID { get; set; }
        public DateTime txnDateTime { get; set; }
        public string receiptNo { get; set; }
        public decimal amount { get; set; }
    }
}
