using AS.OCR.IDAO.Base;
using AS.OCR.Model.Entity;

namespace AS.OCR.IDAO
{
    public interface IAccountDAO: IBaseDAO<Account>
    {
        Account GetByAppId(string AppId);
    }
}
