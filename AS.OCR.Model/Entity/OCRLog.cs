using Dapper.Contrib.Extensions;
using System;

namespace AS.OCR.Model.Entity
{
    [Table("OCRLog")]
    public class OCRLog : AbstractEntity
    {
        public Guid? ApplyId { get; set; }
        public string ImageUrl { get; set; }
        public string OriginalWords { get; set; }
        public Guid? StoreId { get; set; }
        public string StoreName { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? TransDatetime { get; set; }
        public decimal TranAmount { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? MallId { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
