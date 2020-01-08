using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Model.Entity
{
    [Table("ApplyPoint")]
    public class ApplyPoint : AbstractEntity
    {
        public Guid AccountId { get; set; }
        public Guid? MallId { get; set; }
        public Guid? StoreId { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? TransDate { get; set; }
        public decimal? TransAmt { get; set; }
        public string MobileNO { get; set; }
        public string UnionId { get; set; }
        public string ImageUrl { get; set; }
        /// <summary>
        /// 审核状态 0 未审核  1审核通过  2驳回
        /// </summary>
        public byte AuditStatus { get; set; }
        public string Auditor { get; set; }
        public DateTime AuditDate { get; set; }
        public string Remark { get; set; }
        public DateTime AddedOn { get; set; }
        /// <summary>
        /// 校验状态 0 校验失败 1校验成功
        /// </summary>
        public byte VerifyStatus { get; set; }
        /// <summary>
        /// 识别状态 0 识别不完整 1 识别成功
        /// </summary>
        public byte RecognitionStatus { get; set; }
    }
}
