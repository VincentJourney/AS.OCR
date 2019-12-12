using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Model.Business
{
    public class ApplyPointModel
    {
        public Guid ApplyPointId { get; set; }
        public string MallName { get; set; }
        public string StoreName { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TransDate { get; set; }
        public string ReceiptNo { get; set; }
        public decimal TransAmt { get; set; }
        /// <summary>
        /// 0 未审核  1 已审核  2  驳回
        /// </summary>
        public int Status { get; set; }
        public string Photo { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime AddedOn { get; set; }
        /// <summary>
        /// 识别状态 0 未识别  1 已解析原始数据  2 成功完成关键字匹配 3 未成功完成关键字匹配
        /// </summary>
        public int RecongizeStatus { get; set; } = 0;
        /// <summary>
        /// 校验状态 0 校验失败 1 校验成功
        /// </summary>
        public int VerifyStatus { get; set; } = 0;
    }
}
