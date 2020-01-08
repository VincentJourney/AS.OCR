using Dapper.Contrib.Extensions;
using System;
namespace AS.OCR.Model.Entity
{
    [Table("OCRVerifyRule")]
    public class OCRVerifyRule : AbstractEntity
    {
        public Guid StoreId { get; set; }
        public string OCRKey { get; set; }
        /// <summary>
        /// 关键字类型  1 Store 2 ReceiptNO 3 DateTime 4 Amount 5 Mall
        /// </summary>
        public int KeyType { get; set; }
        /// <summary>
        /// 取值方法 1 关键字 2 关键字后 3 隔行 4 关键字前 5 关键字间
        /// </summary>
        public int GetValueWay { get; set; }
        public int SkipLines { get; set; }
        public string GetValueRule { get; set; }
    }
}

