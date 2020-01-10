using AS.OCR.Model.Entity;

namespace AS.OCR.IDAO
{
    public interface IAccountDAO
    {
        Account GetByAppId(string AppId);
    }
}
