using AS.OCR.Model.Entity;
using System;

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
