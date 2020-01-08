using AS.OCR.Dapper;
using AS.OCR.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.OCR.Service
{
    public class AccountService : Infrastructure
    {
        private static readonly AccountDAO accountDAO = new AccountDAO();
        public Account GetByAppId(string AppId) => accountDAO.GetByAppId($"AppId='{AppId}'");

        public Account Get(Guid Id) => accountDAO.Get(Id);
    }
}
