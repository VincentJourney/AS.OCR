using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Model.Entity
{
    [Table("ApplyPointRule")]
    public class ApplyPointRule : AbstractEntity
    {
        public Guid StoreId { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        /// <summary>
        /// 日自动交易笔数 每日最大自动积分记录，0代表不限制
        /// </summary>
        public int TotalMaxForDay { get; set; }
        /// <summary>
        /// 启动自动积分  0 不启动 1 启动
        /// </summary>
        public byte NeedVerify { get; set; }
        /// <summary>
        /// 是否有效  0 不启动 1 启动
        /// </summary>
        public byte Status { get; set; }
        public string AddedOn { get; set; }
    }
}
