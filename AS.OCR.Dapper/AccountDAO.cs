using AS.OCR.Dapper.Base;
using AS.OCR.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.OCR.Dapper
{
    public class AccountDAO : Infrastructure<Account>
    {
        public Account GetByAppId(string AppId) => GetModel($"AppId='{AppId}'");
    }
}
