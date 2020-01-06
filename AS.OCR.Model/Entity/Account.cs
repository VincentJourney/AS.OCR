using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.OCR.Model.Entity
{
    [Table("ApplyPoint")]
    public class Account : AbstractEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string AccountName { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public int Total { get; set; }
        public int Used { get; set; }
        public int Balance { get; set; }
        public DateTime AddedOn { get; set; }
        public byte Status { get; set; }
    }
}
