using Dapper.Contrib.Extensions;
using System;

namespace AS.OCR.Model.Entity
{
    [Table("Store")]
    public class Store : AbstractEntity
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
    }
}
