using AS.OCR.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.OCR.Model.Business
{
    /// <summary>
    /// 公开变量 线程内保持唯一
    /// </summary>
    public class AccountInfo
    {
        [ThreadStatic]
        public static Account Account;

        public static void Dispose()
        {
            AccountInfo.Account = null;
        }
    }
}
