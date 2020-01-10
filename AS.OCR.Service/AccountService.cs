using AS.OCR.IDAO;
using AS.OCR.IService;
using AS.OCR.Model.Entity;
using System;

namespace AS.OCR.Service
{
    public class AccountService : Infrastructure, IAccountService
    {
        private IAccountDAO _accountDAO { get; }
        public AccountService(IAccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
        }
        public Account GetByAppId(string AppId) => _accountDAO.GetByAppId($"AppId='{AppId}'");

        public Account Get(Guid Id) => _accountDAO.Get(Id);
    }
}
