using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AS.OCR.Commom;
using AS.OCR.Commom.Attributes;

namespace AS.OCR.Model.Business
{
    public class ReceiptOCRResponse
    {
        /// <summary>
        /// 识别原始表Id
        /// </summary>
        [ValidateAttr(IgnoreKey = "RecongnizelId")]
        public Guid RecongnizelId { get; set; }
        public Guid? StoreId { get; set; }
        public string StoreName { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? TransDatetime { get; set; }
        public decimal? TranAmount { get; set; }
        public Guid? AccountId { get; set; }
        public string AccountName { get; set; }
    }
}
