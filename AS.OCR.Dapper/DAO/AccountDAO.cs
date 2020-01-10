using AS.OCR.Dapper.Base;
using AS.OCR.IDAO;
using AS.OCR.Model.Entity;

namespace AS.OCR.Dapper.DAO
{
    public class AccountDAO : Infrastructure<Account>, IAccountDAO
    {
        public Account GetByAppId(string AppId) => GetModel($"AppId='{AppId}'");
    }
}
