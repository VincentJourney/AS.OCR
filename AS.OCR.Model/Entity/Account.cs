﻿using Dapper.Contrib.Extensions;
using System;

namespace AS.OCR.Model.Entity
{
    [Table("Account")]
    public class Account : AbstractEntity
    {
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
